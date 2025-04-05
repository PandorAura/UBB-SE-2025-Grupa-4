using Microsoft.UI.Xaml;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System;
using App1.Views;
using Microsoft.Extensions.DependencyInjection;

using App1.Models;
using System.Collections.ObjectModel;
using App1.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.WinUI;
using LiveChartsCore.SkiaSharpView;
using App1.Repositories;
using HarfBuzzSharp;
//using CSharpMarkup.WinUI;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    public sealed partial class MainWindow : Window
    {
        private IScheduler _scheduler;

        //private UserService userService;
        //private CheckersService checkersService;

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



        


        //private void displayReviews()
        //{
        //    ObservableCollection<Review> Reviews = new ObservableCollection<Review>(reviewsService.GetFlaggedReviews());

        //    ReviewsList.ItemsSource = Reviews;
        //}

        //private void displayAppeal()
        //{
        //    ObservableCollection<User> UsersWhichAppealed = new ObservableCollection<User>  // getBannedUsers() from UserService
        //    {
        //        new User(),
        //        //new User(22),
        //        //new User(),
        //        //new User(2),
        //        //new User(),
        //        //new User(12),
        //        //new User(),
        //        //new User(4),
        //        //new User(6),
        //        //new User(),
        //        //new User(79)
        //    };

        //    AppealsList.ItemsSource = UsersWhichAppealed;
        //}

        //private void displayRoleRequests()
        //{
        //    ObservableCollection<User> UsersRoleRequests = new ObservableCollection<User>
        //    {
                //new User(),
                //new User(22),
                //new User(),
                //new User(2),
                //new User(),
                //new User(12),
                //new User(),
                //new User(4),
                //new User(6),
                //new User(),
                //new User(79)
            };

        //    RequestsList.ItemsSource = UsersRoleRequests;
        //}

        //private void LoadPieChart()
        //{
        //    AllUsersPieChart.Series = new List<PieSeries<double>>  // get all users and
        //    // group them by permission? manager, user, admin, etc?
        //    {
        //        new PieSeries<double> { Values = new double[] { 40 }, Name = "Managers" },
        //        new PieSeries<double> { Values = new double[] { 25 }, Name = "Users" },
        //        new PieSeries<double> { Values = new double[] { 35 }, Name = "Admins" }
        //    };
        //}

        //private void LoadStatistics()
        //{
        //    LoadPieChart();
        //    LoadBarChart();
        //}

        //private void LoadBarChart()
        //{
        //    TotalDataBarChart.Series = new List<ISeries>  // get all data from the tables
        //    // all users, all reviews, drinks?? de unde
        //    {
        //        new ColumnSeries<double>
        //        {
        //            Values = new double[] { 10, 20, 30 }, // Your data points
        //        }
        //    };

        //    TotalDataBarChart.XAxes = new List<Axis>
        //    {
        //        new Axis { Labels = new List<string> { "Users", "Drinks", "Reviews" } }  // X-axis labels
        //    };

        //    TotalDataBarChart.YAxes = new List<Axis> { new Axis { Name = "Total", MinLimit = 0 } };
        //}

        //private void ReviewSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    ObservableCollection<Review> AllReviews = new ObservableCollection<Review>(reviewsService.GetFlaggedReviews());
        //    string filter = ReviewSearchTextBox.Text.ToLower();
        //    ReviewsList.ItemsSource = new ObservableCollection<Review>(
        //        AllReviews.Where(review => review.content.ToLower().Contains(filter))
        //    );
        //}

        //private void BannedUserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    ObservableCollection<User> AllAppeals = new ObservableCollection<User>
        //    {
        //        new User(),
        //        new User(22),
        //        new User(),
        //        new User(2),
        //        new User(),
        //        new User(12),
        //        new User(),
        //        new User(4),
        //        new User(6),
        //        new User(),
        //        new User(79)
        //    };
        //    string filter = BannedUserSearchTextBox.Text.ToLower();
        //    AppealsList.ItemsSource = new ObservableCollection<User>(
        //        AllAppeals.Where(user => user.email.ToLower().Contains(filter))
        //    );
        //}

        //private void MenuFlyoutAllowReview_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
        //    {
        //        reviewsService.resetReviewFlags(review.userID);
        //    }
        //    displayReviews();

        //private void ReviewSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    ObservableCollection<Review> AllReviews = new ObservableCollection<Review>  // getReviews() from ReviewsService
        //    {
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review(),
        //        new Review()
        //    };
        //    string filter = ReviewSearchTextBox.Text.ToLower();
        //    ReviewsList.ItemsSource = new ObservableCollection<Review>(
        //        AllReviews.Where(review => review.Content.ToLower().Contains(filter))
        //    );
        //}

        //private void BannedUserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    ObservableCollection<User> AllAppeals = new ObservableCollection<User>
        //    {
        //        new User(),
        //        //new User(22),
        //        //new User(),
        //        //new User(2),
        //        //new User(),
        //        //new User(12),
        //        //new User(),
        //        //new User(4),
        //        //new User(6),
        //        //new User(),
        //        //new User(79)
        //    };
        //    string filter = BannedUserSearchTextBox.Text.ToLower();
        //    AppealsList.ItemsSource = new ObservableCollection<User>(
        //        AllAppeals.Where(user => user.Email.ToLower().Contains(filter))
        //    );
        //}

        ////private void MenuFlyoutAICheck_Click_2(object sender, RoutedEventArgs e)
        ////{
        ////    //TODO
        //}
    
}