using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace CSReportApp
{
    public partial class VideoConfirmationPage : PhoneApplicationPage
    {
        private string fileName;
        private string thumbnailFileName;

        public VideoConfirmationPage()
        {
            InitializeComponent();

            fileName = (string)PhoneApplicationService.Current.State["videoFileName"];
            thumbnailFileName = (string)PhoneApplicationService.Current.State["thumbnailFileName"];

            loadThumbnail();
        }

        private void loadThumbnail()
        {
            byte[] data;

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = isf.OpenFile(thumbnailFileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        stream.Close();
                    }
                }

                MemoryStream memoryStream = new MemoryStream(data);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(memoryStream);
                imageBrush.ImageSource = bitmap;
                imageBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };
            }
            catch (Exception)
            {
            }
        }

        private void RetakeButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());            
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FinalConfirmationPage.xaml", UriKind.Relative));
        }

        private void TouchLayer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            startPlayback();        
        }

        private void startPlayback()
        {
            MediaPlayerLauncher mediaPlayerLauncher = new MediaPlayerLauncher();

            mediaPlayerLauncher.Media = new Uri(fileName, UriKind.Relative);
            mediaPlayerLauncher.Location = MediaLocationType.Data;
            mediaPlayerLauncher.Controls = MediaPlaybackControls.None;
            mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Portrait;

            mediaPlayerLauncher.Show();
        }

        private void imageCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            startPlayback();
        }
    }
}