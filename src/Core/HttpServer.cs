using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Text;
using Swerva.Utilities;

namespace Swerva
{
    public sealed class HttpServer
    {
        public event AsyncEventHandler<HttpRequestEvent> Request;

        private volatile bool run;
        private TcpListener httpListener = null;
        private TcpListener httpsListener = null;
        private X509Certificate certificate = null;

        public HttpServer(HttpConfig config)
        {
            HttpSettings.LoadFromConfig(config);

            this.httpListener = new TcpListener(IPAddress.Any, HttpSettings.Port);
            
            if(HttpSettings.UseHttps)
                this.httpsListener = new TcpListener(IPAddress.Any, HttpSettings.SslPort);
            
            this.certificate = new X509Certificate2(HttpSettings.CertificatePath, HttpSettings.CertificatePassword);
        }

        public async Task Run()
        {
            if(run)
                return;

            run = true;

            if(HttpSettings.UseHttps)
            {
                HttpLog.WriteLine("Server started listening on http://localhost:" + HttpSettings.Port + " and https://localhost:" + HttpSettings.SslPort);
                Task handleHttp = HandleHttp();
                Task handleHttps = HandleHttps();
                await Task.WhenAll(handleHttp, handleHttps);
            }
            else
            {
                HttpLog.WriteLine("Server started listening on http://localhost:" + HttpSettings.Port);
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
                    int headerSize = ReadHeader(stream, out string header);

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

                        HttpResponse response = new HttpResponse(HttpStatusCode.MovedPermanently, new HttpContentType(MediaType.TextPlain));
                        response.AddHeader("Location", location);
                        response.AddHeader("Connection", "close");
                        await response.Send(new HttpContext(stream, null));
                    }
                    else
                    {
                        if(headerSize > 0 && headerSize <= HttpSettings.MaxHeaderSize)
                            await HandleRequest(client, header, stream);
                        else
                            await HandleInvalidRequest(client, header, stream);
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

                    int headerSize = ReadHeader(stream, out string header);
                    
                    if(headerSize > 0 && headerSize <= HttpSettings.MaxHeaderSize)
                        await HandleRequest(client, header, stream);
                    else
                        await HandleInvalidRequest(client, header, stream);
                }
            }
        }

        private int ReadHeader(Stream stream, out string header)
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

        private async Task HandleRequest(TcpClient client, string request, Stream stream)
        {
            if(HttpRequest.TryParse(request, out HttpRequest httpRequest))
            {
                var userHostAddress = GetUserHostAddress(client);
                HttpContext context = new HttpContext(stream, httpRequest, userHostAddress);               

                if (Request != null)
                {
                    await Request.InvokeAllAsync(this, new HttpRequestEvent(context));
                }
            }
            else
            {
                HttpContext context = new HttpContext(stream, null);
                var response = new HttpResponse(HttpStatusCode.BadRequest, new HttpContentType(MediaType.TextHtml), "Bad request");
                response.AddHeader("Cache-Control", "max-age=3600");
                await response.Send(context);
            }
        }

        private async Task HandleInvalidRequest(TcpClient client, string request, Stream stream)
        {
            HttpContext context = new HttpContext(stream, null);
            var response = new HttpResponse(HttpStatusCode.RequestHeaderFieldsTooLarge, new HttpContentType(MediaType.TextHtml), "The request header is too large");
            await response.Send(context);
        }

        private IPEndPoint GetUserHostAddress(TcpClient client)
        {
            IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            return remoteIpEndPoint;
        }
    }
}