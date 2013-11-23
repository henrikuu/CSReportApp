using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;

namespace CSReportApp
{
    public partial class PhotoCapturePage : PhoneApplicationPage
    {
        PhotoCamera camera;

        public PhotoCapturePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
            {
                camera = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                photoViewfinderVideoBrush.SetSource(camera);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (camera != null)
            {
                camera.Dispose();
            }
        }

        private void takePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            //if (camera != null)
            //{
            //    camera.CaptureImage();
            //}
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}