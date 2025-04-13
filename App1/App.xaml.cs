using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml;
using App1.Services;
using App1.Repositories;
using Quartz;
using Quartz.Impl;
using Microsoft.Extensions.Hosting;
using App1.Views;
using App1.AutoChecker;

namespace App1
{
    public partial class App : Application
    {
        public static IHost Host { get; private set; }
        public static Window MainWindow { get; set; }

        public App()
        {
            this.InitializeComponent();
            ConfigureHost();
        }

        private void ConfigureHost()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configuration
                    var config = new ConfigurationBuilder()
                        .AddUserSecrets<App>()
                        .AddEnvironmentVariables()
                        .Build();
                    services.AddSingleton<IConfiguration>(config);

                    string connectionString = "Server=DESKTOP-KEB351O\\SQLEXPRESS;Database=DrinksImdb;Integrated Security=True;TrustServerCertificate=True;";

                    services.AddSingleton<IUserRepository, UserRepo>();
                    services.AddSingleton<IReviewsRepository, ReviewsRepository>();
                    services.AddSingleton<IOffensiveWordsRepository>(provider =>
                    {
                        return new OffensiveWordsRepository(connectionString);
                    });
                    services.AddSingleton<IAutoCheck, AutoCheck>();
                    services.AddSingleton<ICheckersService, CheckersService>();
                    services.AddSingleton<IUpgradeRequestsRepository, UpgradeRequestsRepository>(provider => new UpgradeRequestsRepository(connectionString));
                    services.AddSingleton<IRolesRepository, RolesRepository>();
                    services.AddSingleton<IUserService, UserService>(); 
                    services.AddSingleton<IReviewService, ReviewsService>(); 
                    services.AddSingleton<IUpgradeRequestsService, UpgradeRequestsService>();
                    services.AddTransient<EmailJob>();

                    // Quartz Configuration
                    services.AddSingleton<JobFactory>();
                    services.AddSingleton(provider =>
                    {
                        var factory = new StdSchedulerFactory();
                        var scheduler = factory.GetScheduler().Result;
                        scheduler.JobFactory = provider.GetRequiredService<JobFactory>();
                        return scheduler;
                    });

                    // Jobs
                    services.AddTransient<EmailJob>();
                    services.AddTransient<MainPage>();
                    services.AddTransient<MainWindow>();

                })
                .Build();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var scheduler = Host.Services.GetRequiredService<IScheduler>();
            scheduler.Start().Wait(); 

            MainWindow = Host.Services.GetRequiredService<MainWindow>();
            MainWindow.Activate();

            // Prevent app suspension
            Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().Activated += (s, e) =>
            {
                MainWindow?.Activate();
            };
        }
    }
}
