using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace SkeletalTracking.Gestures
{
    /**
     * Base class with some shared functionality
     */
    public abstract class BaseGesture : IGesture
    {
        public virtual void Activate()
        {
            Active = true;
        }

        public virtual void Deactivate()
        {
            Active = false;
        }

        public Boolean Active { get; protected set; }

        public abstract double GetTriggerScore(SkeletonData frame);

        public abstract bool IsOut(SkeletonData frame);

        public abstract bool IsComplete(SkeletonData frame);

        protected bool IsInBoundingBox(Vector pt, Vector referencePoint, Vector relativeCenter, Vector size)
        {
            Vector boxCenter = referencePoint.Add(relativeCenter);
            return (boxCenter.X - size.X / 2 < pt.X && pt.X < boxCenter.X + size.X / 2 &&
                    boxCenter.Y - size.Y / 2 < pt.Y && pt.Y < boxCenter.Y + size.Y / 2 &&
                    boxCenter.Z - size.Z / 2 < pt.Z && pt.Z < boxCenter.Z + size.Z / 2);
        }

        public static Vector CreateVector(float x, float y, float z)
        {
            return new Vector() { X = x, Y = y, Z = z };
        }
    }

    public static class GestureExtensionClass
    {
        public static Vector Add(this Vector vector1, Vector vector2)
        {
            return new Vector() { X = vector1.X + vector2.X, Y = vector1.Y + vector2.Y, Z = vector1.Z + vector1.Z };
        }
    }
}
