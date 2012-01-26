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

        void Deactivate();

        double GetTriggerScore(SkeletonData frame);

        bool IsOut(SkeletonData frame);

        bool IsComplete(SkeletonData frame);

        bool Active { get; }
    }
}
