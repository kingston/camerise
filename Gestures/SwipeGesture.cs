using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking.Gestures
{
    class SwipeGesture : BaseGesture
    {
        public override double GetTriggerScore(SkeletonData skeleton)
        {
            Joint handRight = skeleton.Joints[JointID.HandRight];
            Joint elbowRight = skeleton.Joints[JointID.ElbowRight];
            Joint central = skeleton.Joints[JointID.Spine];

            Vector boundingBox = CreateVector(0.2f, 0.2f, 10.0f);
            Vector relativeToCentral = CreateVector(-0.3f, 0.6f, 0.3f);

            if (IsInBoundingBox(handRight.Position, central.Position, relativeToCentral, boundingBox))
            {
                return 1.0;
            }
            return 0.0;
        }

        public override bool IsOut(SkeletonData skeleton)
        {
            if (!Active) return false;
            return false;
        }

        public override bool IsComplete(SkeletonData skeleton)
        {
            if (!Active) return false;
            return true;
        }
    }
}
