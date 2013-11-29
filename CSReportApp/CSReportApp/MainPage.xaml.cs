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

namespace CSReportApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {

            InitializeComponent();
            //tee tsekkaus gps
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
    }
}