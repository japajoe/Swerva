using System;
using System.IO;
using System.Text.Json;

namespace Swerva
{
    public sealed class HttpConfig
    {
        public string Name { get; set; }
        public string PublicHtml { get; set; }
        public string PrivateHtml { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
        public ushort Port { get; set; }
        public ushort SslPort { get; set; }
        public bool UseHttps { get; set; }
        public bool UseHttpsForwarding { get; set; }
        public int MaxHeaderSize { get; set; }

        public static HttpConfig LoadFromFile(string filepath)
        {
            if(!File.Exists(filepath))
                return null;

            string json = File.ReadAllText(filepath);

            var settings = new HttpConfig();
            settings.Deserialize(json);
            return settings;
        }

        public static bool SaveToFile(HttpConfig config, string filePath)
        {
            string json = config.Serialize();
            
            try
            {
                File.WriteAllText(json, filePath);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void Deserialize(string json)
        {
            var obj = JsonSerializer.Deserialize<HttpConfig>(json);
            this.Name = obj.Name;
            this.PublicHtml = obj.PublicHtml;
            this.PrivateHtml = obj.PrivateHtml;
            this.CertificatePath = obj.CertificatePath;
            this.CertificatePassword = obj.CertificatePassword;
            this.Port = obj.Port;
            this.SslPort = obj.SslPort;
            this.UseHttps = obj.UseHttps;
            this.UseHttpsForwarding = obj.UseHttpsForwarding;
            this.MaxHeaderSize = obj.MaxHeaderSize;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize<HttpConfig>(this);
        }
    }
}