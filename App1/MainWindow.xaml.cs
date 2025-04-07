using Microsoft.UI.Xaml;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System;
using App1.Views;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class MainWindow : Window
    {
        private IScheduler _scheduler;

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeScheduler().ConfigureAwait(false);
            ScheduleDelayedEmailAutomatically().ConfigureAwait(false);
            this.Activated += OnWindowActivated;
        }

        private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                rootFrame.Content = App.Host.Services.GetRequiredService<MainPage>();

                this.Activated -= OnWindowActivated;
            }
        }

        private async Task InitializeScheduler()
        {
            try
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                _scheduler = await factory.GetScheduler();
                await _scheduler.Start();
                System.Diagnostics.Debug.WriteLine("Scheduler initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Scheduler initialization failed: {ex}");
            }
        }

        private async Task ScheduleDelayedEmailAutomatically()
        {
            try
            {
                IJobDetail job = JobBuilder.Create<EmailJob>()
                    .WithIdentity("autoEmailJob", "emailGroup")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("autoTrigger", "emailGroup")
                    .StartNow()
                    .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 8, 0))
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);
                System.Diagnostics.Debug.WriteLine($"Job scheduled to run every 1 minute");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Job scheduling failed: {ex}");
            }
        }
    }
    
}