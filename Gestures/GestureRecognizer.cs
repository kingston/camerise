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

        public delegate void GestureEventHandler(object sender, GestureEventArgs e);
        public event GestureEventHandler GestureStarted;
        public event GestureEventHandler GestureLeft;
        public event GestureEventHandler GestureCompleted;

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
            // Get first skeleton
            SkeletonData skeleton = (from s in frame.Skeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();

            if (activeGesture != null)
            {
                if (activeGesture.IsComplete(skeleton))
                {
                    GestureCompleted.Invoke(this, new GestureEventArgs(activeGesture));
                    activeGesture.Deactivate();
                    activeGesture = null;
                }
                else if (activeGesture.IsOut(skeleton))
                {
                    GestureLeft.Invoke(this, new GestureEventArgs(activeGesture));
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
                    double score = gesture.GetTriggerScore(skeleton);
                    if (score > Math.Max(maxScore, THRESHOLD))
                    {
                        maxScore = score;
                        maxGesture = gesture;
                    }
                }
                if (maxGesture != null)
                {
                    activeGesture = maxGesture;
                    GestureStarted.Invoke(this, new GestureEventArgs(activeGesture));
                    activeGesture.Activate();
                }
            }
        }
    }

    public class GestureEventArgs
    {
        public GestureEventArgs(IGesture gesture)
        {
            this.Gesture = gesture;
        }

        public IGesture Gesture { get; set; }
    }
}
