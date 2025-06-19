using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ASDS_dev.Pages.UsrMgmt;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace ASDS_dev.Pages;
public sealed partial class ChangePswd : Page
{
    public ChangePswd()
    {
        InitializeComponent();
    }

    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        string userId = usernameBox.Text.Trim();
        string oldPassword = OldPswdBox.Text.Trim();
        string newPassword = NewPswd.Password.Trim();
        string confirmPassword = CnfPswd.Password.Trim();
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(userId))
        {
            ShowDialog("All fields are required.");
            return;
        }
        string pattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{6,}$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword, pattern))
        {
            ShowDialog("Password must be at least 6 characters long and include a letter, number, and special symbol.");
            return;
        }
        if (newPassword ==oldPassword)
        {
            ShowDialog("New password and old password can`t be same.");
            return;
        }

        if (newPassword != confirmPassword)
        {
            ShowDialog("New password and Confirm password do not match.");
            return;
        }

        if (!UserDatabase.ValidateUser(userId, oldPassword))
        {
            ShowDialog("Old password is incorrect or user not found.");
            return;
        }

        bool updated = UserDatabase.UpdateUserPassword(userId, newPassword);

        if (updated)
        {
            ShowDialog("Password updated successfully.");
            ClearInputs();
            Frame.Navigate(typeof(Login));
        }
        else
        {
            ShowDialog("Failed to update password. Try again.");
        }
    }

    private void ClearInputs()
    {
        OldPswdBox.Text = "";
        usernameBox.Text = "";
        NewPswd.Password = "";
        CnfPswd.Password = "";
    }

   

    private void ShowDialog(string message)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = "Validation",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        _ = dialog.ShowAsync();
    }

    private void LoginClick(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(ASDS_dev.Pages.Login));
    }
    private void Home_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainWindow)); 
    }
}
