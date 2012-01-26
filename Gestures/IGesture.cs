using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace SkeletalTracking.Gestures
{
    /// <summary>
    /// Provides an interface for creating different gestures
    /// </summary>
    public interface IGesture
    {
        /// <summary>
        /// Called when the gesture is to be activated (usually when the trigger score
        /// is above some threshold, i.e. at start of gesture)
        /// </summary>
        void Activate();

        /// <summary>
        /// Called when the gesture is to be deactivated (usually when the skeleton is
        /// out of the active zone or the gesture is completed and thus needs to be cancelled)
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Gets the score (between 0 and 1) of the likelihood that the current skeleton is
        /// within the trigger zone.  It's used to work out when to start the gesture.
        /// </summary>
        /// <param name="frame">The skeleton frame to process</param>
        /// <returns>A double between 0 and 1 of the likelihood that the skeleton should trigger the gesture</returns>
        double GetTriggerScore(SkeletonData frame);

        /// <summary>
        /// Tests whether the gesture should be ended without completion
        /// because it is now outside the active zone.
        /// </summary>
        /// <param name="frame">The skeleton frame to process</param>
        /// <returns>Whether the skeleton is outside the active zone of the current gesture</returns>
        bool IsOut(SkeletonData frame);

        /// <summary>
        /// Tests whether the gesture should be marked as complete.
        /// </summary>
        /// <param name="frame">The skeleton frame to process</param>
        /// <returns>Whether the skeleton is in the "end-state" of the gesture</returns>
        bool IsComplete(SkeletonData frame);

        /// <summary>
        /// Gets whether the gesture is currently active, i.e. we are in the middle of this gesture
        /// </summary>
        bool Active { get; }
    }
}
