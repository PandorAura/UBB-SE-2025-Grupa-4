using Microsoft.UI.Xaml;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System;
using App1.Ai_Check;

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
                    .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Minute))
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);
                System.Diagnostics.Debug.WriteLine($"Job scheduled to run at {DateTime.Now.AddMinutes(1)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Job scheduling failed: {ex}");
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }

        private void TrainModel_Click(object sender, RoutedEventArgs e)
        {
            ReviewModelTrainer.TrainModel();
        }
    }
}