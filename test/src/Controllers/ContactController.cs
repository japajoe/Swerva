using Swerva;
using System.Threading.Tasks;

namespace SwervaTester.Controllers
{
    public sealed class ContactController : HttpControllerBase
    {
        private static HttpPageContent content;
        private string template;

        public ContactController()
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
                template = template.Replace("$(title)", "Contact - Swerva Web");
                template = template.Replace("$(header_text)", "Contact");
                template = template.Replace("$(head)", "");
                template = template.Replace("$(content)", "<p>Contact me on <a href=\"https://github.com/japajoe\" target=\"blank\">GitHub</a></p>");
            }

            var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextHtml), template);
            response.AddHeader("Cache-Control", "max-age=60");
            return response;
        }
    }
}