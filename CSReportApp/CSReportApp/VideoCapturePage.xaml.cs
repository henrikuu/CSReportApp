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
using System.Windows.Media;
using System.IO;

namespace CSReportApp
{
    public partial class VideoCapturePage : PhoneApplicationPage
    {
        private CaptureSource captureSource;
        private VideoCaptureDevice videoCaptureDevice;
        private FileSink fileSink;
        private string fileName = "capture.mp4";
        private string thumbnailFileName = "capture_th.jpg";
        private bool recording = false;

        public VideoCapturePage()
        {
            InitializeComponent();
        }

        private void startRecording()
        {
            try
            {
                if (captureSource.VideoCaptureDevice != null && captureSource.State == CaptureState.Started)
                {
                    captureSource.Stop();

                    fileSink.CaptureSource = captureSource;
                    fileSink.IsolatedStorageFileName = fileName;
                }

                if (captureSource.VideoCaptureDevice != null && captureSource.State == CaptureState.Stopped)
                {
                    captureSource.Start();
                    captureSource.CaptureImageAsync();

                    recordButton.Content = "Stop";
                    recording = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void stopRecording()
        {
            try
            {
                if (captureSource.VideoCaptureDevice != null && captureSource.State == CaptureState.Started)
                {
                    captureSource.Stop();
                    recording = false;

                    fileSink.CaptureSource = null;
                    fileSink.IsolatedStorageFileName = null;

                    recordButton.Content = "Record";
                }
            }
            catch (Exception)
            {
            }
            finally 
            {
                PhoneApplicationService.Current.State["videoFileName"] = fileName;
                PhoneApplicationService.Current.State["thumbnailFileName"] = thumbnailFileName;

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/VideoConfirmationPage.xaml", UriKind.Relative)));
            }
        }

        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!recording)
            {
                startRecording();
            }
            else
            {
                stopRecording();
            }
        }

        private void viewfinderRectangle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!recording)
            {
                startRecording();
            }
            else
            {
                stopRecording();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (captureSource == null)
            {
                captureSource = new CaptureSource();
                captureSource.CaptureImageCompleted += captureSource_CaptureImageCompleted;

                fileSink = new FileSink();
                videoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

                if (videoCaptureDevice != null)
                {
                    videoBrush.SetSource(captureSource);
                    videoBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };
                    captureSource.Start();
                }
            }
        }

        private void captureSource_CaptureImageCompleted(object sender, CaptureImageCompletedEventArgs e)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                System.Windows.Media.Imaging.WriteableBitmap wb = e.Result;

                if (isoStore.FileExists(thumbnailFileName))
                    isoStore.DeleteFile(thumbnailFileName);

                IsolatedStorageFileStream file = isoStore.CreateFile(thumbnailFileName);

                System.Windows.Media.Imaging.Extensions.SaveJpeg(wb, file, wb.PixelWidth, wb.PixelHeight, 0, 85);

                file.Close();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (captureSource != null)
            {
                captureSource.CaptureImageCompleted -= captureSource_CaptureImageCompleted;

                if (captureSource.VideoCaptureDevice != null && captureSource.State == CaptureState.Started)
                {
                    captureSource.Stop();
                }

                captureSource = null;
                fileSink = null;
                videoCaptureDevice = null;
            }

            base.OnNavigatedFrom(e);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.GoBack());
        }
    }
}