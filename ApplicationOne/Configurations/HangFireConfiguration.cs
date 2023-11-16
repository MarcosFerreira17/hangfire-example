using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApplicationOne.Filters;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationOne.Configurations;
public static class HangFireConfiguration
{
    public static void AddHangFireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration["ConnectionStrings:HangFireDb"];

        services.AddHangfire(options =>
        {
            options.UseSimpleAssemblyNameTypeSerializer();
            options.UseRecommendedSerializerSettings();
            options.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                SchemaName = "ApplicationOneHF"
            });
        });

        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
        {
            Attempts = 0,
            OnAttemptsExceeded = AttemptsExceededAction.Delete
        });

        services.AddHangfireServer();
    }

    public static void UseHangFireConfig(this IApplicationBuilder app)
    {
        if (app.ApplicationServices.GetService<IWebHostEnvironment>().IsDevelopment())
            app.UseHangfireDashboard("/hangfire");
        else
        {
            var configuration = app.ApplicationServices.GetService<IConfiguration>();

            var _logger = app.ApplicationServices.GetService<ILogger<HangfireDashboardJwtAuthorizationFilter>>();

            var options = new DashboardOptions
            {
                Authorization = new IDashboardAuthorizationFilter[]
                {
                    new HangfireDashboardJwtAuthorizationFilter(new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false,
                        SignatureValidator = (access_token, parameters) => new JwtSecurityToken(access_token),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                    }, _logger)
                }
            };

            app.UseHangfireDashboard("/hangfire", options);
        }
    }
}

