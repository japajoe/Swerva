using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Swerva
{
    public sealed class HttpResponse
    {
        public HttpStatusCode StatusCode { get; private set; }
        public HttpContentType ContentType { get; private set; }
        public Stream Content { get; private set; }
        public HttpCacheControl CacheControl { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public HttpResponse(HttpStatusCode statusCode, HttpContentType contentType)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
            this.Content = null;
            this.CacheControl = null;
            this.Headers = new Dictionary<string, string>();
        }

        public HttpResponse(HttpStatusCode statusCode, HttpContentType contentType, Stream content)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
            this.Content = content;
            this.CacheControl = null;
            this.Headers = new Dictionary<string, string>();
        }

        public HttpResponse(HttpStatusCode statusCode, HttpContentType contentType, string content)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
            this.CacheControl = null;
            this.Headers = new Dictionary<string, string>();
            this.Content = new MemoryStream();
            var writer = new StreamWriter(this.Content);
            writer.Write(content);
            writer.Flush();
            this.Content.Position = 0;
        }

        private int WriteHeader(byte[] buffer, int offset, int count)
        {
            long contentLength = Content == null ? 0 : Content.Length;
            StringBuilder ss = new StringBuilder();
            ss.Append("HTTP/1.1 " + (int)StatusCode + "\n");
            ss.Append("Date: " + System.DateTime.UtcNow.ToString() + "\n");
            ss.Append("Server: " + HttpSettings.Name + "\n");

            if(Headers != null)
            {
                foreach(var item in Headers)
                {
                    ss.Append(item.Key + ": " + item.Value + "\n");
                }
            }

            if(contentLength > 0)
            {
                ss.Append("Content-Length: " + contentLength + "\n");
                ss.Append("Content-Type: " + ContentType.ToString() + "\n");
            }
            
            if(CacheControl != null)
            {
                string cacheControl = "Cache-Control: " + CacheControl.Build();
                ss.Append(cacheControl + "\n");
            }
            
            ss.Append("Connection: " + "close" + "\n\n");

            ReadOnlySpan<char> chars = ss.ToString().AsSpan();
            Span<byte> bytes = buffer.AsSpan(offset, count);

            return System.Text.Encoding.UTF8.GetBytes(chars, bytes);
        }

        private int WriteContent(byte[] buffer, int offset, int count)
        {
            if(Content == null)
                return 0;
            return Content.Read(buffer, offset, count);
        }

        public async Task Send(HttpContext context, byte[] buffer)
        {
            try
            {
                int numBytes = WriteHeader(buffer, 0, buffer.Length);
                await context.Stream.WriteAsync(buffer, 0, numBytes);

                while(numBytes > 0)
                {
                    numBytes = WriteContent(buffer, 0, buffer.Length);
                    
                    if(numBytes > 0)
                    {
                        await context.Stream.WriteAsync(buffer, 0, numBytes);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Content?.Dispose();
            }
        }
    }
}