using App1.Ai_Check;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace App1
{
    public sealed partial class MainWindow : Window
    {
        private IScheduler _scheduler;

        public MainWindow()
        {
            this.InitializeComponent();
            InitializeScheduler().ConfigureAwait(false); // Start scheduler
            ScheduleDelayedEmailAutomatically().ConfigureAwait(false); // Auto-schedule email
        }


            private async Task InitializeScheduler()
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                _scheduler = await factory.GetScheduler();
                await _scheduler.Start();
            }

            // Schedule email without button click
            private async Task ScheduleDelayedEmailAutomatically()
            {
                var jobData = new JobDataMap
        {
            { "RecipientEmail", "aurapandor@gmail.com" },
            { "Subject", "Auto-Scheduled Email" },
            { "Body", "This email was scheduled automatically when the app started!" }
        };

                IJobDetail job = JobBuilder.Create<EmailJob>()
                    .WithIdentity("autoEmailJob", "emailGroup")
                    .UsingJobData(jobData)
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("autoTrigger", "emailGroup")
                    .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Minute)) // Send after 1 minute
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);

                // Optional: Show a notification (if UI is ready)
                DispatcherQueue.TryEnqueue(() =>
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Email Scheduled",
                        Content = "An email will be sent in 1 minute.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                   // _ = dialog.ShowAsync();
                });
            }
        



        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }

        

        private void TrainModel_Click(object sender, RoutedEventArgs e)
        {
            ReviewModelTrainer.TrainModel();
            ContentDialog dialog = new ContentDialog
            {
                Title = "Training Complete",
                Content = "The model has been trained and saved.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}