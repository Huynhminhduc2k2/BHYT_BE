namespace BHYT_BE.Common.AppSetting
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            Logging = new Logging();
            AllowedHosts = new List<string> { };
            ConnectionStrings = new ConnectionStrings();
            EmailSettings = new EmailSettings();
            Jwt = new Jwt();
            SystemEmail = "";
            configuration.GetSection("Logging").Bind(Logging);
            configuration.GetSection("AllowedHosts").Bind(AllowedHosts);
            configuration.GetSection("ConnectionStrings").Bind(ConnectionStrings);
            configuration.GetSection("EmailSettings").Bind(EmailSettings);
            configuration.GetSection("Jwt").Bind(Jwt);
            configuration.GetSection("SystemEmail").Bind(SystemEmail);
        }
        public Logging Logging { get; set; }
        public List<string> AllowedHosts { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public EmailSettings EmailSettings { get; set; }
        public Jwt Jwt { get; set; }
        public string SystemEmail { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
        public Console Console { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class Console
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
    }

    public class ConnectionStrings
    {
        public string DBConnection { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class Jwt
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireDays { get; set; }
    }
}
