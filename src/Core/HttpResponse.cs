using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Buffers;

namespace Swerva
{
    public sealed class HttpResponse
    {
        public HttpStatusCode StatusCode { get; private set; }
        public HttpContentType ContentType { get; private set; }
        public Stream Content { get; private set; }
        public Dictionary<string, string> Headers { get; set; }

        public HttpResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
            this.ContentType = new HttpContentType(MediaType.TextPlain);
            this.Content = null;
            this.Headers = new Dictionary<string, string>();

            AddDefaultHeaders();
        }

        public HttpResponse(HttpStatusCode statusCode, HttpContentType contentType)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
            this.Content = null;
            this.Headers = new Dictionary<string, string>();

            AddDefaultHeaders();
        }

        public HttpResponse(HttpStatusCode statusCode, HttpContentType contentType, Stream content)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
            this.Content = content;
            this.Headers = new Dictionary<string, string>();

            AddDefaultHeaders();
        }

        public HttpResponse(HttpStatusCode statusCode, HttpContentType contentType, string content)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
            this.Headers = new Dictionary<string, string>();
            this.Content = new MemoryStream();
            var writer = new StreamWriter(this.Content);
            writer.Write(content);
            writer.Flush();
            this.Content.Position = 0;

            AddDefaultHeaders();
        }

        private void AddDefaultHeaders()
        {
            AddHeader("Date", System.DateTime.UtcNow.ToString());
            AddHeader("Server", HttpSettings.Name);            
        }

        private string GetHeader()
        {
            HttpResponseBuilder builder = new HttpResponseBuilder();
            builder.Start(StatusCode);

            if(Headers != null)
            {
                foreach(var item in Headers)
                {
                    builder.AddHeader(item.Key, item.Value);
                }
            }

            long contentLength = Content == null ? 0 : Content.Length;

            if(contentLength > 0)
            {
                builder.AddHeader("Content-Length", contentLength.ToString());
                builder.AddHeader("Content-Type",ContentType.ToString());
            }

            builder.End();

            return builder.ToString();
        }

        public void AddHeader(string key, string value)
        {
            Headers[key] = value;
        }

        public async Task Send(HttpContext context)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(HttpSettings.BufferSize);
            
            try
            {
                string header = GetHeader();

                int bytesPerChar = 4; //Wild assumption that a char can take up to 4 bytes
                int charsPerIteration = Math.Min(header.Length, buffer.Length / bytesPerChar);
                int numChars = header.Length;
                int charIndex = 0;                
                long numBytes = 0;

                while(charIndex < numChars)
                {
                    numBytes = Encoding.UTF8.GetBytes(header, charIndex, charsPerIteration, buffer, 0);
                    if(numBytes > 0)
                    {
                        await context.Stream.WriteAsync(buffer, 0, (int)numBytes);
                        charIndex += (int)numBytes++;
                    }
                }
                
                numBytes = (Content == null) ? 0 : Content.Length;

                while(numBytes > 0)
                {
                    numBytes = Content.Read(buffer, 0, buffer.Length);
                    
                    if(numBytes > 0)
                    {
                        await context.Stream.WriteAsync(buffer, 0, (int)numBytes);
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
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}