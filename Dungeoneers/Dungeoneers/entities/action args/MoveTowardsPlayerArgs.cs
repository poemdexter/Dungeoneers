using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.action_args
{
    class MoveTowardsPlayerArgs : ActionArgs
    {
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public int[][] floor { get; set; }

        public MoveTowardsPlayerArgs(float X, float Y, int[][] fl)
        {
            PlayerX = X;
            PlayerY = Y;
            floor = fl;
        }
    }
}
