using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ASDS_dev.Pages;
public sealed partial class DateTimeSync : Page
{
    public DateTimeSync()
    {
        this.InitializeComponent();

        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (s, e) =>
        {
            DateTimeTextBlock.Text = DateTime.Now.ToString("MMMM d, yyyy - hh: mm:ss tt");
        };
        timer.Start();

        UsernameText.Text = $"User: {SessionManager.CurrentUsername}";
       

    }
    private DispatcherTimer timer = new();

   
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(Login));
    }
    private void UserPage1(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(ASDS_dev.Pages.UsrMgmt.UserPage));
    }
    private void HomeClick(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(ASDS_dev.MainWindow));

    }
    private void timeClick(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(ASDS_dev.Pages.DateTimeSync));
    }
}
