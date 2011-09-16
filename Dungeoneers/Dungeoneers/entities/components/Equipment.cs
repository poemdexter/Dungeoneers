using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Equipment : Component
    {
        public Weapon MainHand { get; set; }
        public Armor Chest { get; set; }
        public Weapon Ranged { get; set; }

        public Equipment()
        {
            this.Name = "Equipment";
        }
    }
}
