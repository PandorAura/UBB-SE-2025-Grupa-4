using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using App1.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1.Views
{
    public sealed partial class AppealDetailsPage : Page
    {
        private User selectedUser;
        private List<User> users;
        private List<Review> reviews;

        public AppealDetailsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter is (User user, List<User> userList, List<Review> reviewList))
            {
                selectedUser = user;
                users = userList;
                reviews = reviewList;

                UserInfoText.Text = $"User ID: {selectedUser.userId}\nEmail: {selectedUser.email}\nStatus: {(selectedUser.permissionID == 1 ? "Active" : "Banned")}";

                // Load user's reviews
                ReviewsListView.ItemsSource = reviews
                    .Where(r => r.userID == selectedUser.userId)
                    .Select(r => $"Review ID: {r.reviewID}, Content: {r.content}")
                    .ToList();
            }
        }

        private void BanUser_Click(object sender, RoutedEventArgs e)
        {
            selectedUser.permissionID = 0;
            UserInfoText.Text = $"User ID: {selectedUser.userId}\nEmail: {selectedUser.email}\nStatus: Banned";
        }

        private void AcceptAppeal_Click(object sender, RoutedEventArgs e)
        {
            selectedUser.permissionID = 1;
            UserInfoText.Text = $"User ID: {selectedUser.userId}\nEmail: {selectedUser.email}\nStatus: Active";
        }

        private void CloseAppeal_Click(object sender, RoutedEventArgs e)
        {
            selectedUser.hasAppealed = false;

            // Update the main list to remove users with no active appeals
            users.RemoveAll(u => !u.hasAppealed);

            // Navigate back to the previous page (refreshing the list)
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
