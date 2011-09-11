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

        public AIPoint(Vector2 pos, Vector2 prnt)
        {
            this.Position = pos;
            this.Parent = prnt;
        }
    }
}
