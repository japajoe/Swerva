﻿using System;
using System.Threading.Tasks;
using Swerva.Controllers;

namespace Swerva
{
    class Program
    {
        static HttpServer server;
        static HttpRouteMapper routeMapper;

        static async Task Main(string[] args)
        {
            /*In order to support https, you need to have a certificate.
            To generate a self signed certificate run following command:
            dotnet dev-certs https -ep certicatename.pfx -p passwordhere
            */

            Console.CancelKeyPress += OnCancelKeyPress;

            HttpConfig config = HttpConfig.LoadFromFile("settings.json");
            
            routeMapper = new HttpRouteMapper();
            routeMapper.Add<IndexController>("/");
            routeMapper.Add<IndexController>("/index");
            routeMapper.Add<IndexController>("/home");
            routeMapper.Add<NotFoundController>("/404", true);

            server = new HttpServer(config, routeMapper);
            await server.Run();
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            server?.Stop();
        }
    }
}