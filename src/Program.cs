using System;
using System.Threading.Tasks;
using Swerva.Controllers;

namespace Swerva
{
    class Program
    {
        static HttpApplication application;
        static HttpRouteMapper routeMapper;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += OnCancelKeyPress;

            HttpConfig config = HttpConfig.LoadFromFile("settings.json");
            
            routeMapper = new HttpRouteMapper();
            routeMapper.Add<IndexController>("/");
            routeMapper.Add<IndexController>("/index");
            routeMapper.Add<IndexController>("/home");
            routeMapper.Add<ExampleController>("/example");
            routeMapper.Add<ContactController>("/contact");
            routeMapper.Add<APIController>("/api/v1");

            application = new HttpApplication(config, routeMapper);
            await application.Run();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            application?.Stop();
        }
    }
}