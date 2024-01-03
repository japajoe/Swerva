using System;
using System.Collections.Generic;

namespace Swerva
{
    public class HttpCacheControl
    {
        private List<string> directives;

        public HttpCacheControl()
        {
            directives = new List<string>();
        }

        public HttpCacheControl SetMaxAge(int seconds)
        {
            if (seconds < 0)
            {
                throw new ArgumentException("Seconds must be non-negative.", nameof(seconds));
            }

            directives.Add($"max-age={seconds}");
            return this;
        }

        public HttpCacheControl SetNoCache()
        {
            directives.Add("no-cache");
            return this;
        }

        public HttpCacheControl SetNoStore()
        {
            directives.Add("no-store");
            return this;
        }

        public HttpCacheControl SetPublic()
        {
            directives.Add("public");
            return this;
        }

        public HttpCacheControl SetPrivate()
        {
            directives.Add("private");
            return this;
        }

        public HttpCacheControl SetMustRevalidate()
        {
            directives.Add("must-revalidate");
            return this;
        }

        public HttpCacheControl AddCustomDirective(string directive)
        {
            if (string.IsNullOrWhiteSpace(directive))
            {
                throw new ArgumentException("Directive must not be null or empty.", nameof(directive));
            }

            directives.Add(directive);
            return this;
        }

        public string Build()
        {
            if (directives.Count == 0)
            {
                throw new InvalidOperationException("Cache-Control must have at least one directive.");
            }

            return string.Join(", ", directives);
        }
    }
}