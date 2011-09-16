using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Mana : Component
    {
        public int Max_MP { get; set; }
        public int Current_MP { get; set; }

        public Mana(int amount)
        {
            this.Name = "Mana";
            this.Max_MP = amount;
            this.Current_MP = amount;
        }
    }
}

