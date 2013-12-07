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
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace CSReportApp
{
    public partial class PhotoConfirmationView : PhoneApplicationPage
    {
        private string fileName = "";

        public PhotoConfirmationView()
        {
            InitializeComponent();

            fileName = (string)PhoneApplicationService.Current.State["fileName"];
            loadImage();
        }

        private void loadImage()
        {
            byte[] data;

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
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
    }
}