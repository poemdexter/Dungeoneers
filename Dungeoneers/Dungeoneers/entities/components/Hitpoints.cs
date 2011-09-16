using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Hitpoints : Component
    {
        public int Max_HP { get; set; }
        public int Current_HP { get; set; }

        public Hitpoints(int amount)
        {
            this.Name = "Hitpoints";
            this.Max_HP = amount;
            this.Current_HP = amount;
        }
    }
}
