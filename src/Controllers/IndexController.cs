using System.Threading.Tasks;

namespace Swerva.Controllers
{
    public sealed class IndexController : HttpControllerBase
    {
        private string template;
        private HttpCacheControl cacheControl;

        public IndexController()
        {
            cacheControl = new HttpCacheControl()
                .SetMaxAge(60);

            template = System.IO.File.ReadAllText(HttpSettings.PrivateHtml + "/template.html");
            template = template.Replace("$(title)", "Home");
            template = template.Replace("$(header_text)", "Home");
            template = template.Replace("$(content)", "Welcome to the home page");
        }

        public async override Task<HttpResponse> OnGet(HttpContext context)
        {
            await Task.Delay(1);
            var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextHtml), template);
            response.CacheControl = cacheControl;
            return response;
        }

        public async override Task<HttpResponse> OnPost(HttpContext context)
        {
            string payload = await ReadContentAsString(context);
            HttpLog.WriteLine(payload);
            return new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextPlain));
        }
    }
}