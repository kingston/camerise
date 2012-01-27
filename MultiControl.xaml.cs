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
using SkeletalTracking.Gestures;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MultiControl.xaml
    /// </summary>
    public partial class MultiControl : UserControl, Interfaces.IGestureControl
    {
        private Boolean DEBUG_MODE = true;

        private GestureRecognizer recognizer;
        private DispatcherTimer countdownTimer = new DispatcherTimer();
   

        public MultiControl()
        {
            InitializeComponent();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            recognizer = new GestureRecognizer();
            recognizer.AddGesture(new SwipeGesture());
            recognizer.AddGesture(new ShutterGesture());
            recognizer.AddGesture(new ThrustGesture());
            recognizer.GestureCompleted += new GestureRecognizer.GestureEventHandler(recognizer_GestureCompleted);
            recognizer.GestureLeft += new GestureRecognizer.GestureEventHandler(recognizer_GestureLeft);
            recognizer.GestureStarted += new GestureRecognizer.GestureEventHandler(recognizer_GestureStarted);
        }

        public void processVideoFrame(ImageFrameReadyEventArgs e)
        {
            //Automagically create BitmapSource for Video
            uxBackgroundImage.Source = e.ImageFrame.ToBitmapSource();
        }

        public void processSkeletonFrame(SkeletonFrame frame)
        {
            // Get first skeleton
            SkeletonData skeleton = (from s in frame.Skeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();

            if (skeleton != null)
            {
                // Output diagnostic info
                if (DEBUG_MODE)
                {
                    String diagnosticInfo = "Diagnostic Info:\n";
                    JointID[] diagIDs = { JointID.ShoulderRight, JointID.HandRight, JointID.ElbowRight, JointID.Spine };
                    foreach (var id in diagIDs)
                    {
                        diagnosticInfo += getJointString(skeleton, id);
                    }
                    diagnosticInfo += "Funky Stuff:\n";

                    uxDiagnosticLabel.Content = diagnosticInfo;
                }
                recognizer.ProcessSkeleton(skeleton);
            }
            else
            { 
                uxDiagnosticLabel.Content = "No skeleton detected currently";
            }
        }

        private string getJointString(SkeletonData skeleton, JointID jointID)
        {
            Joint jt = skeleton.Joints[jointID];
            var pos = jt.Position;
            return jointID.ToString() + ": (" + pos.X + ", " + pos.Y + ", " + pos.Z + ")\n";
        }

        public void activateControl()
        {
            // Do nothing for now
        }

        void recognizer_GestureStarted(object sender, GestureEventArgs e)
        {
            Button curButton = GestureToButton(e.Gesture);
            curButton.Background = new SolidColorBrush(Color.FromArgb(0x88, 0xFF, 0xFF, 0xFF));
        }

        void recognizer_GestureLeft(object sender, GestureEventArgs e)
        {
            Button curButton = GestureToButton(e.Gesture);
            curButton.Background = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));
        }

        void recognizer_GestureCompleted(object sender, GestureEventArgs e)
        {
            Button curButton = GestureToButton(e.Gesture);
            curButton.Background = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));
            IGesture gesture = e.Gesture;
            if (gesture.GetType() == typeof(SwipeGesture))
            {
                if (OnLastPhotoActivated != null)
                    OnLastPhotoActivated(this, EventArgs.Empty);
            }
            else if (gesture.GetType() == typeof(ShutterGesture))
            {
                ActivateCameraTimer();
            }
            else if (gesture.GetType() == typeof(ThrustGesture))
            {
                if (OnSettingsActivated != null)
                    OnSettingsActivated(this, EventArgs.Empty);
            }
        }

        private int timeLeft = -1;

        private void ActivateCameraTimer()
        {
            // Avoid duplicate camera actions
            if (timeLeft < 0)
            {
                uxCountdownOverlayCanvas.Visibility = System.Windows.Visibility.Visible;
                timeLeft = 3;
                countdownTimer_Tick(null, EventArgs.Empty); //Manually trigger event
                countdownTimer.Start();
            }
        }

        void countdownTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                uxCountdownLabel.Content = timeLeft + "...";
                timeLeft--;
            }
            else if (timeLeft == 0)
            {
                uxCountdownOverlayCanvas.Visibility = System.Windows.Visibility.Collapsed;
                if (OnTakePhotoActivated != null)
                    OnTakePhotoActivated(this, EventArgs.Empty);
                timeLeft--;
                countdownTimer.Stop();
            }
        }

        private Button GestureToButton(IGesture gesture)
        {
            if (gesture.GetType() == typeof(SwipeGesture))
            {
                return uxViewLastButton;
            }
            else if (gesture.GetType() == typeof(ShutterGesture))
            {
                return uxTakePhotoButton;
            }
            else if (gesture.GetType() == typeof(ThrustGesture))
            {
                return uxSettingsButton;
            }
            return null;
        }


        public event EventHandler OnTakePhotoActivated;

        public event EventHandler OnLastPhotoActivated;

        public event EventHandler OnSettingsActivated;
    }
}
