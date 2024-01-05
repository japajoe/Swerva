using System.Text;
using System.Threading.Tasks;

namespace Swerva.Controllers
{
    public sealed class ExampleController : HttpControllerBase
    {
        private static HttpPageContent content;
        private string template;

        public ExampleController()
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
                template = template.Replace("$(title)", "Example - Swerva Web");
                template = template.Replace("$(header_text)", "Example");
                template = template.Replace("$(head)", "<script src=\"api.js\"></script>");

                StringBuilder sb = new StringBuilder();
                sb.Append("<p>Click the button to send a POST request<p>");
                sb.Append("<p><button type=\"button\" class=\"btn btn-success\" id=\"buttonSend\">Send</button></p>");
                sb.Append("<p id=\"responseArea\"></p>");
                
                template = template.Replace("$(content)", sb.ToString());
            }

            var response = new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.TextHtml), template);
            response.AddHeader("Cache-Control", "max-age=60");
            return response;
        }
    }
}