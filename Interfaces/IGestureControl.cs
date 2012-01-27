using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace SkeletalTracking.Interfaces
{
    interface IGestureControl
    {
        /// <summary>
        /// Called when the control is activated in the main window
        /// </summary>
        void activateControl();

        void processVideoFrame(ImageFrameReadyEventArgs e);

        void processSkeletonFrame(SkeletonFrame frame);

        event EventHandler OnTakePhotoActivated;
        event EventHandler OnLastPhotoActivated;
        event EventHandler OnSettingsActivated;
    }
}
