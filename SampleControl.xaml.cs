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

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for SampleControl.xaml
    /// </summary>
    public partial class SampleControl : UserControl, Interfaces.IGestureControl
    {
        private Dictionary<int, Target> targets = new Dictionary<int, Target>();
        private SkeletonController currentController;

        //Scaling constants
        public float k_xMaxJointScale = 1.5f;
        public float k_yMaxJointScale = 1.5f;

        public SampleControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Force video to the background
            Canvas.SetZIndex(image1, -10000);
            currentController = new SkeletonController(null);
            InitTargets();
        }

        private void InitTargets()
        {
            targets.Add(1, new Target(target1, 1));
            targets.Add(2, new Target(target2, 2));
            targets.Add(3, new Target(target3, 3));
            targets.Add(4, new Target(target4, 4));
            targets.Add(5, new Target(target5, 5));
            currentController.controllerActivated(targets);
            Canvas.SetZIndex(target1, 100);
            Canvas.SetZIndex(target2, 100);
            Canvas.SetZIndex(target3, 100);
            Canvas.SetZIndex(target4, 100);
            Canvas.SetZIndex(target5, 100);
        }

        public void processVideoFrame(ImageFrameReadyEventArgs e)
        {
            //Automagically create BitmapSource for Video
            image1.Source = e.ImageFrame.ToBitmapSource();    
        }

        public void processSkeletonFrame(SkeletonFrame frame)
        {

            SkeletonFrame allSkeletons = frame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();


            if (skeleton != null)
            {
                //set positions on our joints of interest (already defined as Ellipse objects in the xaml)
                SetEllipsePosition(headEllipse, skeleton.Joints[JointID.Head]);
                SetEllipsePosition(leftEllipse, skeleton.Joints[JointID.HandLeft]);
                SetEllipsePosition(rightEllipse, skeleton.Joints[JointID.HandRight]);
                SetEllipsePosition(shoulderCenter, skeleton.Joints[JointID.ShoulderCenter]);
                SetEllipsePosition(shoulderRight, skeleton.Joints[JointID.ShoulderRight]);
                SetEllipsePosition(shoulderLeft, skeleton.Joints[JointID.ShoulderLeft]);
                SetEllipsePosition(ankleRight, skeleton.Joints[JointID.AnkleRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointID.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointID.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointID.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(elbowLeft, skeleton.Joints[JointID.ElbowLeft]);
                SetEllipsePosition(elbowRight, skeleton.Joints[JointID.ElbowRight]);
                SetEllipsePosition(ankleLeft, skeleton.Joints[JointID.AnkleLeft]);
                SetEllipsePosition(footLeft, skeleton.Joints[JointID.FootLeft]);
                SetEllipsePosition(footRight, skeleton.Joints[JointID.FootRight]);
                SetEllipsePosition(wristLeft, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(wristRight, skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(kneeLeft, skeleton.Joints[JointID.KneeLeft]);
                SetEllipsePosition(kneeRight, skeleton.Joints[JointID.KneeRight]);
                SetEllipsePosition(hipCenter, skeleton.Joints[JointID.HipCenter]);
                currentController.processSkeletonFrame(skeleton, targets);

            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            var scaledJoint = joint.ScaleTo(640, 480, k_xMaxJointScale, k_yMaxJointScale);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetTop(ellipse, scaledJoint.Position.Y - (double)ellipse.GetValue(Canvas.WidthProperty) / 2);
            Canvas.SetZIndex(ellipse, (int)-Math.Floor(scaledJoint.Position.Z * 100));
            if (joint.ID == JointID.HandLeft || joint.ID == JointID.HandRight)
            {
                byte val = (byte)(Math.Floor((joint.Position.Z - 0.8) * 255 / 2));
                ellipse.Fill = new SolidColorBrush(Color.FromRgb(val, val, val));
            }
        }

        public void activateControl()
        {
            //throw new NotImplementedException();
        }

        public event EventHandler OnTakePhotoActivated;

        public event EventHandler OnLastPhotoActivated;

        public event EventHandler OnSettingsActivated;
    }
}
