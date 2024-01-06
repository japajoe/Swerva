using Swerva;
using System.Threading.Tasks;

namespace SwervaTester.Controllers
{
    public sealed class IndexController : HttpControllerBase
    {
        private static HttpPageContent content;
        private string template;

        public IndexController()
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
                template = template.Replace("$(title)", "Home - Swerva Web");
                template = template.Replace("$(header_text)", "Home");
                template = template.Replace("$(head)", "");
                template = template.Replace("$(content)", "<p>Welcome to the home page</p>");
            }

            var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextHtml), template);
            response.AddHeader("Cache-Control", "max-age=60");
            return response;
        }
    }
}