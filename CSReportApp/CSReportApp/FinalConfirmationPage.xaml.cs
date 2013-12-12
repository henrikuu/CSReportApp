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
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;

namespace CSReportApp
{
    public partial class FinalConfirmationPage : PhoneApplicationPage
    {
        private string faultText = "";
        private string additionalText = "";
        private string positionString = "";
        private string anid = "";
        private List<String> faultListSource;

        public FinalConfirmationPage()
        {
            InitializeComponent();

            faultListSource = new List<String>();
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
            sendReportButton.IsEnabled = false;
            cancelReportButton.IsEnabled = false;
            moreInfoTextBox.IsEnabled = false;
            faultSelectionlistPicker.IsEnabled = false;

            faultText = faultListSource[faultSelectionlistPicker.SelectedIndex];

            if (moreInfoTextBox.Text.ToString() != "More information (optional)")
                additionalText = moreInfoTextBox.Text.ToString();
            
            anid = GetAnid();

            progressBar.Visibility = Visibility.Visible;
            statusTextBlock.Text = "Getting location...";
            statusTextBlock.Visibility = Visibility.Visible;

            getGeolocation();
        }

        private void uploadReport()
        {
            //Testikoodissa simuloidaan upload lisäämällä 5 sekunnin odotus worker threadilla
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(doWorkCompleted);
            worker.DoWork += new DoWorkEventHandler(doWork);
            worker.RunWorkerAsync();
        }

        public static void doWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(7000);
        }

        public void doWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                progressBar.Visibility = Visibility.Collapsed;
                statusTextBlock.Text = "";

                MessageBox.Show("Thank you for your report!");                
            });
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative)));
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

        /// <summary>
        /// Gets geolocation data. Is run asynchronously.
        /// </summary>
        private async void getGeolocation() 
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition position = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(1), timeout: TimeSpan.FromSeconds(30));

                positionString = string.Format("Latitude: {0:0.0000}, Longitude: {1:0.0000}, Accuracy: {2}", position.Coordinate.Latitude, position.Coordinate.Longitude, position.Coordinate.Accuracy);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    MessageBox.Show(ex.Message);
                });
            }
            finally
            {
                statusTextBlock.Text = "Uploading...";
                uploadReport();
            }
        }

        /// <summary>
        /// Go back to the previous view if cancel button is pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelReportButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());  
        }

        /// <summary>
        /// Gets rid of the textbox hint text when the textbox is activated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moreInfoTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (moreInfoTextBox.Text == "More information (optional)")
            {
                moreInfoTextBox.Text = "";
                moreInfoTextBox.FontStyle = System.Windows.FontStyles.Normal;
            }   
        }

        /// <summary>
        /// Changes the focus outside the textbox if enter key is pressed, which in turn closes the keyboard. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moreInfoTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }
    }
}