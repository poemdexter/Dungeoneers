using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Position : Component
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            this.Name = "Position";
            this.X = x;
            this.Y = y;
        }
    }
}
