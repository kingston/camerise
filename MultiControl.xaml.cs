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

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MultiControl.xaml
    /// </summary>
    public partial class MultiControl : UserControl, Interfaces.IGestureControl
    {
        private Boolean DEBUG_MODE = true;

        private GestureRecognizer recognizer;
   

        public MultiControl()
        {
            InitializeComponent();
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
                    diagnosticInfo += 

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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            recognizer = new GestureRecognizer();
            recognizer.AddGesture(new SwipeGesture());
            recognizer.AddGesture(new ShutterGesture());
            recognizer.AddGesture(new ThrustGesture());
            recognizer.GestureCompleted += new GestureRecognizer.GestureEventHandler(recognizer_GestureCompleted);
        }

        void recognizer_GestureCompleted(object sender, GestureEventArgs e)
        {
            //MessageBox.Show("Detected: "+ e.Gesture.GetType().Name);

            string currGesture = e.Gesture.GetType().Name;
            if (currGesture.Equals("ThrustGesture"))
            {
                Target currTarget = new Target(takePhotoIndicator, 10);
                currTarget.setTargetSelected();
            }
            if (currGesture.Equals("SwipeGesture")) 
            {
                Target currTarget = new Target(viewLastIndicator, 10);
                currTarget.setTargetSelected();
            }
            if (currGesture.Equals("ShutterGesture")) 
            {
                Target currTarget = new Target(settingsIndicator, 10);
                currTarget.setTargetSelected();
            }

        }
    }
}
