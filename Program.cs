using BHYT_BE.Common.AppSetting;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repositories.Data;
using BHYT_BE.Internal.Repositories.UserRepo;
using BHYT_BE.Internal.Repository.Data;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using BHYT_BE.Internal.Services.InsuranceService;
using BHYT_BE.Internal.Services.MapperService;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Stripe;
using System.Security.Claims;
using System.Text;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

logger.Info("init main");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddOptions();
    AppSettings appSettings = new AppSettings();
    builder.Configuration.Bind(appSettings);
    builder.Services.AddAutoMapper(typeof(UserMapperService));
    // Add services to the container.
    builder.Services.AddSingleton<AppSettings>(_ => appSettings);
    builder.Services.AddTransient<IEmailAdapter, EmailAdapter>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "react",
                          builder =>
                          {
                              builder.WithOrigins("http://localhost:3000")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader(); ;
                          });
    });
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    
    builder.Services.AddMemoryCache();
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
    // Context
    builder.Services.AddDbContext<InsuranceDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    builder.Services.AddDbContext<InsuranceHistoryDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    builder.Services.AddDbContext<InsurancePaymentHistoryDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    builder.Services.AddDbContext<InsurancePaymentHistoryDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    builder.Services.AddDbContext<InsuranceRequestDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    builder.Services.AddDbContext<InsuranceRequestPaymentDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    builder.Services.AddDbContext<UserDBContext>(options => options.UseNpgsql(appSettings.ConnectionStrings.DBConnection));
    // Init service and repo
    builder.Services.AddScoped<IInsurancePaymenHistoryRepository, InsurancePaymenHistoryRepository>();
    builder.Services.AddScoped<IInsuranceHistoryRepository, InsuranceHistoryRepository>();
    builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
    builder.Services.AddScoped<IInsuranceService, InsuranceService>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    builder.Services.AddIdentity<User, IdentityRole>()
     .AddEntityFrameworkStores<UserDBContext>()
     .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = appSettings.Jwt.Issuer,
           ValidAudience = appSettings.Jwt.Audience,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Secret))
       });

    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Thiết lập về Password
        options.Password.RequireDigit = false; // Không bắt phải có số
        options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
        options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
        options.Password.RequireUppercase = false; // Không bắt buộc chữ in
        options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
        options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

        // Cấu hình Lockout - khóa user
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
        options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
        options.Lockout.AllowedForNewUsers = true;

        // Cấu hình về User.
        options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;  // Email là duy nhất

        // Cấu hình đăng nhập.
        options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
        options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    });

    builder.Services.AddScoped<ITokenRepository, JWTRepository>();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var insuranceDbContext = services.GetRequiredService<InsuranceDBContext>();
            insuranceDbContext.Database.Migrate();

            var insuranceHistoryDbContext = services.GetRequiredService<InsuranceHistoryDBContext>();
            insuranceHistoryDbContext.Database.Migrate();

            var insurancePaymentHistoryDbContext = services.GetRequiredService<InsurancePaymentHistoryDBContext>();
            insurancePaymentHistoryDbContext.Database.Migrate();

            var insuranceRequestDbContext = services.GetRequiredService<InsuranceRequestDBContext>();
            insuranceRequestDbContext.Database.Migrate();

            var insuranceRequestPaymentDbContext = services.GetRequiredService<InsuranceRequestPaymentDBContext>();
            insuranceRequestPaymentDbContext.Database.Migrate();

            var userDbContext = services.GetRequiredService<UserDBContext>();
            userDbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while applying migrations.");
            throw;
        }
    }
    StripeConfiguration.ApiKey = "sk_test_51OQ6efGzNiwrigil7GWec9IUCQb1kma855hkTTx7g6XuYKY8H6gMvuRuIpq2uLgeYwov0AiI7BQsIqsPBY9Ahhve00HzncCvSr";
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseCors("react");
    app.UseHttpsRedirection();

    app.UseAuthorization();
    app.UseAuthentication();
    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}