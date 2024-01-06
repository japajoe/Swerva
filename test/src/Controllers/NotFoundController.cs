using Swerva;
using System.Threading.Tasks;

namespace SwervaTester.Controllers
{
    public sealed class NotFoundController : HttpControllerBase
    {
        private static HttpPageContent content;
        private string template;

        public NotFoundController()
        {
            if(content == null)
                content = new HttpPageContent(HttpSettings.PrivateHtml + "/template.html");
        }

        public async override Task<HttpResponse> OnGet(HttpContext context)
        {
            bool result = await content.LoadAsync();

            if(result)
            {
                template = content.Content;
                template = template.Replace("$(title)", "Not Found - Swerva Web");
                template = template.Replace("$(header_text)", "Error 404");
                template = template.Replace("$(head)", "");
                template = template.Replace("$(content)", "<p>The requested document was not found</p>");
            }

            var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextHtml), template);
            response.AddHeader("Cache-Control", "max-age=3600");
            return response;
        }
    }
}