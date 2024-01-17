using BHYT_BE.Internal.Adapter;
using Stripe;

namespace BHYT_BE.Common.AppSetting
{
    public class AppSettings
    {
        private readonly WebApplicationBuilder _builder;

        public AppSettings(WebApplicationBuilder builder)
        {
            _builder = builder;
            Logging = _builder.Configuration.GetSection("Logging").Get<Logging>() ?? new Logging();
            AllowedHosts = _builder.Configuration.GetSection("AllowedHosts").Get<List<string>>() ?? new List<string>();
            ConnectionStrings = _builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>() ?? new ConnectionStrings();
            EmailSettings = _builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            Jwt = _builder.Configuration.GetSection("Jwt").Get<Jwt>() ?? new Jwt();
            SystemEmail = _builder.Configuration.GetValue<string>("SystemEmail") ?? "";
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            _builder.Services.Configure<Logging>(_builder.Configuration.GetSection("Logging"));
            _builder.Services.Configure<List<string>>(_builder.Configuration.GetSection("AllowedHosts"));
            _builder.Services.Configure<ConnectionStrings>(_builder.Configuration.GetSection("ConnectionStrings"));
            _builder.Services.Configure<EmailSettings>(_builder.Configuration.GetSection("EmailSettings"));
            _builder.Services.Configure<Jwt>(_builder.Configuration.GetSection("Jwt"));
            _builder.Services.Configure<string>(_builder.Configuration.GetSection("SystemEmail"));
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
