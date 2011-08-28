using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.action_args
{
    class ChangeDirectionOfAnimationArgs : ActionArgs
    {
        public string Direction { get; set; }

        public ChangeDirectionOfAnimationArgs(string direction)
        {
            this.Direction = direction;
        }
    }
}
