using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking.Gestures
{
    class ThrustGesture : BaseGesture
    {
        public override void Activate()
        {
            Active = true;
        }

        public override void Deactivate()
        {
            Active = false;
        }

        public override double GetTriggerScore(SkeletonData skeleton)
        {
            Joint handRight = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480);
            return 0.0;
        }

        public override bool IsOut(SkeletonData skeleton)
        {
            return false;
        }

        public override bool IsComplete(SkeletonData skeleton)
        {
            return false;
        }
    }
}
