using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace SkeletalTracking.Gestures
{
    public interface IGesture
    {
        void Activate();

        double GetTriggerScore(SkeletonFrame frame);

        bool IsOut(SkeletonFrame frame);

        bool IsComplete(SkeletonFrame frame);

        void Deactivate();

        bool Active { get; set; }
    }
}
