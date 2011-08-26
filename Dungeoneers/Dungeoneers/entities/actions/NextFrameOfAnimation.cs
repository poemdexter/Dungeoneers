using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;
using Microsoft.Xna.Framework;

namespace Dungeoneers.entities.actions
{
    class NextFrameOfAnimation : EntityAction
    {
        public NextFrameOfAnimation()
        {
            this.Name = "NextFrameOfAnimation";
        }

        public override void Do()
        {
            if (this.Entity != null)
            {
                Animation animation = this.Entity.GetComponent("Animation") as Animation;
                if (animation != null)
                {
                    if (animation.Looping)
                    {
                        animation.CurrentFrame++;
                        // if we have 2 frames, they count as 0 and 1.  2 is out of bounds.
                        if (animation.CurrentFrame == animation.FrameCount)
                            animation.CurrentFrame = 0;
                        animation.SourceRect = new Rectangle(animation.CurrentFrame * animation.FrameHeight, 0, animation.FrameHeight, animation.FrameHeight);
                    }
                    else
                    {
                        // make sure we haven't hit the end of animation
                        if (animation.CurrentFrame + 1 != animation.FrameCount)
                            animation.CurrentFrame++;
                        animation.SourceRect = new Rectangle(animation.CurrentFrame * animation.FrameHeight, 0, animation.FrameHeight, animation.FrameHeight);
                    }
                }
            }
        }
    }
}
