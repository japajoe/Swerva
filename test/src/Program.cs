using Swerva;
using System;
using System.Threading.Tasks;

namespace SwervaTester
{
    class Program
    {
        static HttpApplication application;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += OnCancelKeyPress;

            HttpConfig config = HttpConfig.LoadFromFile("settings.json");

            if(config == null)
                Environment.Exit(1);

            application = new HttpApplication(config);
            await application.Run();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            application?.Stop();
        }
    }
}