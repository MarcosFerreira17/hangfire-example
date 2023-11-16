
using ApplicationOne.Services;
using Hangfire;

namespace ApplicationOne.Jobs;
public class HelloWorldWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    public HelloWorldWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var _service = scope.ServiceProvider.GetRequiredService<IHelloWorldService>();

        RecurringJob.AddOrUpdate("Recurring Job Run Every Day", () => _service.HelloWorldAsync(), Cron.Daily);

        return Task.CompletedTask;
    }
}
