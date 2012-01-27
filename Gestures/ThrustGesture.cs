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

        private int HoldingFrames;
        private int N_HOLD_FRAMES = 30;
        private bool WasHolding = false;

        private bool IsRightArmThrusting(SkeletonData skeleton)
        {
            Joint handRight = skeleton.Joints[JointID.HandRight];
            Joint elbowRight = skeleton.Joints[JointID.ElbowRight];
            Joint shoulderRight = skeleton.Joints[JointID.ShoulderRight];

            Vector boundingBox = CreateVector(0.1f, 0.1f, 10.0f);
            Vector relativeToCentral = CreateVector(0, 0, 0);

            if (IsInBoundingBox(elbowRight.Position, shoulderRight.Position, relativeToCentral, boundingBox) &&
                IsInBoundingBox(handRight.Position, elbowRight.Position, relativeToCentral, boundingBox))
            {
                return true;
            }
            return false;
        }

        public override double GetTriggerScore(SkeletonData skeleton)
        {
            if (IsRightArmThrusting(skeleton) && !WasHolding)
            {
                WasHolding = true;
                HoldingFrames = 0;
                return 1.0;
            }
            return 0.0;
        }

        public override bool IsOut(SkeletonData skeleton)
        {
            if (IsRightArmThrusting(skeleton))
            {
                return false;
            }
            WasHolding = false;
            return true;
        }

        public override bool IsComplete(SkeletonData skeleton)
        {
            HoldingFrames++;
            if (HoldingFrames >= N_HOLD_FRAMES)
            {
                return true;
            }
            return false;
        }
    }
}
