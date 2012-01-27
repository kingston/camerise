using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using SkeletalTracking.Interfaces;
using System.Windows.Media.Animation;

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        private IGestureControl currentControl;
        private IGestureControl[] gestureControls;

        private BitmapSource lastSource = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Kinect Runtime
        Runtime nui;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gestureControls = new IGestureControl[] { funControl, multiControl, sampleControl };
            // Attach event handlers
            foreach (IGestureControl control in gestureControls)
            {
                control.OnTakePhotoActivated += new EventHandler(control_OnTakePhotoActivated);
                control.OnLastPhotoActivated += new EventHandler(control_OnLastPhotoActivated);
                control.OnSettingsActivated += new EventHandler(control_OnSettingsActivated);
            }
            changeCurrentControl(funControl);
            SetupKinect();
        }

        void control_OnLastPhotoActivated(object sender, EventArgs e)
        {
            uxTakenImage.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
            uxTakenImage.Opacity = 1;

            // Hide it when we're done
            DoubleAnimation imageAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(500)));
            imageAnimation.BeginTime = TimeSpan.FromSeconds(3);
            Storyboard.SetTarget(imageAnimation, uxTakenImage);
            Storyboard.SetTargetProperty(imageAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard imageSb = new Storyboard();
            imageSb.Children.Add(imageAnimation);
            imageSb.Begin();
        }

        void control_OnSettingsActivated(object sender, EventArgs e)
        {
            uxSettingsOverlayCanvas.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
            uxSettingsOverlayCanvas.Opacity = 1;
            // Hide it when we're done
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(500)));
            fadeOutAnimation.BeginTime = TimeSpan.FromSeconds(1.5);
            Storyboard.SetTarget(fadeOutAnimation, uxSettingsOverlayCanvas);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));
            Storyboard imageSb = new Storyboard();
            imageSb.Children.Add(fadeOutAnimation);
            imageSb.Begin();
        }

        /// <summary>
        /// Provided to make sure multiple photos aren't taken at once
        /// </summary>
        private DateTime lastPhotoTaken = DateTime.MinValue;

        void control_OnTakePhotoActivated(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastPhotoTaken).TotalMilliseconds > 1000)
            {
                // Flash it
                DoubleAnimation flashAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(100)));
                Storyboard.SetTarget(flashAnimation, uxFlashRectangle);
                Storyboard.SetTargetProperty(flashAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard sb = new Storyboard();
                sb.Children.Add(flashAnimation);
                sb.Begin();

                // Actually take photo
                uxTakenImage.Source = lastSource;
                uxTakenImage.BeginAnimation(UIElement.OpacityProperty, null); // reset animation
                uxTakenImage.Opacity = 1;

                // Hide it when we're done
                DoubleAnimation imageAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(500)));
                imageAnimation.BeginTime = TimeSpan.FromSeconds(2);
                Storyboard.SetTarget(imageAnimation, uxTakenImage);
                Storyboard.SetTargetProperty(imageAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard imageSb = new Storyboard();
                imageSb.Children.Add(imageAnimation);
                imageSb.Begin();
                lastPhotoTaken = DateTime.Now;
            }
        }

        private void SetupKinect()
        {
            if (Runtime.Kinects.Count == 0)
            {
                this.Title = "No Kinect connected"; 
            }
            else
            {
                //use first Kinect
                nui = Runtime.Kinects[0];

                //Initialize to do skeletal tracking
                nui.Initialize(RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex);

                //add event to receive skeleton data
                nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

                //add event to receive video data
                nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);

                //to experiment, toggle TransformSmooth between true & false and play with parameters            
                nui.SkeletonEngine.TransformSmooth = true;
                TransformSmoothParameters parameters = new TransformSmoothParameters();
                // parameters used to smooth the skeleton data
                parameters.Smoothing = 0.3f;
                parameters.Correction = 0.3f;
                parameters.Prediction = 0.4f;
                parameters.JitterRadius = 0.7f;
                parameters.MaxDeviationRadius = 0.2f;
                nui.SkeletonEngine.SmoothParameters = parameters;

                //Open the video stream
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                
            }
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            lastSource = e.ImageFrame.ToBitmapSource();
            currentControl.processVideoFrame(e);
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            currentControl.processSkeletonFrame(e.SkeletonFrame);
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            //Cleanup
            nui.Uninitialize();
        }

        private void changeCurrentControl(IGestureControl control)
        {
            foreach (Control gestureControl in gestureControls)
            {
                gestureControl.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.Title = "Current Controller: " + control.GetType().Name;
            uxCurrentController.Content = control.GetType().Name;
            (control as Control).Visibility = System.Windows.Visibility.Visible;
            currentControl = control;
            control.activateControl();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                changeCurrentControl(funControl);
            }

            if (e.Key == Key.D2)
            {
                changeCurrentControl(multiControl);
            }

            if (e.Key == Key.D3)
            {
                changeCurrentControl(sampleControl);
            }

        }
    }


}
