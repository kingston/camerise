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
            startGestureTime = DateTime.Now;
            Active = true;
        }

        public override void Deactivate()
        {
            Active = false;
        }

        /// <summary>
        /// Time to hold the gesture until it gets completed (in milliseconds)
        /// </summary>
        private const int HOLD_TIME = 1000;
        private DateTime startGestureTime;

        /// <summary>
        /// True if the gesture was completed but the hand never went back down (to avoid duplicate gestures from same motion)
        /// </summary>
        private bool hasGestureEscaped = true;

        private bool IsRightArmThrusting(SkeletonData skeleton)
        {
            Joint handRight = skeleton.Joints[JointID.HandRight];
            Joint elbowRight = skeleton.Joints[JointID.ElbowRight];
            Joint shoulderRight = skeleton.Joints[JointID.ShoulderRight];

            Vector boundingBox = CreateVector(0.2f, 0.2f, 50.0f);
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
            if (IsRightArmThrusting(skeleton) && hasGestureEscaped)
            {
                return 1.0;
            }
            hasGestureEscaped = true;
            return 0.0;
        }

        public override bool IsOut(SkeletonData skeleton)
        {
            return !IsRightArmThrusting(skeleton);
        }

        public override bool IsComplete(SkeletonData skeleton)
        {
            if (DateTime.Now.Subtract(this.startGestureTime).TotalMilliseconds > HOLD_TIME)
            {
                hasGestureEscaped = false;
                return true;
            }
            return false;
        }
    }
}
