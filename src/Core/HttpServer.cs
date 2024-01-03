using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Text;

namespace Swerva
{
    public class HttpServer
    {
        private volatile bool run;
        private TcpListener httpListener = null;
        private TcpListener httpsListener = null;
        private X509Certificate certificate = null;
        private HttpRouteMapper routeMapper = null;

        public HttpServer(HttpConfig config, HttpRouteMapper routeMapper)
        {
            HttpSettings.LoadFromConfig(config);

            this.httpListener = new TcpListener(IPAddress.Any, HttpSettings.Port);
            
            if(HttpSettings.UseHttps)
                this.httpsListener = new TcpListener(IPAddress.Any, HttpSettings.SslPort);
            
            this.certificate = new X509Certificate2(HttpSettings.CertificatePath, HttpSettings.CertificatePassword);
            this.routeMapper = routeMapper;
        }

        public async Task Run()
        {
            if(run)
                return;

            run = true;

            if(HttpSettings.UseHttps)
            {
                HttpLog.WriteLine("Server started");
                HttpLog.WriteLine("HTTP port:  " + HttpSettings.Port);
                HttpLog.WriteLine("HTTPS port: " + HttpSettings.SslPort);
                HttpLog.WriteLine("Maximum header size: " + HttpSettings.MaxHeaderSize);

                Task handleHttp = HandleHttp();
                Task handleHttps = HandleHttps();
                await Task.WhenAll(handleHttp, handleHttps);
            }
            else
            {
                HttpLog.WriteLine("Server started");
                HttpLog.WriteLine("HTTP port:  " + HttpSettings.Port);
                HttpLog.WriteLine("Maximum header size: " + HttpSettings.MaxHeaderSize);

                Task handleHttp = HandleHttp();
                await Task.WhenAll(handleHttp);
            }
            
        }

        private async Task HandleHttp()
        {
            httpListener.Start();

            while(run)
            {
                try
                {
                    await HandleHttpConnection();
                }
                catch(Exception ex)
                {
                    HttpLog.WriteLine(ex.Message);
                }
            }
        }

        private async Task HandleHttps()
        {
            httpsListener.Start();

            while(run)
            {
                try
                {
                    await HandleHttpsConnection();
                }
                catch(Exception ex)
                {
                    HttpLog.WriteLine(ex.Message);
                }
            }
        }

        public void Stop()
        {
            run = false;
        }

        private async Task HandleHttpConnection()
        {
            using(var client = await httpListener.AcceptTcpClientAsync())
            {
                using(var stream = client.GetStream())
                {
                    byte[] buffer = new byte[HttpSettings.MaxHeaderSize];
                    int headerSize = ReadHeader(buffer, stream, out string header);

                    if(HttpSettings.UseHttps && HttpSettings.UseHttpsForwarding)
                    {
                        string location = "https://localhost:" + HttpSettings.SslPort;

                        if(headerSize > 0 && headerSize <= HttpSettings.MaxHeaderSize)
                        {
                            if(HttpRequest.TryParse(header, out HttpRequest httpRequest))
                            {
                                location = location + httpRequest.RawURL;
                            }
                        }

                        string redirectResponse = "HTTP/1.1 301 Moved Permanently\r\n" + 
                        "Location: " + location + "\r\n" +
                        "Connection: close\r\n" + "/\r\n\r\n";
                        var response = Encoding.UTF8.GetBytes(redirectResponse);

                        await stream.WriteAsync(response, 0, response.Length);
                    }
                    else
                    {
                        if(headerSize > 0 && headerSize <= HttpSettings.MaxHeaderSize)
                            await HandleRequest(header, buffer, stream);
                        else
                            await HandleInvalidRequest(header, buffer, stream);
                    }
                }
            }
        }

        private async Task HandleHttpsConnection()
        {
            using(var client = await httpsListener.AcceptTcpClientAsync())
            {
                using(var stream = new SslStream(client.GetStream(), false))
                {
                    stream.AuthenticateAsServer(certificate, clientCertificateRequired: false, checkCertificateRevocation: true);

                    byte[] buffer = new byte[HttpSettings.MaxHeaderSize];

                    int headerSize = ReadHeader(buffer, stream, out string header);
                    
                    if(headerSize > 0 && headerSize <= HttpSettings.MaxHeaderSize)
                        await HandleRequest(header, buffer, stream);
                    else
                        await HandleInvalidRequest(header, buffer, stream);
                }
            }
        }

        private int ReadHeader(byte[] buffer, Stream stream, out string header)
        {
            header = string.Empty;

            // Read the header byte by byte until you encounter a double CRLF ("\r\n\r\n") indicating the end of the header
            StringBuilder headerBuilder = new StringBuilder();
            int prevByte = -1;
            int currentByte;
            int headerSize = 0;

            while ((currentByte = stream.ReadByte()) != -1)
            {
                headerBuilder.Append((char)currentByte);
                headerSize++;

                if(headerSize > HttpSettings.MaxHeaderSize)
                    break;

                // Check for the end of the header (double CRLF)
                if (prevByte == '\r' && currentByte == '\n')
                {
                    if (headerBuilder.ToString().EndsWith("\r\n\r\n"))
                    {
                        break;
                    }
                }

                prevByte = currentByte;
            }

            // At this point, headerBuilder.ToString() contains the entire HTTP header
            header = headerBuilder.ToString();
            return headerSize;
        }

        private async Task HandleRequest(string request, byte[] buffer, Stream stream)
        {
            if(HttpRequest.TryParse(request, out HttpRequest httpRequest))
            {
                HttpLog.WriteLine(httpRequest.Method + ": " + httpRequest.RawURL);
                HttpContext context = new HttpContext(stream, httpRequest);

                if(routeMapper.GetRoute(httpRequest.URL, out HttpRoute route, false))
                {
                    await route.ProcessRequest(context, buffer);
                }
                else
                {
                    string filepath = HttpSettings.PublicHtml + context.Request.URL;

                    //To do: fix the path to prevent looking for files outside of allowed directory
                    if(System.IO.File.Exists(filepath))
                    {
                        HttpCacheControl cacheControl = new HttpCacheControl()
                            .SetMaxAge(3600);

                        MediaType mediaType = MediaType.ApplicationOctetStream;

                        if(MimeTypeMap.TryGetMimeType(filepath, out string mimeType))
                        {
                            mediaType = HttpContentType.GetMediaTypeFromString(mimeType);
                        }

                        var filestream = new System.IO.FileStream(filepath, FileMode.Open, FileAccess.Read);
                        var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(mediaType), filestream);
                        response.CacheControl = cacheControl;
                        await response.Send(context, buffer);
                    }
                    else
                    {
                        if(routeMapper.GetRoute("/404", out route, true))
                        {
                            await route.ProcessRequest(context, buffer);
                        }
                        else
                        {
                            var response = new HttpResponse(HttpStatusCode.NotFound, new HttpContentType(MediaType.TextHtml), "The requested document was not found");
                            await response.Send(context, buffer);
                        }
                    }
                }
            }
            else
            {
                HttpContext context = new HttpContext(stream, null);
                var response = new HttpResponse(HttpStatusCode.BadRequest, new HttpContentType(MediaType.TextHtml), "Bad request");
                await response.Send(context, buffer);
            }
        }

        private async Task HandleInvalidRequest(string request, byte[] buffer, Stream stream)
        {
            HttpContext context = new HttpContext(stream, null);
            var response = new HttpResponse(HttpStatusCode.RequestHeaderFieldsTooLarge, new HttpContentType(MediaType.TextHtml), "The request header is too large");
            await response.Send(context, buffer);
        }
    }
}