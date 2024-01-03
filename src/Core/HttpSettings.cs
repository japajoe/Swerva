namespace Swerva
{
    public static class HttpSettings
    {
        public static string Name { get; private set; }
        public static string PublicHtml { get; private set; }
        public static string PrivateHtml { get; private set; }
        public static string CertificatePath { get; private set; }
        public static string CertificatePassword { get; private set; }
        public static ushort Port { get; private set; }
        public static ushort SslPort { get; private set; }
        public static bool UseHttps { get; private set; }
        public static bool UseHttpsForwarding { get; private set; }
        public static int MaxHeaderSize { get; private set; }

        public static void LoadFromConfig(HttpConfig config)
        {
            Name = config.Name;
            PublicHtml = config.PublicHtml;
            PrivateHtml = config.PrivateHtml;
            CertificatePath = config.CertificatePath;
            CertificatePassword = config.CertificatePassword;
            Port = config.Port;
            SslPort = config.SslPort;
            UseHttps = config.UseHttps;
            UseHttpsForwarding = config.UseHttpsForwarding;
            MaxHeaderSize = config.MaxHeaderSize;
        }
    }
}