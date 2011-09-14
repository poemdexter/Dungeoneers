using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Microsoft.Xna.Framework;

namespace Dungeoneers.entities.action_args
{
    class ChangePositionArgs : ActionArgs
    {
        public Vector2 Delta { get; set; }

        public ChangePositionArgs(Vector2 delta)
        {
            this.Delta = delta;
        }
    }
}
