using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Position : Component
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Position(float x, float y)
        {
            this.Name = "Position";
            this.X = x;
            this.Y = y;
        }
    }
}
