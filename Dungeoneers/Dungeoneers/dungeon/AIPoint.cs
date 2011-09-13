using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dungeoneers.dungeon
{
    public class AIPoint
    {
        public Vector2 Position { get; set; }
        public Vector2 Parent { get; set; }
        public int G { get; set; } // cost to move to this square
        public int H { get; set; } // distance to goal
        public int F { get { return G + H; } } // sum of G and H

        public AIPoint(Vector2 pos, Vector2 prnt, int g, int h)
        {
            this.Position = pos;
            this.Parent = prnt;
            this.G = g;
            this.H = h;
        }
    }
}
