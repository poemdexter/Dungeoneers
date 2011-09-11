using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.action_args;
using Dungeoneers.entities.components;
using Microsoft.Xna.Framework;
using Dungeoneers.dungeon;

namespace Dungeoneers.entities.actions
{
    class MoveTowardsPlayer : EntityAction
    {
        public MoveTowardsPlayer()
        {
            this.Name = "MoveTowardsPlayer";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is MoveTowardsPlayerArgs)
            {
                Position position = this.Entity.GetComponent("Position") as Position;
                MoveTowardsPlayerArgs arguments = args as MoveTowardsPlayerArgs;

                if (position != null)
                {
                    // gotta get A* path 
                    List<AIPoint> openList = new List<AIPoint>();
                    List<AIPoint> closedList = new List<AIPoint>();

                    AIPoint startPoint = new AIPoint(new Vector2(position.X, position.Y), new Vector2(position.X, position.Y));
                    openList.Add(startPoint);

                    if (arguments.floor[(int)position.X][(int)position.Y] == 1)
                    {
                        
                    }
                }
            }
        }
    }
}
