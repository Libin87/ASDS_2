using System;
using System.Threading.Tasks;
using ASDS_dev.Pages.UsrMgmt;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static ASDS_dev.Pages.DatabaseHelper;

namespace ASDS_dev.Pages
{
    public sealed partial class Login : Page
    {
        public Login()
            {
                InitializeComponent();
            }

        private void ChangePswd(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ASDS_dev.Pages.ChangePswd));
        }
       

        private async void Login_click(object sender, RoutedEventArgs e)
        {
            string userId = usernameBox.Text;
            string password = PasswordBox.Password;

            var result = UserDatabase.ValidateLogin(userId, password);

            if (result.IsUserNotFound)
            {
                await ShowDialog("Login Failed", "User not found. Please sign up first.");
                return;
            }

            if (result.IsPasswordIncorrect)
            {
                await ShowDialog("Login Failed", "Incorrect password. Please try again.");
                return;
            }

            if (result.IsSuspended)
            {
                await ShowDialog("Access Denied", "Your account is Blocked. Please contact admin.");
                return;
            }
            LoginButton.Visibility = Visibility.Collapsed;
            
            

            SessionManager.CurrentUsername = userId;
            usernameBox.Text = $"User: {userId}";
            Frame.Navigate(typeof(MainWindow));
            await ShowDialog("Login Successful", $"Welcome back, {userId}!");
        }


        private async Task ShowDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainWindow));
        }




    }
}
