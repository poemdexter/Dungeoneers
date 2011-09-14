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
                    Vector2 next = getNextPointToPlayer(position, arguments);
                    if (next != Vector2.Zero)
                    {
                        this.Entity.DoAction("ChangeAbsPosition", new ChangePositionArgs(next));
                    }
                }
            }
        }

        public Vector2 getNextPointToPlayer(Position pos, MoveTowardsPlayerArgs args)
        {
            // gotta get A* path to player if possible
            // http://www.policyalmanac.org/games/aStarTutorial.htm
            // following those steps, just going to comment step numbers for ease

            List<AIPoint> openList = new List<AIPoint>();
            List<AIPoint> closedList = new List<AIPoint>();

            // 1
            AIPoint startPoint = new AIPoint(new Vector2(pos.X, pos.Y), new Vector2(pos.X, pos.Y), 0, 0);
            openList.Add(startPoint);

            Vector2 player = new Vector2(args.PlayerX, args.PlayerY);
            bool found = false;

            while (openList.Count != 0 || !found)
            {
                // 4
                AIPoint parentAI = openList[0];
                openList.Remove(parentAI);
                closedList.Add(parentAI);

                // we found the player!
                if (parentAI.Position == player)
                {
                    found = true;
                    break;
                }

                Vector2 parent = new Vector2(parentAI.Position.X, parentAI.Position.Y);
                // 2
                // checking in this order
                // 2 3 4
                // 1 X 5
                // 8 7 6
                if (args.floor[(int)parentAI.Position.X - 1][(int)parentAI.Position.Y] == 1) // 1
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X - 1, parentAI.Position.Y);
                    if (!isInList(tempV, closedList)) // 5
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 10, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 10)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X - 1][(int)parentAI.Position.Y - 1] == 1) // 2
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X - 1, parentAI.Position.Y - 1);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 14, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 14)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X][(int)parentAI.Position.Y - 1] == 1) // 3
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X, parentAI.Position.Y - 1);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 10, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 10)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X + 1][(int)parentAI.Position.Y - 1] == 1) // 4
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X + 1, parentAI.Position.Y - 1);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 14, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 14)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X + 1][(int)parentAI.Position.Y] == 1) // 5
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X + 1, parentAI.Position.Y);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 10, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 10)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X + 1][(int)parentAI.Position.Y + 1] == 1) // 6
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X + 1, parentAI.Position.Y + 1);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 14, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 14)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X][(int)parentAI.Position.Y + 1] == 1) // 7
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X, parentAI.Position.Y + 1);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 10, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 10)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                if (args.floor[(int)parentAI.Position.X - 1][(int)parentAI.Position.Y + 1] == 1) // 8
                {
                    Vector2 tempV = new Vector2(parentAI.Position.X - 1, parentAI.Position.Y + 1);
                    if (!isInList(tempV, closedList))
                    {
                        if (!isInList(tempV, openList)) // 6
                        {
                            openList.Add(new AIPoint(tempV, parent, parentAI.G + 14, getG(tempV, player)));
                        }
                        else
                        {
                            AIPoint pathCheckPoint = getAIPoint(tempV, openList);
                            if (pathCheckPoint.G > parentAI.G + 14)
                            {
                                pathCheckPoint.Parent = parent;
                            }
                        }
                    }
                }

                // 3
                openList = openList.OrderBy(x => x.F).ToList();

            }

            // get path now.
            if (found)
            {
                // backtrack through closed list parents to get path.
                AIPoint start = getAIPoint(player, closedList);
                Stack<AIPoint> pointStack = new Stack<AIPoint>();
                bool pathing = true;
                while (pathing)
                {
                    if (start.Parent == startPoint.Position)
                    {
                        pathing = false;
                        return start.Position;
                        // move mob to start.position
                    }
                    else
                    {
                        pointStack.Push(start);
                        start = getAIPoint(start.Parent, closedList);
                    }
                }
            }
            return Vector2.Zero;
        }

        // grab the AIPoint based on vector
        public AIPoint getAIPoint(Vector2 v, List<AIPoint> list)
        {
            foreach (AIPoint point in list)
            {
                if (point.Position == v)
                    return point;
            }
            return null;
        }

        // checks if AIPoint exists in List by checking positions
        public bool isInList(Vector2 v, List<AIPoint> list)
        {
            foreach (AIPoint point in list)
            {
                if (point.Position == v)
                    return true;
            }
            return false;
        }

        // gets manhattan distance from mob to player
        public int getG(Vector2 mob, Vector2 player)
        {
            return (int)(Math.Abs(mob.X - player.X) + Math.Abs(mob.Y - player.Y));
        }
    }
}
