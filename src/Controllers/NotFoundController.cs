using System.Threading.Tasks;

namespace Swerva.Controllers
{
    public sealed class NotFoundController : HttpControllerBase
    {
        private string template;
        private HttpCacheControl cacheControl;

        public NotFoundController()
        {
            cacheControl = new HttpCacheControl()
                .SetMaxAge(60);

            template = System.IO.File.ReadAllText(HttpSettings.PrivateHtml + "/template.html");
            template = template.Replace("$(title)", "404 - Not Found");
            template = template.Replace("$(header_text)", "Error 404");
            template = template.Replace("$(content)", "The requested document was not found");
        }

        public async override Task<HttpResponse> OnGet(HttpContext context)
        {
            await Task.Delay(1);

            var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextHtml), template);
            response.CacheControl = cacheControl;
            return response;
        }
    }
}