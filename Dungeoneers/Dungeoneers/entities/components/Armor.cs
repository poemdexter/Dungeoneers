using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Armor : Component
    {
        public int DmgReduction { get; set; }
        public string ArmorName { get; set; }

        public Armor(int value, string name)
        {
            this.Name = "Armor";
            this.DmgReduction = value;
            this.ArmorName = name;
        }
    }
}
