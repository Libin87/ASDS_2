using System;
using ASDS_dev;
using ASDS_dev.Pages;
using ASDS_dev.Pages.UsrMgmt;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ASDS_dev
{
    public sealed partial class MainWindow : Page
    {
        public MainViewModel ViewModel { get; set; }
        private CanvasBitmap svgImage;

        public MainWindow()
        {
            this.InitializeComponent();
            if (!string.IsNullOrEmpty(SessionManager.CurrentUsername))
            {
                // User is logged in → Show Logout, Hide Login
                LoginButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;

                UsernameText.Text = $"User: {SessionManager.CurrentUsername}";
            }
            else
            {
                // User is not logged in → Show Login, Hide Logout
                LoginButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Collapsed;

                UsernameText.Text = "User: Guest";
            }
            UserDatabase.InitializeDatabase();


            ViewModel = new MainViewModel();

            // Attach Loaded event handler to the Page
            this.Loaded += MainWindow_Loaded;
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentUsername = null;

            // Refresh UI
            LoginButton.Visibility = Visibility.Visible;
            LogoutButton.Visibility = Visibility.Collapsed;
            UsernameText.Text = "User: Guest";

            Frame.Navigate(typeof(Login)); // Change `MainWindow` to your actual home page class

        }


        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/diagram.svg"));
            using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            var device = CanvasDevice.GetSharedDevice();
            svgImage = await CanvasBitmap.LoadAsync(device, stream);
            SvgCanvas.Invalidate();
        }

        private void SvgCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (svgImage != null)
            {
                args.DrawingSession.DrawImage(svgImage);
            }
        }

        private void WaterVolumeInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(WaterVolumeInput.Text, out double volume))
            {
                double maxVolume = 100.0;
                double targetScale = Math.Clamp(volume / maxVolume, 0, 1);

                DoubleAnimation scaleAnimation = new DoubleAnimation
                {
                    To = targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(scaleAnimation);

                Storyboard.SetTarget(scaleAnimation, WaterLevelScale);
                Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");

                storyboard.Begin();
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ASDS_dev.Pages.Login));
        }

        private void UserPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ASDS_dev.Pages.UsrMgmt.UserPage));
        }
        private void timeClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ASDS_dev.Pages.DateTimeSync));
        }

    }
}
