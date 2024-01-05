using System.Text;

namespace Swerva
{
    public sealed class HttpResponseBuilder
    {
        private StringBuilder stringBuilder;
        
        public HttpResponseBuilder()
        {
            stringBuilder = new StringBuilder();
        }

        public void Start(HttpStatusCode status)
        {
            stringBuilder.Append("HTTP/1.1 " + (int)status + "\n");
        }

        public void AddHeader(string key, string value)
        {
            stringBuilder.Append(key + ": " + value + "\n");
        }

        public void AddString(string value)
        {
            stringBuilder.Append(value);
        }

        public void End()
        {
            stringBuilder.Append("\n");
        }

        public void Clear()
        {
            stringBuilder.Clear();
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }        
    }
}