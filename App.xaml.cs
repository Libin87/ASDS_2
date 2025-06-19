using ASDS_dev.Pages;
using ASDS_dev.Pages.UsrMgmt;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ASDS_dev
{
    public partial class App : Application
    {
        private Window m_window;
        public App()
        {
            this.InitializeComponent();
        }
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            DatabaseHelper.InitializeDatabase();

            m_window = new Window();
            Frame rootFrame = new Frame();
            m_window.Content = rootFrame;
            rootFrame.Navigate(typeof(MainWindow)); 
            m_window.Activate();
        }



    }
}
