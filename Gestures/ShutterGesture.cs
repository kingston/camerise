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
        private bool shutterWasClosed = false;

        private List<Joint> GetShoulders(SkeletonData skeleton)
        {
            List<Joint> shoulders = new List<Joint>(2);
            shoulders.Add(skeleton.Joints[JointID.ShoulderLeft]);
            shoulders.Add(skeleton.Joints[JointID.ShoulderRight]);
            return shoulders;
        }

        private List<Joint> GetElbows(SkeletonData skeleton)
        {
            List<Joint> elbows = new List<Joint>(2);
            elbows.Add(skeleton.Joints[JointID.ElbowLeft]);
            elbows.Add(skeleton.Joints[JointID.ElbowRight]);
            return elbows;
        }

        private List<Joint> GetHands(SkeletonData skeleton)
        {
            List<Joint> hands = new List<Joint>(2);
            hands.Add(skeleton.Joints[JointID.HandLeft]);
            hands.Add(skeleton.Joints[JointID.HandRight]);
            return hands;
        }
        
        private bool AreUpperArmsHorizontal(List<Joint> elbows, List<Joint> shoulders)
        {
            for (int i = 0; i < 2; i++) 
            {
                if (Math.Abs(elbows[i].Position.Y - shoulders[i].Position.Y) >= 0.2) {
                    return false;
                }
            }
            return true;
        }

        private bool AreArmsOpen(List<Joint> elbows, List<Joint> shoulders)
        {
            if ((elbows[0].Position.X < shoulders[0].Position.X) &&
                (elbows[1].Position.X > shoulders[1].Position.X))
            {
                return true;
            }
            return false;
        }

        private bool AreArmsClosed(List<Joint> elbows, List<Joint> shoulders)
        {
            if ((elbows[0].Position.X > shoulders[0].Position.X) &&
                (elbows[1].Position.X < shoulders[1].Position.X))
            {
                return true;
            }
            return false;
        }
        
        private bool AreLowerArmsVertical(List<Joint> elbows, List<Joint> hands)
        {
            for (int i = 0; i < 2; i++) 
            {
                if ((Math.Abs(elbows[i].Position.Z - hands[i].Position.Z) >= 0.2) ||
                    (Math.Abs(elbows[i].Position.X - hands[i].Position.X) >= 0.2)) {
                    return false;
                }
            }
            return true;
        }

        private bool AreHandsAboveElbows(List<Joint> elbows, List<Joint> hands)
        {
            for (int i = 0; i < 2; i++) 
            {
                if (hands[i].Position.Y <= elbows[i].Position.Y) {
                    return false;
                }
            }
            return true;
        }

        public override double GetTriggerScore(SkeletonData skeleton)
        {
            //Trigger:
            // elbow-shoulder y is close, elbow x is outside shoulder x
            // hand-elbow z is close, hand-elbow x is close, hand is above elbow
            
            List<Joint> shoulders = GetShoulders(skeleton);
            List<Joint> elbows = GetElbows(skeleton);
            List<Joint> hands = GetHands(skeleton);

            if (AreUpperArmsHorizontal(elbows, shoulders) && AreArmsOpen(elbows, shoulders) && 
                AreLowerArmsVertical(elbows, hands) && AreHandsAboveElbows(elbows, hands))
            {
                shutterWasClosed = false;
                return 1.0;
            }
            return 0.0;
        }

        public override bool IsOut(SkeletonData skeleton)
        {
            //Out:
            // elbow-shoulder y is not close
            // (hand-elbow z is not close, hand-elbow x is not close,) hand is not above elbow

            List<Joint> shoulders = GetShoulders(skeleton);
            List<Joint> elbows = GetElbows(skeleton);
            List<Joint> hands = GetHands(skeleton);

            if (!AreUpperArmsHorizontal(elbows, shoulders) || !AreHandsAboveElbows(elbows, hands))
            {
                return true;
            }
            return false;
        }

        public override bool IsComplete(SkeletonData skeleton)
        {
            //Success: close then open "shutter"
            //Close shutter:
            // elbow-shoulder y is close, elbow x is inside shoulder x
            // hand-elbow z is close, hand-elbow x is close, hand is above elbow
            //Open shutter:
            // elbow-shoulder y is close, elbow x is outside shoulder x
            // hand-elbow z is close, hand-elbow x is close, hand is above elbow

            List<Joint> shoulders = GetShoulders(skeleton);
            List<Joint> elbows = GetElbows(skeleton);
            List<Joint> hands = GetHands(skeleton);

            if (!shutterWasClosed)
            {
                // Look for shutter closing
                if (AreUpperArmsHorizontal(elbows, shoulders) && AreArmsClosed(elbows, shoulders) &&
                    AreLowerArmsVertical(elbows, hands) && AreHandsAboveElbows(elbows, hands))
                {
                    shutterWasClosed = true;
                }
                return false;
            }
            else
            {
                // Look for shutter opening
                if (AreUpperArmsHorizontal(elbows, shoulders) && AreArmsOpen(elbows, shoulders) &&
                    AreLowerArmsVertical(elbows, hands) && AreHandsAboveElbows(elbows, hands))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
