using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking.Gestures
{
    class ShutterGesture : BaseGesture
    {

        public override double GetTriggerScore(SkeletonData skeleton)
        {
            Joint handRight = skeleton.Joints[JointID.HandRight];
            Joint elbowRight = skeleton.Joints[JointID.ElbowRight];
            Joint shoulderRight = skeleton.Joints[JointID.ShoulderRight];

            Joint handLeft = skeleton.Joints[JointID.HandLeft];
            Joint elbowLeft = skeleton.Joints[JointID.ElbowLeft];
            Joint shoulderLeft = skeleton.Joints[JointID.ShoulderLeft];

            //Trigger:
            // elbow-shoulder y is close, elbow x is outside shoulder x
            // hand-elbow z is close, hand-elbow x is close, hand is above elbow

            Vector boundingBox = CreateVector(0.2f, 0.2f, 10.0f);
            Vector relativeToCentral = CreateVector(0, 0, 0);

            if (IsInBoundingBox(elbowRight.Position, shoulderRight.Position, relativeToCentral, boundingBox) &&
                IsInBoundingBox(handRight.Position, elbowRight.Position, relativeToCentral, boundingBox))
            {
                return true;
            }
            return false;
        }

        public override bool IsOut(SkeletonData skeleton)
        {
            //Out:
            // elbow-shoulder y is not close
            // (hand-elbow z is not close, hand-elbow x is not close,) hand is not above elbow
            return false;
        }

        public override bool IsComplete(SkeletonData skeleton)
        {
            // Success: close then open "shutter"
            // Close shutter:
            //
            return false;
        }
    }
}
