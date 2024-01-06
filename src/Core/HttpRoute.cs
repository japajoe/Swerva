using System;
using System.Threading.Tasks;

namespace Swerva
{
    public sealed class HttpRoute
    {
        public string URL { get; private set; }
        public Type ControllerType { get; private set; }
        public bool IsInternal { get; private set; }

        public HttpRoute(string url, Type controllerType, bool isInternal)
        {
            this.URL = url;
            this.ControllerType = controllerType;
            this.IsInternal = isInternal;
        }

        public async Task<HttpResponse> GetResponse(HttpContext context)
        {
            HttpControllerBase controller = (HttpControllerBase)Activator.CreateInstance(ControllerType);
            HttpResponse response = null;

            switch(context.Request.Method)
            {
                case HttpRequestMethod.Get:
                    response = await controller.OnGet(context);
                    break;
                case HttpRequestMethod.Post:
                    response = await controller.OnPost(context);
                    break;
                case HttpRequestMethod.Connect:
                    response = await controller.OnConnect(context);
                    break;
                case HttpRequestMethod.Head:
                    response = await controller.OnHead(context);
                    break;
                case HttpRequestMethod.Options:
                    response = await controller.OnOptions(context);
                    break;
                case HttpRequestMethod.Patch:
                    response = await controller.OnPatch(context);
                    break;
                case HttpRequestMethod.Delete:
                    response = await controller.OnDelete(context);
                    break;
                case HttpRequestMethod.Put:
                    response = await controller.OnPut(context);
                    break;
                case HttpRequestMethod.Trace:
                    response = await controller.OnTrace(context);
                    break;
                case HttpRequestMethod.Unknown:
                    response = new HttpResponse(HttpStatusCode.NotImplemented, new HttpContentType(MediaType.TextHtml), "Method not implemented");
                    break;
            }

            return response;            
        }     
    }
}