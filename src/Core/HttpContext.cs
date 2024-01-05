using System.IO;
using System.Net;

namespace Swerva
{
    public sealed class HttpContext
    {
        public Stream Stream { get; private set; }
        public HttpRequest Request { get; private set; }
        public IPEndPoint UserHostAddress { get; private set; }

        public HttpContext(Stream stream, HttpRequest request, IPEndPoint userHostAddress)
        {
            this.Stream = stream;
            this.Request = request;
            this.UserHostAddress = userHostAddress;
        }

        public HttpContext(Stream stream, HttpRequest request)
        {
            this.Stream = stream;
            this.Request = request;
            this.UserHostAddress = null;
        }
    }
}