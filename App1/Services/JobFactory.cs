using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;
using System;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        System.Diagnostics.Debug.WriteLine("JobFactory created");
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Creating job of type: {bundle.JobDetail.JobType.Name}");
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Job creation failed: {ex}");
            throw;
        }
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}