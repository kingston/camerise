using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace SkeletalTracking.Interfaces
{
    interface IGestureControl
    {
        void activateControl();

        void processVideoFrame(ImageFrameReadyEventArgs e);

        void processSkeletonFrame(SkeletonFrame frame);
    }
}
