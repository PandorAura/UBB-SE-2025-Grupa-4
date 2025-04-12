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
using App1.ViewModels;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1.Views
{

    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; }

        public MainPage(IReviewService reviewsService,
                   IUserService userService, IUpgradeRequestsService upgradeRequestsService, ICheckersService checkersService, IAutoCheck autoCheck)
        {
            this.InitializeComponent();

            ViewModel = new MainPageViewModel(
                reviewsService,
                userService,
                upgradeRequestsService,
                checkersService,
                autoCheck
            );

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            this.DataContext = ViewModel;
            
            this.Unloaded += MainPage_Unloaded;
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsWordListVisible))
            {
                WordListPopup.Visibility = ViewModel.IsWordListVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void AppealsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is User selectedUser)
            {
                ViewModel.SelectedAppealUser = selectedUser;
                
                ShowAppealDetailsUI(sender);
            }
        }
        
        private void ShowAppealDetailsUI(object anchor)
        {
            Flyout flyout = new Flyout();
            StackPanel panel = new StackPanel { Padding = new Thickness(10) };
            TextBlock userInfo = new TextBlock
            {
                FontSize = 18
            };
            userInfo.SetBinding(TextBlock.TextProperty, new Microsoft.UI.Xaml.Data.Binding
            {
                Path = new PropertyPath("UserStatusDisplay"),
                Source = ViewModel,
                Mode = Microsoft.UI.Xaml.Data.BindingMode.OneWay
            });
            
            TextBlock reviewsHeader = new TextBlock
            {
                Text = "User Reviews:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 5)
            };
            
            ListView reviewsList = new ListView
            {
                MaxHeight = 200
            };
            reviewsList.SetBinding(ListView.ItemsSourceProperty, new Microsoft.UI.Xaml.Data.Binding
            {
                Path = new PropertyPath("UserReviewsFormatted"),
                Source = ViewModel,
                Mode = Microsoft.UI.Xaml.Data.BindingMode.OneWay
            });
            
            Button banButton = new Button
            {
                Content = "Keep Ban",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Command = ViewModel.KeepBanCommand
            };
            
            Button appealButton = new Button
            {
                Content = "Accept Appeal",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green),
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Command = ViewModel.AcceptAppealCommand
            };
            
            Button closeButton = new Button
            {
                Content = "Close Appeal Case",
                HorizontalAlignment = HorizontalAlignment.Center,
                Command = ViewModel.CloseAppealCaseCommand
            };
            
            closeButton.Click += (s, args) => { flyout.Hide(); };

            panel.Children.Add(userInfo);

            panel.Children.Add(reviewsHeader);
            panel.Children.Add(reviewsList);
            panel.Children.Add(banButton);
            panel.Children.Add(appealButton);
            panel.Children.Add(closeButton);
            
            flyout.Content = panel;
            flyout.Placement = FlyoutPlacementMode.Left;
            flyout.ShowAt((FrameworkElement)anchor);
        }
        
        private void RequestList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is UpgradeRequest selectedRequest)
            {
                ViewModel.SelectedUpgradeRequest = selectedRequest;
                ShowUpgradeRequestDetailsUI(sender);
            }
        }
        
        private void ShowUpgradeRequestDetailsUI(object anchor)
        {
            Flyout flyout = new Flyout();
            StackPanel panel = new StackPanel { Padding = new Thickness(10) };
            
            TextBlock userInfo = new TextBlock
            {
                Text = ViewModel.UserUpgradeInfo,
                FontSize = 18
            };

            TextBlock reviewsHeader = new TextBlock
            {
                Text = "User Reviews:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 5)
            };

            ListView reviewsList = new ListView
            {
                ItemsSource = ViewModel.UserReviewsWithFlags,
                Height = 100
            };

            panel.Children.Add(userInfo);
            panel.Children.Add(reviewsHeader);
            panel.Children.Add(reviewsList);

            flyout.Content = panel;
            flyout.Placement = FlyoutPlacementMode.Left;
            flyout.ShowAt((FrameworkElement)anchor);
        }
        
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int requestId)
            {
                ViewModel.HandleUpgradeRequest(true, requestId);
            }
        }
        
        private void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int requestId)
            {
                ViewModel.HandleUpgradeRequest(false, requestId);
            }
        }

        private void ReviewSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterReviews(ReviewSearchTextBox.Text);
        }

        private void BannedUserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        { 
            ViewModel.FilterAppeals(BannedUserSearchTextBox.Text);
        }

        private void MenuFlyoutAllowReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                ViewModel.ResetReviewFlags(review.ReviewID);
            }
        }

        private void MenuFlyoutHideReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                ViewModel.HideReview(review.UserID, review.ReviewID);
            }
        }

        private void MenuFlyoutAICheck_Click_2(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is Review review)
            {
                ViewModel.RunAICheck(review);
            }
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
                ViewModel.AddOffensiveWord(newWord);
            }
        }
    }
}
