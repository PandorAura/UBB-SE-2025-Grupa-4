using App1.Ai_Check;
using App1.Models;
using App1.Services;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.UI.Text;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {

        private IReviewService reviewsService;
        private IUserService userService;
        private CheckersService checkersService;
        private IUpgradeRequestsService requestsService;

        //TO DO: Add interface for requests, pass to main Page, same as the others

        public MainPage(IReviewService reviewsService,
                   IUserService userService, IUpgradeRequestsService upgradeRequestsService
                   )
        {
            this.InitializeComponent();

            if ( reviewsService == null ) {
                throw new ArgumentNullException(nameof(reviewsService));
            }
            if (userService == null)
            {
                throw new ArgumentNullException(nameof(userService));
            }
            this.reviewsService = reviewsService;
            this.userService = userService;
            this.requestsService = upgradeRequestsService;
            checkersService = new CheckersService(reviewsService);

            //reviewsService = new ReviewsService();
            //userService = new UserService();
            LoadStatistics();
            displayReviews();
            displayAppeal();
            displayRoleRequests();

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

                selectedUser.PermissionID = 0; // Assuming 0 is the permission ID for banned users


                TextBlock userInfo = new TextBlock
                {
                    Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\nStatus: Banned",
                    FontSize = 18
                };

                // List of reviews for this user
                List<Review> userReviews = reviewsService.GetReviewsByUser(selectedUser.UserId);

                TextBlock reviewsHeader = new TextBlock
                {
                    Text = "User Reviews:",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };

                ListView reviewsList = new ListView
                {
                    ItemsSource = userReviews.Select(r => $"{r.Content}").ToList(),
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
                    selectedUser.PermissionID = 0;
                    userInfo.Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\nStatus: Banned";
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
                    selectedUser.PermissionID = 1;
                    userInfo.Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\nStatus: Active";
                };

                // Close Button
                Button closeButton = new Button
                {
                    Content = "Close Appeal Case",
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                closeButton.Click += (s, args) =>
                {
                    selectedUser.HasAppealed = false;
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

                LoadStatistics();
            }
        }
        private void RequestList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is UpgradeRequest selectedRequest)
            {
                Flyout flyout = new Flyout();
                StackPanel panel = new StackPanel { Padding = new Thickness(10) };
                int userID = selectedRequest.RequestingUserId;
                User selectedUser = userService.GetUserBasedOnID(userID);
                int currentRoleID = userService.GetHighestRoleBasedOnUserID(selectedUser.UserId);
                string currentRoleName = requestsService.GetRoleNameBasedOnID(currentRoleID);
                string requiredRoleName = requestsService.GetRoleNameBasedOnID(currentRoleID + 1);
                TextBlock userInfo = new TextBlock
                {
                    Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\n{currentRoleName} -> {requiredRoleName}",
                    FontSize = 18
                };

                List<Review> userReviews = reviewsService.GetReviewsByUser(selectedUser.UserId);

                TextBlock reviewsHeader = new TextBlock
                {
                    Text = "User Reviews:",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };

                ListView reviewsList = new ListView
                {
                    ItemsSource = userReviews.Select(r => $"{r.Content}\nFlags: {r.NumberOfFlags}").ToList(),
                    Height = 100
                };



                // Add items to panel
                panel.Children.Add(userInfo);
                panel.Children.Add(reviewsHeader);
                panel.Children.Add(reviewsList);


                flyout.Content = panel;
                flyout.Placement = FlyoutPlacementMode.Left;
                flyout.ShowAt((FrameworkElement)sender);
            }
        }



        private void displayRoleRequests()
        {
            List<UpgradeRequest> upgradeRequests = requestsService.GetAllRequests();
            ObservableCollection<UpgradeRequest> UsersRoleRequests = new ObservableCollection<UpgradeRequest>(upgradeRequests);

            RequestsList.ItemsSource = UsersRoleRequests;

        }
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is int RequestId)
                {
                    this.requestsService.HandleRequest(true, RequestId);

                }
            }
            this.displayRoleRequests();
        }
        private void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag is int RequestId)
                {
                    this.requestsService.HandleRequest(false, RequestId);
                }
            }
            this.displayRoleRequests();
        }

        private void LoadPieChart()
        {
            var usersCount = userService.GetActiveUsers(1).Count;
            var adminsCount = userService.GetActiveUsers(2).Count;
            var bannedCount = userService.GetBannedUsers().Count;
            AllUsersPieChart.Series = new List<PieSeries<double>> 
            {
                new PieSeries<double> { Values = new double[] { bannedCount }, Name = "Banned" },
                new PieSeries<double> { Values = new double[] { usersCount }, Name = "Users" },
                new PieSeries<double> { Values = new double[] { adminsCount }, Name = "Admins" }
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
                AllReviews.Where(review => review.Content.ToLower().Contains(filter))
            );
        }

        private void BannedUserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        { 
            //TO DO: GET APPEALING USERS <3
            
            string filter = BannedUserSearchTextBox.Text.ToLower();
            AppealsList.ItemsSource = new ObservableCollection<User>(
                userService.GetAppealingUsers()
            );
        }

        private void MenuFlyoutAllowReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                reviewsService.resetReviewFlags(review.ReviewID);
            }
            displayReviews();

        }

        private void MenuFlyoutHideReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                reviewsService.HideReview(review.UserID);
                reviewsService.resetReviewFlags(review.ReviewID); //Reviews are displayed if they have at least one flag
            }
            displayReviews();
        }

        private void MenuFlyoutAICheck_Click_2(object sender, RoutedEventArgs e)
        {
            //checkersService.RunAICheck
        }


        private void Button_AutoCheck_Click(object sender, RoutedEventArgs e)
        {
            List<Review> reviews = reviewsService.GetFlaggedReviews();

            List<string> messages = checkersService.RunAutoCheck(reviews);

            displayReviews();
        }
    }

}
