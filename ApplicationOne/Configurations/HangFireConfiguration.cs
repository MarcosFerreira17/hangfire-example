using Hangfire;

namespace ApplicationOne.Configurations;
public static class HangFireConfiguration
{
    public static void AddHangFireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration["ConnectionStrings:HangFireDb"];

        services.AddHangfire(options =>
        {
            options.UseRecommendedSerializerSettings();
            options.UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer();
    }

    public static void UseHangFireConfig(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard();
    }
}
