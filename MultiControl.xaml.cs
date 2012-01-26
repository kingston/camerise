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
using SkeletalTracking.Gestures;

namespace SkeletalTracking
{
    /// <summary>
    /// Interaction logic for MultiControl.xaml
    /// </summary>
    public partial class MultiControl : UserControl, Interfaces.IGestureControl
    {
        private GestureRecognizer recognizer;

        public MultiControl()
        {
            InitializeComponent();
            recognizer = new GestureRecognizer();
        }

        public void processVideoFrame(ImageFrameReadyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void processSkeletonFrame(SkeletonFrame frame)
        {
            recognizer.ProcessSkeleton(frame);
        }

        public void activateControl()
        {
            // Do nothing for now
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            recognizer = new GestureRecognizer();
            recognizer.GestureCompleted += new GestureRecognizer.GestureEventHandler(recognizer_GestureCompleted);
        }

        void recognizer_GestureCompleted(object sender, GestureEventArgs e)
        {
            MessageBox.Show("I got a gesture! " + e.Gesture.GetType().Name);
        }
    }
}
