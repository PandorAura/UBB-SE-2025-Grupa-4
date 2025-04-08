using App1.Models;
using App1.Services;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Text;
using System;
using System.Runtime.CompilerServices;
using App1.AutoChecker;

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
        private ICheckersService checkersService;
        private IUpgradeRequestsService requestsService;
        private IAutoCheck autoCheck;

        public MainPage(IReviewService reviewsService,
                   IUserService userService, IUpgradeRequestsService upgradeRequestsService, ICheckersService checkersService, IAutoCheck autoCheck
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
            this.checkersService = checkersService;
            this.autoCheck = autoCheck;
            // checkersService = new CheckersService(reviewsService);

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

                selectedUser.Roles.Add(new Role(0, "Banned"));

                TextBlock userInfo = new TextBlock
                {
                    Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\nStatus: Banned",
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
                    ItemsSource = userReviews.Select(r => $"{r.Content}").ToList(),
                    MaxHeight = 200
                };

                Button banButton = new Button
                {
                    Content = "Keep Ban",
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                banButton.Click += (s, args) =>
                {
                    selectedUser.Roles.Add(new Role(0, "Banned"));
                    userInfo.Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\nStatus: Banned";
                    LoadStatistics();
                };

                Button appealButton = new Button
                {
                    Content = "Accept Appeal",
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                appealButton.Click += (s, args) =>
                {
                    selectedUser.Roles.Add(new Role(1, "Banned"));
                    userInfo.Text = $"User ID: {selectedUser.UserId}\nEmail: {selectedUser.Email}\nStatus: Active";
                    LoadStatistics();
                };

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
                    //LoadStatistics();
                };

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
            LoadStatistics();
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
            LoadStatistics();
        }

        private void LoadPieChart()
        {
            int bannedCount, usersCount, adminsCount, managerCount;
            bannedCount = usersCount = adminsCount = managerCount = 0;

            List<User> users = userService.GetAllUsers();
            foreach (var user in users) { 
                var count = user.Roles.Count;
                switch (count) { 
                    case 0: bannedCount++; break;
                    case 1: usersCount++; break;
                    case 2: adminsCount++; break;
                    case 3: managerCount++; break;
                }
            }
            
            AllUsersPieChart.Series = new List<PieSeries<double>> 
            {
                new PieSeries<double> { Values = new double[] { bannedCount }, Name = "Banned" },
                new PieSeries<double> { Values = new double[] { usersCount }, Name = "Users" },
                new PieSeries<double> { Values = new double[] { adminsCount }, Name = "Admins" },
                new PieSeries<double> { Values = new double[] { managerCount }, Name = "Managers" }
            };
        }

        private void LoadStatistics()
        {
            LoadPieChart();
            LoadBarChart();
        }

        private void LoadBarChart()
        {
            //flagged reviews = pending, hidden reviews = rejected
            var rejectedCount = reviewsService.GetHiddenReviews().Count;
            var pendingCount = reviewsService.GetFlaggedReviews().Count;
            var totalCount = reviewsService.GetReviews().Count;
            TotalDataBarChart.Series = new List<ISeries>
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { rejectedCount, pendingCount, totalCount }, // Your data points
                }
            };

            TotalDataBarChart.XAxes = new List<Axis>
            {
                new Axis { Labels = new List<string> { "rejected", "pending", "total" } }  // X-axis labels
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
            LoadStatistics();
        }

        private void MenuFlyoutHideReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                reviewsService.HideReview(review.UserID);
                reviewsService.resetReviewFlags(review.ReviewID); //Reviews are displayed if they have at least one flag
            }
            displayReviews();
            LoadStatistics();
        }

        private void MenuFlyoutAICheck_Click_2(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                checkersService.RunAICheck(review);
            }
            displayReviews();
        }


        private void Button_AutoCheck_Click(object sender, RoutedEventArgs e)
        {
            List<Review> reviews = reviewsService.GetFlaggedReviews();

            List<string> messages = checkersService.RunAutoCheck(reviews); //put the messages in a logs file

            displayReviews();
            LoadStatistics();
        }

        private void Button_ModifyOffensiveWordsList_Click(object sender, RoutedEventArgs e)
        {
            WordsList.ItemsSource = checkersService.getOffensiveWordsList();
            WordListPopup.Visibility = Visibility.Visible;
        }

        private async void AddWord_Click(object sender, RoutedEventArgs e)
        {
            TextBox input = new TextBox { PlaceholderText = "Enter new word..." };

            ContentDialog dialog = new ContentDialog
            {
                Title = "Add New Word",
                Content = input,
                PrimaryButtonText = "Add Word",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                var newWord = input.Text.Trim();
                if (!string.IsNullOrWhiteSpace(newWord))
                {
                    checkersService.AddOffensiveWord(newWord);
                    WordsList.ItemsSource = null;
                    WordsList.ItemsSource = checkersService.getOffensiveWordsList();
                }
            }
        }

        private void DeleteWord_Click(object sender, RoutedEventArgs e)
        {
            if (WordsList.SelectedItem is string selectedWord)
            {
                checkersService.DeleteOffensiveWord(selectedWord);
                WordsList.ItemsSource = null;
                WordsList.ItemsSource = checkersService.getOffensiveWordsList();
            }
        }

        private void CancelWordPopup_Click(object sender, RoutedEventArgs e)
        {
            WordListPopup.Visibility = Visibility.Collapsed;
        }
    }
}
