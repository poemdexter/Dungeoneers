using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;
using Dungeoneers.entities.action_args;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeoneers.entities.actions
{
    class ChangeDirectionOfAnimation : EntityAction
    {
        public ChangeDirectionOfAnimation()
        {
            this.Name = "ChangeDirectionOfAnimation";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is ChangeDirectionOfAnimationArgs)
            {
                Animation animation = this.Entity.GetComponent("Animation") as Animation;
                if (animation != null)
                {
                    string direction = ((ChangeDirectionOfAnimationArgs)args).Direction;

                    if (direction == "left")
                    {
                        animation.Effects = SpriteEffects.FlipHorizontally;
                    }
                    else if (direction == "right")
                    {
                        animation.Effects = SpriteEffects.None;
                    }
                }
            }
        }
    }
}
