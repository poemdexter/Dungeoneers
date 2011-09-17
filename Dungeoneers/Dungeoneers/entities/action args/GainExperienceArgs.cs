using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.action_args
{
    class GainExperienceArgs : ActionArgs
    {
        public int Amount { get; set; }

        public GainExperienceArgs(int amount)
        {
            this.Amount = amount;
        }
    }
}

