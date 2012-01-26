using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking.Gestures
{
    class SwipeGesture : IGesture
    {
        public void Activate()
        {
            this.Active = true;
        }

        public void Deactivate()
        {
            this.Active = false;
        }

        public double GetTriggerScore(SkeletonData skeleton)
        {
            Joint handRight = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480);
            return 0.0;
        }

        public bool IsOut(SkeletonData skeleton)
        {
            return false;
        }

        public bool IsComplete(SkeletonData skeleton)
        {
            return false;
        }

        public bool Active { get; private set; }
    }
}
