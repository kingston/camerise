using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;

namespace SkeletalTracking
{
    class SkeletonController
    {
        //Scaling constants
        public float k_xMaxJointScale = 1.5f;
        public float k_yMaxJointScale = 1.5f;
        private FunControl window;

        public SkeletonController(FunControl win)
        {
            window = new FunControl(); 
        }

        //This function will be implemented by you in the subclass files provided.
        //A simple example of highlighting targets when hovered over has been provided below

        //Note: targets is a dictionary that allows you to retrieve the corresponding target on screen
        //and manipulate its state and position, as well as hide/show it (see class defn. below).
        //It is indexed from 1, thus you can retrieve an individual target with the expression
        //targets[3], which would retrieve the target labeled "3" on screen.
        public virtual void processSkeletonFrame(SkeletonData skeleton, Dictionary<int, Target> targets)
        {

            /*Example implementation*/

            foreach (var target in targets)
            {
                Target cur = target.Value;
                int targetID = cur.id; //ID in range [1..5]

                //Scale the joints to the size of the window
                Joint leftHand = skeleton.Joints[JointID.HandLeft].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
                Joint rightHand = skeleton.Joints[JointID.HandRight].ScaleTo(640, 480, window.k_xMaxJointScale, window.k_yMaxJointScale);
                
                //Calculate how far our left hand is from the target in both x and y directions
                double deltaX_left = Math.Abs(leftHand.Position.X - cur.getXPosition());
                double deltaY_left = Math.Abs(leftHand.Position.Y - cur.getYPosition());

                //Calculate how far our right hand is from the target in both x and y directions
                double deltaX_right = Math.Abs(rightHand.Position.X - cur.getXPosition());
                double deltaY_right = Math.Abs(rightHand.Position.Y - cur.getYPosition());

                //If we have a hit in a reasonable range, highlight the target
                if (deltaX_left < 15 && deltaY_left < 15 || deltaX_right < 15 && deltaY_right < 15)
                {
                    cur.setTargetSelected();
                }
                else
                {
                    cur.setTargetUnselected();
                }
            }

        }

        //This is called when the controller becomes active. This allows you to place your targets and do any 
        //initialization that you don't want to repeat with each new skeleton frame. You may also 
        //directly move the targets in the MainWindow.xaml file to achieve the same initial repositioning.
        public virtual void controllerActivated(Dictionary<int, Target> targets){
            //targets[1].setTargetPosition(80, 200);
            //targets[2].hideTarget();
            //targets[2].showTarget();
            //targets[5].isHidden();
            //targets[3].setTargetHighlighted();           
        }

        //The default value that gets passed to MaxSkeletonX and MaxSkeletonY in the Coding4Fun Joint.ScaleTo function is 1.5f
        //This function will change that so that your scaling in processSkeletonFrame aligns with the scaling done when we
        //position the ellipses in the MainWindow.xaml.cs file.
        public void adjustScale(float f)
        {
            window.k_xMaxJointScale = f;
            window.k_yMaxJointScale = f;
        }

    }
}
