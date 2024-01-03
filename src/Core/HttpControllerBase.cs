using System.Threading.Tasks;

namespace Swerva
{
    public class HttpControllerBase
    {
        public async virtual Task<HttpResponse> OnConnect(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnDelete(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnGet(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.NotFound, new HttpContentType(MediaType.TextHtml), "Not found");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnHead(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnOptions(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnPatch(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnPost(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnPut(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnTrace(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.MethodNotAllowed, new HttpContentType(MediaType.TextHtml), "Method not allowed");
            return await Task.Run(task);
        }

        public async virtual Task<HttpResponse> OnUnknown(HttpContext context)
        {
            var task = () => new HttpResponse(HttpStatusCode.NotImplemented, new HttpContentType(MediaType.TextHtml), "Not implemented");
            return await Task.Run(task);
        }

        protected async Task<string> ReadContentAsString(HttpContext context)
        {
            byte[] buffer = new byte[1024];

            if(context.Request.ContentLength > 0)
            {
                ulong bytesRead = 0;
                string payload = string.Empty;
                
                while(bytesRead < context.Request.ContentLength)
                {
                    int numBytes = await context.Stream.ReadAsync(buffer, 0, buffer.Length);
                    if(numBytes > 0)
                        payload += System.Text.Encoding.UTF8.GetString(buffer, 0, numBytes);
                    bytesRead += (ulong)numBytes;
                }

                return payload;
            }

            return string.Empty;
        }
    }
}