using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;
using System;

public class JobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobFactory(IServiceProvider serviceProvider) { _serviceProvider = serviceProvider; }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        try
        {
            IJob? job = _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            return job == null ? throw new Exception("Couldn't retrieve the required service") : job;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Job creation failed: {ex}");
            throw;
        }
    }

    public void ReturnJob(IJob job) { (job as IDisposable)?.Dispose(); }
}