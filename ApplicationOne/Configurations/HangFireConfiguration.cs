using Hangfire;

namespace ApplicationOne.Configurations;
public static class HangFireConfiguration
{
    public static void AddHangFireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(options =>
        {
            options.UseColouredConsoleLogProvider();
            options.UseSqlServerStorage(
                        configuration.GetConnectionString("HangFireDb"));
        });
        services.AddHangfireServer();
    }

    public static void UseHangFireConfig(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard();
    }
}
