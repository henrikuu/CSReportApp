using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Info;

namespace CSReportApp
{
    public partial class FinalConfirmationPage : PhoneApplicationPage
    {
        private string faultText;
        private string additionalText = "";
        private string positionString;
        private string anid;

        public FinalConfirmationPage()
        {
            InitializeComponent();

            List<String> faultListSource = new List<String>();
            faultListSource.Add("None selected");
            faultListSource.Add("Broken streetlamp");
            faultListSource.Add("Pothole");
            faultListSource.Add("Vandalized property");
            faultListSource.Add("Graffiti");
            faultListSource.Add("Blocked road");

            faultSelectionlistPicker.ItemsSource = faultListSource;
        }

        private void sendReportButton_Click(object sender, RoutedEventArgs e)
        {
            //faultText = FaultSelection.SelectedItem.ToString();
            
            if (moreInfoTextBox.Text.ToString() != "More information (optional)")
                additionalText = moreInfoTextBox.Text.ToString();

            getGeolocation();
            anid = GetAnid();

        }

        /// <summary>
        /// Gets the user's unique user ID. Returns a blank guid if uid retrieval fails.
        /// </summary>
        /// <returns></returns>
        private string GetAnid()
        {
            object anid;

            int ANIDLength = 32;
            int ANIDOffset = 2;

            if (UserExtendedProperties.TryGetValue("ANID2", out anid))
            {
                if (anid != null && anid.ToString().Length >= (ANIDLength + ANIDOffset - 1))
                {
                    return anid.ToString().Substring(ANIDOffset, ANIDLength);
                }
                else
                {
                    return Guid.Empty.ToString();
                }
            }
            else
            {
                return Guid.Empty.ToString();
            }
        } 

        private async void getGeolocation() 
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                sendReportButton.IsEnabled = false;
                progressBar.Visibility = Visibility.Visible;
                statusTextBlock.Text = "Getting location...";
                statusTextBlock.Visibility = Visibility.Visible;

                Geoposition position = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(1), timeout: TimeSpan.FromSeconds(30));

                positionString = string.Format("Latitude: {0:0.0000}, Longitude: {1:0.0000}, Accuracy: {2}", position.Coordinate.Latitude, position.Coordinate.Longitude, position.Coordinate.Accuracy);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
                statusTextBlock.Text = "";
                statusTextBlock.Visibility = Visibility.Collapsed;
                sendReportButton.IsEnabled = true;
            }
        }

        private void cancelReportButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());  
        }

        private void moreInfoTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (moreInfoTextBox.Text == "More information (optional)")
            {
                moreInfoTextBox.Text = "";
                moreInfoTextBox.FontStyle = System.Windows.FontStyles.Normal;
            }   
        }
    }
}