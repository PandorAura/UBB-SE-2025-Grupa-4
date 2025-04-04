using App1.Ai_Check;
using App1.Models;
using App1.Services;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {

        private ReviewsService reviewsService;
        private UserService userService;
        private CheckersService checkersService;

        public MainPage()
        {
            this.InitializeComponent();
            reviewsService = new ReviewsService();
            userService = new UserService();
            LoadStatistics();
            displayReviews();
            displayAppeal();
            displayRoleRequests();

        }
        private void TrainModel_Click(object sender, RoutedEventArgs e)
        {
            ReviewModelTrainer.TrainModel();
            ContentDialog dialog = new ContentDialog
            {
                Title = "Training Complete",
                Content = "The model has been trained and saved.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // Set the XamlRoot property
            };
            _ = dialog.ShowAsync();
        }

        private void displayReviews()
        {
            ObservableCollection<Review> Reviews = new ObservableCollection<Review>(reviewsService.GetFlaggedReviews());

            ReviewsList.ItemsSource = Reviews;
        }

        private void displayAppeal()
        {
            ObservableCollection<User> UsersWhichAppealed = new ObservableCollection<User>(userService.GetAppealingUsers());

            AppealsList.ItemsSource = UsersWhichAppealed;
        }


        private void AppealsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is User selectedUser)
            {
                Flyout flyout = new Flyout();
                StackPanel panel = new StackPanel { Padding = new Thickness(10) };

                selectedUser.permissionID = 0; // Assuming 0 is the permission ID for banned users


                TextBlock userInfo = new TextBlock
                {
                    Text = $"User ID: {selectedUser.userId}\nEmail: {selectedUser.email}\nStatus: Banned",
                    FontSize = 18
                };

                // List of reviews for this user
                List<Review> userReviews = reviewsService.GetReviewsByUser(selectedUser.userId);

                TextBlock reviewsHeader = new TextBlock
                {
                    Text = "User Reviews:",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };

                ListView reviewsList = new ListView
                {
                    ItemsSource = userReviews.Select(r => $"{r.content}").ToList(),
                    MaxHeight = 200
                };

                // Ban Button
                Button banButton = new Button
                {
                    Content = "Keep Ban",
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                banButton.Click += (s, args) =>
                {
                    selectedUser.permissionID = 0;
                    userInfo.Text = $"User ID: {selectedUser.userId}\nEmail: {selectedUser.email}\nStatus: Banned";
                };

                // Appeal Button
                Button appealButton = new Button
                {
                    Content = "Accept Appeal",
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                appealButton.Click += (s, args) =>
                {
                    selectedUser.permissionID = 1;
                    userInfo.Text = $"User ID: {selectedUser.userId}\nEmail: {selectedUser.email}\nStatus: Active";
                };

                // Close Button
                Button closeButton = new Button
                {
                    Content = "Close Appeal Case",
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                closeButton.Click += (s, args) =>
                {
                    selectedUser.hasAppealed = false;
                    AppealsList.ItemsSource = null;
                    AppealsList.ItemsSource = userService.GetAppealingUsers();
                    flyout.Hide();
                };

                // Add items to panel
                panel.Children.Add(userInfo);
                panel.Children.Add(reviewsHeader);
                panel.Children.Add(reviewsList);
                panel.Children.Add(banButton);
                panel.Children.Add(appealButton);
                panel.Children.Add(closeButton);


                flyout.Content = panel;
                flyout.Placement = FlyoutPlacementMode.Left;
                flyout.ShowAt((FrameworkElement)sender);
            }
        }



        private void displayRoleRequests()
        {
            ObservableCollection<User> UsersRoleRequests = new ObservableCollection<User>(userService.GetAppealingUsers());

            RequestsList.ItemsSource = UsersRoleRequests;
        }

        private void LoadPieChart()
        {
            AllUsersPieChart.Series = new List<PieSeries<double>>  // get all users and
            // group them by permission? manager, user, admin, etc?
            {
                new PieSeries<double> { Values = new double[] { 40 }, Name = "Managers" },
                new PieSeries<double> { Values = new double[] { 25 }, Name = "Users" },
                new PieSeries<double> { Values = new double[] { 35 }, Name = "Admins" }
            };
        }

        private void LoadStatistics()
        {
            LoadPieChart();
            LoadBarChart();
        }

        private void LoadBarChart()
        {
            TotalDataBarChart.Series = new List<ISeries>  // get all data from the tables
            // all users, all reviews, drinks?? de unde
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 10, 20, 30 }, // Your data points
                }
            };

            TotalDataBarChart.XAxes = new List<Axis>
            {
                new Axis { Labels = new List<string> { "Users", "Drinks", "Reviews" } }  // X-axis labels
            };

            TotalDataBarChart.YAxes = new List<Axis> { new Axis { Name = "Total", MinLimit = 0 } };
        }

        private void ReviewSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObservableCollection<Review> AllReviews = new ObservableCollection<Review>(reviewsService.GetFlaggedReviews());
            string filter = ReviewSearchTextBox.Text.ToLower();
            ReviewsList.ItemsSource = new ObservableCollection<Review>(
                AllReviews.Where(review => review.content.ToLower().Contains(filter))
            );
        }

        private void BannedUserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObservableCollection<User> AllAppeals = new ObservableCollection<User>
            {
                new User(),
                new User(22),
                new User(),
                new User(2),
                new User(),
                new User(12),
                new User(),
                new User(4),
                new User(6),
                new User(),
                new User(79)
            };
            string filter = BannedUserSearchTextBox.Text.ToLower();
            AppealsList.ItemsSource = new ObservableCollection<User>(
                AllAppeals.Where(user => user.email.ToLower().Contains(filter))
            );
        }

        private void MenuFlyoutAllowReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                reviewsService.resetReviewFlags(review.userID);
            }
            displayReviews();

        }

        private void MenuFlyoutHideReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                reviewsService.HideReview(review.userID);
                reviewsService.resetReviewFlags(review.userID); //Reviews are displayed if they have at least one flag
            }
            displayReviews();
        }

        private void MenuFlyoutAICheck_Click_2(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }

}
