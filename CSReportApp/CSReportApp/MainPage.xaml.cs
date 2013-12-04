using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CSReportApp.Resources;
using System.Device.Location;

namespace CSReportApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {

            InitializeComponent();
            checkGPS();
        }

        private void menuButtonTakePicture_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PhotoCapturePage.xaml", UriKind.Relative));
        }

        private void Help_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

        private void menuButtonTakeVideo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/VideoCapturePage.xaml", UriKind.Relative));
        }

        private void checkGPS()
        {
            GeoCoordinateWatcher g = new GeoCoordinateWatcher();
            g.Start();

            if (g.Permission.Equals(GeoPositionPermission.Denied) || g.Permission.Equals(GeoPositionPermission.Unknown))
            {
                MessageBox.Show("Location services are disabled. To enable them, Goto Settings - Location - Enable Location Services.", "Location services", MessageBoxButton.OK);
                Application.Current.Terminate();
            }
        }
    }
}