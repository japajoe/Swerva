using System.Collections.Generic;

namespace Swerva
{
    public sealed class HttpRouteMapper
    {
        private Dictionary<string, HttpRoute> routes;

        public HttpRouteMapper()
        {
            routes = new Dictionary<string, HttpRoute>();
        }

        public void Add<T>(string url, bool isInternal = false) where T : HttpControllerBase
        {
            if(!routes.ContainsKey(url))
            {
                routes.Add(url, new HttpRoute(url, typeof(T), isInternal));
            }
        }

        public bool GetRoute(string url, out HttpRoute route, bool allowInternal)
        {
            route = null;

            url = url.ToLower();

            if(routes.ContainsKey(url))
            {
                route = routes[url];

                if(route.IsInternal)
                {
                    if(allowInternal)
                        return true;
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}