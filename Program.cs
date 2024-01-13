using BHYT_BE.Internal.Repositories.Data;
using BHYT_BE.Internal.Repositories.UserRepo;
using BHYT_BE.Internal.Repository.Data;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using BHYT_BE.Internal.Services.InsuranceService;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Stripe;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

logger.Debug("init main");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
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
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<InsuranceDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
    builder.Services.AddDbContext<InsuranceHistoryDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
    builder.Services.AddDbContext<UserDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));
    // Init service and repo
    builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
    builder.Services.AddScoped<IInsuranceService, InsuranceService>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
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
