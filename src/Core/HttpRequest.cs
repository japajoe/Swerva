using System.Collections.Generic;

namespace Swerva
{
    public enum HttpRequestMethod
    {
        Connect,
        Delete,
        Get,
        Head,
        Options,
        Patch,
        Post,
        Put,
        Trace,
        Unknown
    }

    public sealed class HttpRequest
    {
        public HttpRequestMethod Method { get; private set; }
        public string URL { get; private set; }
        public string RawURL { get; set; }
        public string Host { get; private set; }
        public string UserAgent { get; private set; }
        public string Accept { get; private set; }
        public string AcceptLanguage { get; private set; }
        public string AcceptEncoding { get; private set; }
        public string DNT { get; private set; }
        public string Connection { get; private set; }
        public string Referer { get; private set; }
        public string SecFetchDest { get; private set; }
        public string SecFetchMode { get; private set; }
        public string SecFetchSite { get; private set; }
        public string CacheControl { get; private set; }        
        public string UpgradeInsecureRequests { get; private set; }
        public HttpContentType ContentType { get; set; }
        public ulong ContentLength { get; private set; }
        public Dictionary<string,string> KeyValuePairs { get; set; }
        public Dictionary<string,string> Cookies { get; set; }

        private static Dictionary<string, HttpRequestMethod> requestMethodTable = new Dictionary<string, HttpRequestMethod>();

        public HttpRequest()
        {
            if(requestMethodTable.Count == 0)
            {
                CreateRequestMethodTable();
            }
        }

        private void CreateRequestMethodTable()
        {
            requestMethodTable.Add("connect", HttpRequestMethod.Connect);
            requestMethodTable.Add("delete", HttpRequestMethod.Delete);
            requestMethodTable.Add("get", HttpRequestMethod.Get);
            requestMethodTable.Add("head", HttpRequestMethod.Head);
            requestMethodTable.Add("options", HttpRequestMethod.Options);
            requestMethodTable.Add("patch", HttpRequestMethod.Patch);
            requestMethodTable.Add("post", HttpRequestMethod.Post);
            requestMethodTable.Add("put", HttpRequestMethod.Put);
            requestMethodTable.Add("trace", HttpRequestMethod.Trace);
        }

        private void GetKeyValuePairs()
        {
            if(URL.Contains('?'))
            {
                var components = URL.Split('?');

                if(IsNullOrEmpty(components))
                    return;
                if(!IsOfMinimumSize(components, 2))
                    return;

                if(components[1].Contains('&'))
                {
                    if(components[1].Contains('='))
                    {                     
                        components = components[1].Split('&');

                        if(!IsOfMinimumSize(components, 2))
                            return;

                        for (int i = 0; i < components.Length; i++)
                        {
                            var keyValue = components[i].Split('=');

                            if(!IsOfMinimumSize(keyValue, 2))
                                continue;

                            string key = keyValue[0];
                            string value = keyValue[1];

                            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                                continue;

                            KeyValuePairs.Add(key, value);
                        }
                    }
                }
                else
                {
                    if(components[1].Contains('='))
                    {
                        components = components[1].Split('=');

                        if (!IsOfMinimumSize(components, 2))
                        {
                            return;
                        }

                        if (components.Length % 2 != 0)
                        {
                            return;
                        }

                        for (int i = 0; i < components.Length; i+=2)
                        { 
                            string key = components[i];
                            string value = components[+1];

                            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                                continue;
                            
                            KeyValuePairs.Add(key, value);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string s = string.Empty;
            s += "Method: " + Method + "\n";
            s += "URL: " + URL + "\n";
            s += "RawURL: " + RawURL + "\n";
            s += "Host: " + Host + "\n";
            s += "User-Agent: " + UserAgent + "\n";
            s += "Accept: " + Accept + "\n";
            s += "Accept-Language: " + AcceptLanguage + "\n";
            s += "Accept-Encoding: " + AcceptEncoding + "\n";
            s += "DNT: " + DNT + "\n";
            s += "Connection: " + Connection + "\n";
            s += "Referer: " + Referer + "\n";
            s += "Sec-Fetch-Dest: " + SecFetchDest + "\n";
            s += "Sec-Fetch-Mode: " + SecFetchMode + "\n";
            s += "Sec-Fetch-Site: " + SecFetchSite + "\n";
            s += "Cache-Control: " + CacheControl + "\n";
            s += "Upgrade-Insecure-Requests: " + UpgradeInsecureRequests + "\n";
            s += "Content-Type: " + ContentType.type + "; " + ContentType.charSet + "\n";
            s += "Content-Length: " + ContentLength;
            return s;
        }

        public static bool TryParse(string request, out HttpRequest httpRequest)
        {
            httpRequest = null;

            var lines = request.Split('\n');

            if(IsNullOrEmpty(lines))
                return false;

            var methodComponents = lines[0].Split(' ');

            if(IsNullOrEmpty(methodComponents))
                return false;

            if(!IsOfMinimumSize(methodComponents, 2))
                return false;

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<string> cookies = new List<string>();

            string method = methodComponents[0].Trim();
            string url = string.Empty;
            string protocol = string.Empty;
            methodComponents[1] = methodComponents[1].Trim();

            url = methodComponents[1];
            protocol = methodComponents[2];

            dictionary.Add("Method", method);
            dictionary.Add("URL", url);

            for (int i = 1; i < lines.Length; i++)
            {
                var lineComponents = lines[i].Split(':');

                if(IsNullOrEmpty(lineComponents))
                    continue;

                if(!IsOfMinimumSize(lineComponents, 2))
                    continue;

                string key = lineComponents[0].Trim();

                string value = string.Empty;

                lineComponents = lines[i].Split(' ');

                lineComponents[1] = lineComponents[1].TrimStart();

                for (int j = 1; j < lineComponents.Length; j++)
                {
                    if(j < lineComponents.Length -1)
                        value += lineComponents[j] + " ";
                    else
                        value += lineComponents[j];
                }

                dictionary.TryAdd(key, value);

                if(key.ToLower() == "cookie")
                {
                    cookies.Add(value);
                }
            }

            httpRequest = new HttpRequest();
            httpRequest.Method = GetRequestMethod(GetValue(dictionary, "Method"));
            httpRequest.URL = GetValue(dictionary, "URL");
            httpRequest.RawURL = GetValue(dictionary, "URL");
            httpRequest.Host = GetValue(dictionary, "Host");
            httpRequest.UserAgent = GetValue(dictionary, "User-Agent");
            httpRequest.Accept = GetValue(dictionary, "Accept");
            httpRequest.AcceptLanguage = GetValue(dictionary, "Accept-Language");
            httpRequest.AcceptEncoding = GetValue(dictionary, "Accept-Encoding");
            httpRequest.DNT = GetValue(dictionary, "DNT");
            httpRequest.Connection = GetValue(dictionary, "Connection");
            httpRequest.Referer = GetValue(dictionary, "Referer");
            httpRequest.SecFetchDest = GetValue(dictionary, "Sec-Fetch-Dest");
            httpRequest.SecFetchMode = GetValue(dictionary, "Sec-Fetch-Mode");
            httpRequest.SecFetchSite = GetValue(dictionary, "Sec-Fetch-Site");
            httpRequest.CacheControl = GetValue(dictionary, "Cache-Control");
            httpRequest.UpgradeInsecureRequests = GetValue(dictionary, "Upgrade-Insecure-Requests");
            httpRequest.ContentType = GetContentType(GetValue(dictionary, "Content-Type"));
            httpRequest.ContentLength = GetContentLength(GetValue(dictionary, "Content-Length"));

            httpRequest.KeyValuePairs = new Dictionary<string, string>();
            httpRequest.GetKeyValuePairs();

            httpRequest.Cookies = new Dictionary<string, string>();
            httpRequest.GetCookies(cookies);

            if(httpRequest.URL.Contains('?'))
                httpRequest.URL = httpRequest.URL.Split('?')[0];    

            return true;
        }

        private static bool IsNullOrEmpty<T>(T[] array)
        {
            if(array == null)
                return true;

            if(array.Length == 0)
                return true;

            return false;
        }

        private static bool IsOfMinimumSize<T>(T[] array, int minimumSize)
        {
            return array.Length >= minimumSize;
        }        

        private static string GetValue(Dictionary<string,string> dictionary, string key)
        {
            if(dictionary.TryGetValue(key, out string value))
            {
                return value;
            }

            return string.Empty;
        }

        private static HttpRequestMethod GetRequestMethod(string s)
        {
            s = s.ToLower();

            if(requestMethodTable.ContainsKey(s))
            {
                return requestMethodTable[s];
            }

            return HttpRequestMethod.Unknown;
        }

        private static ulong GetContentLength(string s)
        {
            if(ulong.TryParse(s, out ulong value))
            {
                return value;
            }

            return 0;
        }

        private static HttpContentType GetContentType(string s)
        {
            MediaType mediaType = MediaType.Unknown;

            if(s.Contains(';'))
            {
                var components = s.Split(';');

                if (components != null)
                {
                    if (components.Length > 1)
                    {
                        mediaType = HttpContentType.GetMediaTypeFromString(components[0].Trim());
                        var charSet = HttpContentType.GetCharSetFromString(components[1].Trim());
                        return new HttpContentType(mediaType, charSet);
                    }
                }
            }

            mediaType = HttpContentType.GetMediaTypeFromString(s.Trim());
            return new HttpContentType(mediaType);            
        }

        private void GetCookies(List<string> cookies)
        {
            if(cookies.Count == 0)
                return;

            for (int i = 0; i < cookies.Count; i++)
            {
                //Multiple cookie key/value pairs
                if (cookies[i].Contains(';'))
                {
                    var keyValuePairs = cookies[i].Split(';');
                    for (int j = 0; j < keyValuePairs.Length; j++)
                    {
                        var components = keyValuePairs[j].Split('=');

                        if(components.Length == 2)
                        {
                            string key = components[0].Trim();
                            string value = components[1].Trim();
                            Cookies.TryAdd(key, value);
                        }
                    }
                }
                else
                {
                    var components = cookies[i].Split('=');

                    if(components.Length == 2)
                    {
                        string key = components[0].Trim();
                        string value = components[1].Trim();
                        Cookies.TryAdd(key, value);
                    }
                }
            }
        }
    }
}