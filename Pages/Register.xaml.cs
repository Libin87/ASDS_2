using System;
using System.Threading.Tasks;
using ASDS_dev.Pages.UsrMgmt;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ASDS_dev.Pages
{
    public sealed partial class Register : Page
    {
        public Register()
        {
            InitializeComponent();
        }

        private async void Register_click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameBox.Text.Trim();
            string lastName = LastNameBox.Text.Trim();
            string UserId = usernameBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmBox.Password;

            bool isValid = true;

            // Validate First Name
            if (string.IsNullOrWhiteSpace(firstName))
            {
                FirstNameError.Text = "First name is required.";
                FirstNameError.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(lastName))
            {
                LastNameError.Text = "Last name is required.";
                LastNameError.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(UserId))
            {
                usernameError.Text = "UserName is required.";
                usernameError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (UserId.Length < 5)
            {
                usernameError.Text = "UserName must be at least 5 characters.";
                usernameError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                usernameError.Visibility = Visibility.Collapsed;
            }



            // Validate Password
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError.Text = "Password is required.";
                PasswordError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (password.Length < 6)
            {
                PasswordError.Text = "Password must be at least 6 characters.";
                PasswordError.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Validate Confirm Password
            if (string.IsNullOrWhiteSpace(confirm))
            {
                ConfirmError.Text = "Confirm your password.";
                ConfirmError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (password != confirm)
            {
                ConfirmError.Text = "Passwords do not match.";
                ConfirmError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
                return;

            // Proceed with registration
            string passwordHash = PasswordHelper.HashPassword(password);
            bool success = UserDatabase.RegisterUser(UserId,firstName, lastName, passwordHash);
            if (success)
            {
                Frame.Navigate(typeof(Login));
                await ShowDialog("Success", "Registration successful!");
            }
            else
            {
                await ShowDialog("Error", "Email already exists.");
            }
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

       

        // LostFocus handlers for live inline validation
        private void FirstNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FirstNameError.Text = string.IsNullOrWhiteSpace(FirstNameBox.Text)
                ? "First name is required."
                : "";
            FirstNameError.Visibility = string.IsNullOrWhiteSpace(FirstNameBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        private void usernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            usernameError.Text = string.IsNullOrWhiteSpace(usernameBox.Text)
                ? "Username is required."
                : "";
            usernameError.Visibility = string.IsNullOrWhiteSpace(usernameBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LastNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            LastNameError.Text = string.IsNullOrWhiteSpace(LastNameBox.Text)
                ? "Last name is required."
                : "";
            LastNameError.Visibility = string.IsNullOrWhiteSpace(LastNameBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                PasswordError.Text = "Password is required.";
                PasswordError.Visibility = Visibility.Visible;
            }
            else if (PasswordBox.Password.Length < 6)
            {
                PasswordError.Text = "Password must be at least 6 characters.";
                PasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordError.Text = "";
                PasswordError.Visibility = Visibility.Collapsed;
            }
        }

        private void ConfirmBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ConfirmBox.Password))
            {
                ConfirmError.Text = "Confirm your password.";
                ConfirmError.Visibility = Visibility.Visible;
            }
            else if (ConfirmBox.Password != PasswordBox.Password)
            {
                ConfirmError.Text = "Passwords do not match.";
                ConfirmError.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmError.Text = "";
                ConfirmError.Visibility = Visibility.Collapsed;
            }
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainWindow)); 
        }
        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ASDS_dev.Pages.Login));
        }

    }
}
