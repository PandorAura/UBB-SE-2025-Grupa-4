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
using App1.Ai_Check;
using App1.Services;
using App1.Models;
using System.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
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
                XamlRoot = this.Content.XamlRoot // Set the XamlRoot property
            };
            _ = dialog.ShowAsync();
        }

        private List<Review> reviews = new List<Review>
            {
                new Review(1, "This product is stupid! I love it.", 101),
                new Review(2, "Terrible service, waste of money.", 102),
                new Review(3, "Absolutely fantastic! Highly recommended.", 103),
                new Review(4, "This is complete garbage, don't buy!", 104),
                new Review(5, "Meh, it’s okay. Nothing idiot.", 105)
            };
            
        private void AutoCheck_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ok");
            CheckersService checkers = new CheckersService();
            List<string> messages = checkers.RunAutoCheck(reviews);

            OutputBox.Text = "";
            foreach (var message in messages)
            {
                OutputBox.Text += message + '\n';
            }
        }
    }

}
