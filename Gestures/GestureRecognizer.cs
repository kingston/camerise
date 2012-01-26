using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace SkeletalTracking.Gestures
{
    public class GestureRecognizer
    {
        private const double THRESHOLD = 0.8;

        private List<IGesture> gestures;
        private IGesture activeGesture;

        public delegate void GestureRecognizedHandler(object sender, GestureRecognizedEventArgs e);
        public event GestureRecognizedHandler GestureRecognized;

        public GestureRecognizer()
        {
            gestures = new List<IGesture>();
        }

        public void AddGesture(IGesture gesture)
        {
            gestures.Add(gesture);
        }

        public void ProcessSkeleton(SkeletonFrame frame)
        {
            if (activeGesture != null)
            {
                if (activeGesture.IsComplete(frame))
                {
                    GestureRecognized.Invoke(this, new GestureRecognizedEventArgs(activeGesture));
                    activeGesture.Deactivate();
                    activeGesture = null;
                }
                else if (activeGesture.IsOut(frame))
                {
                    activeGesture.Deactivate();
                    activeGesture = null;
                }
            }
            if (activeGesture == null)
            {
                double maxScore = 0;
                IGesture maxGesture = null;
                foreach (IGesture gesture in gestures)
                {
                    double score = gesture.GetTriggerScore(frame);
                    if (score > Math.Max(maxScore, THRESHOLD))
                    {
                        maxScore = score;
                        maxGesture = gesture;
                    }
                }
                if (maxGesture != null)
                {
                    activeGesture = maxGesture;
                    activeGesture.Activate();
                }
            }
        }
    }

    public class GestureRecognizedEventArgs
    {
        public GestureRecognizedEventArgs(IGesture gesture)
        {
            this.Gesture = gesture;
        }

        public IGesture Gesture { get; set; }
    }
}
