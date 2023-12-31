using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationOne.Controllers;

[Route("[controller]")]
[ApiController]
public class HangFireJobsController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangFireJobsController(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }
    [HttpGet]
    [Route("FireForgetJobs")]
    public string FireForget()
    {
        //Fire-and-forget jobs are executed only once and almost immediately after creation.
        var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Fire And Forget Job"));
        return $"Job ID: {jobId}. Fire And Forget jobs Executed!";
    }

    [HttpGet]
    [Route("DelayedJobs")]
    public string Delayed()
    {
        //Delayed jobs are executed only once too, but not immediately, after a certain time interval.
        var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Delayed Job Executed!"), TimeSpan.FromSeconds(10));
        return $"Job ID: {jobId}.Delayed Jobs Executed!";
    }

    [HttpGet]
    [Route("RecurringJobs")]
    public string Recurring()
    {
        //Recurring jobs fire many times on the specified CRON schedule.
        RecurringJob.AddOrUpdate("Recurring Job Run Every Day", () => Console.WriteLine("Recurring Job Executed"), Cron.Daily);
        return "Recurring Job Executed!";
    }

    [HttpGet]
    [Route("ContinuationsJobs")]
    public async Task<string> Continuations()
    {
        //Parent Job
        var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Continuations Job Parent is Being Executed!"));
        Console.WriteLine("Delayed For 1 Minute");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine("1 Minute Has Been Completed Child Job Executed");
        BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuations Job Child is Being Executed!"));
        return "Continuations Jobs Executed!";
    }
}
