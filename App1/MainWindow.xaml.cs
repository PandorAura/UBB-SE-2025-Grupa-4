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

using App1.Models;
using System.Collections.ObjectModel;
using App1.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        private ReviewsService reviewsService;
        public ObservableCollection<Review> Reviews { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            reviewsService = new ReviewsService();

            Reviews = new ObservableCollection<Review>
            {
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review(),
                new Review()
            };

            ReviewsList.ItemsSource = Reviews;
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


    }

}
