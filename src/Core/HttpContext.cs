using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace Swerva
{
    public sealed class HttpContext
    {
        public Stream Stream { get; private set; }
        public HttpRequest Request { get; private set; }

        public HttpContext(Stream stream, HttpRequest request)
        {
            this.Stream = stream;
            this.Request = request;
        }
    }
}