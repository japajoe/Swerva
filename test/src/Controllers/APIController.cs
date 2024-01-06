using Swerva;
using System.Text.Json;
using System.Threading.Tasks;

namespace SwervaTester.Controllers
{
    public sealed class APIController : HttpControllerBase
    {
        private static int numRequests = 0;
        
        public async override Task<HttpResponse> OnPost(HttpContext context)
        {
            numRequests++;

            string json = await ReadContentAsStringAsync(context);
            HttpLog.WriteLine("Received JSON: " + json);
            
            APIResponse response = new APIResponse(numRequests, "Thank you!");

            return new HttpResponse(HttpStatusCode.OK, new HttpContentType(MediaType.ApplicationJson), response.Serialize());
        }
    }

    public class APIResponse
    {
        public int NumberOfRequests { get; set; }
        public string Message { get; set; }

        public APIResponse(int numberOfRequests, string message)
        {
            this.NumberOfRequests = numberOfRequests;
            this.Message = message;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize<APIResponse>(this);
        }
    }
}