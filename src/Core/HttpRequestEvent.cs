using System;

namespace Swerva
{
    public sealed class HttpRequestEvent : EventArgs
    {
        public HttpContext context;

        public HttpRequestEvent(HttpContext context)
        {
            this.context = context;
        }
    }
}