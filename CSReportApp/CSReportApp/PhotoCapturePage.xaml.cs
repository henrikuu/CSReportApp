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
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.IO;

namespace CSReportApp
{
    public partial class PhotoCapturePage : PhoneApplicationPage
    {
        PhotoCamera camera;
        private bool focusAndShoot = false;
        private int counter = 0;

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

                photoViewfinderVideoBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };

                camera.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(captureCompletedHandler);
                camera.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(captureImageAvailableHandler);
                camera.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(captureThumbnailAvailableHandler);
                camera.AutoFocusCompleted += new EventHandler<CameraOperationCompletedEventArgs>(autoFocusCompletedHandler);

                CameraButtons.ShutterKeyHalfPressed += onButtonHalfPressHandler;
                CameraButtons.ShutterKeyPressed += onButtonFullPressHandler;
                CameraButtons.ShutterKeyReleased += onButtonReleaseHandler;

            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                 {
                     MessageBox.Show("Camera not supported in this phone");
                 });

                NavigationService.GoBack();
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

            //Deployment.Current.Dispatcher.BeginInvoke(delegate()
            //{
            //    MessageBox.Show("Navigate from");
            //});

            if (camera != null)
            {
                camera.Dispose();
                camera.CaptureImageAvailable -= captureImageAvailableHandler;
                camera.CaptureCompleted -= captureCompletedHandler;
                camera.CaptureThumbnailAvailable -= captureThumbnailAvailableHandler;
                camera.AutoFocusCompleted -= autoFocusCompletedHandler;

                CameraButtons.ShutterKeyHalfPressed -= onButtonHalfPressHandler;
                CameraButtons.ShutterKeyPressed -= onButtonFullPressHandler;
                CameraButtons.ShutterKeyReleased -= onButtonReleaseHandler;
            }
        }

        private void takePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (camera != null)
                {
                    focusAndShoot = true;
                    camera.Focus();
                }
            }
            catch (Exception) { }
        }

        void autoFocusCompletedHandler(object sender, CameraOperationCompletedEventArgs e)
        {
            if (focusAndShoot)
            {
                focusAndShoot = false;

                try
                {
                    camera.CaptureImage();
                }
                catch (Exception) { }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());
        }

        void captureCompletedHandler(object sender, CameraOperationCompletedEventArgs e)
        {
            counter++;
        }
        
        public void captureThumbnailAvailableHandler(object sender, ContentReadyEventArgs e)
        {
            string fileName = counter + "_th.jpg";

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead = -1;

                        while ((bytesRead = e.ImageStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                e.ImageStream.Close();
            }
        }
        
        void captureImageAvailableHandler(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = counter + ".jpg";

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = isf.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead = -1;

                        while ((bytesRead = e.ImageStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                PhoneApplicationService.Current.State["fileName"] = fileName;
                e.ImageStream.Close();

                navigateToConfirmationPage();
            }
        }

        private void navigateToConfirmationPage()
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/PhotoConfirmationView.xaml", UriKind.Relative)));
        }

        private void onButtonHalfPressHandler(object sender, EventArgs e)
        {
            if (camera != null && camera.IsFocusSupported == true)
            {
                try
                {
                    camera.Focus();
                }
                catch (Exception) { }
            }
        }

        private void onButtonFullPressHandler(object sender, EventArgs e)
        {
            if (camera != null)
            {
                try
                {
                    camera.CaptureImage();
                }
                catch (Exception) { }
            }
        }

        private void onButtonReleaseHandler(object sender, EventArgs e)
        {
            try
            {
                if (camera != null && camera.IsFocusSupported == true)
                {
                    camera.CancelFocus();
                }
            }
            catch (Exception) { }
        }

        private void photoViewfinderCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            takePhotoButton_Click(this, null);
        }
    }
}