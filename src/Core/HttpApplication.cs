using System;
using System.IO;
using System.Threading.Tasks;
using Swerva.Controllers;

namespace Swerva
{
    public sealed class HttpApplication
    {
        private HttpServer server;
        private HttpRouteMapper routeMapper;

        public HttpApplication(HttpConfig config, HttpRouteMapper routeMapper)
        {
            this.routeMapper = routeMapper;
            this.routeMapper.Add<NotFoundController>("/404", true);

            server = new HttpServer(config);

            server.Request += async (sender, e) =>
            {
                await OnRequest(e.context);
            };
        }

        public async Task Run()
        {
            await server.Run();
        }

        public void Stop()
        {
            server.Stop();
        }

        private async Task OnRequest(HttpContext context)
        {
            HttpLog.WriteLine(context.Request.Method + ": " + context.Request.RawURL);

            if(routeMapper.GetRoute(context.Request.URL, out HttpRoute route, false))
            {
                var response = await route.GetResponse(context);
                await response.Send(context);
            }
            else
            {
                string filepath = HttpSettings.PublicHtml + context.Request.URL;

                //To do: fix the path to prevent looking for files outside of allowed directory
                if(System.IO.File.Exists(filepath) && IsPathWithinDirectory(filepath, HttpSettings.PublicHtml))
                {
                    HttpContentType contentType = HttpContentType.GetContentTypeFromFileExtension(filepath);
                    var filestream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                    var response = new HttpResponse(HttpStatusCode.OK, contentType, filestream);
                    response.AddHeader("Cache-Control", "max-age=3600");
                    await response.Send(context);
                }
                else
                {
                    if(routeMapper.GetRoute("/404", out route, true))
                    {
                        var response = await route.GetResponse(context);
                        await response.Send(context);
                    }
                    else
                    {
                        var response = new HttpResponse(HttpStatusCode.NotFound, new HttpContentType(MediaType.TextHtml));
                        response.AddHeader("Cache-Control", "max-age=3600");
                        await response.Send(context);
                    }
                }
            }          
        }

        private bool IsPathWithinDirectory(string path, string directory)
        {
            string fullPath = Path.GetFullPath(path);
            string fullDirectoryPath = Path.GetFullPath(directory);
            return fullPath.StartsWith(fullDirectoryPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}