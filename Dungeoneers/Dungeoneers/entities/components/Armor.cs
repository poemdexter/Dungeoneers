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

        public Armor()
        {
            this.Name = "Armor";
            this.DmgReduction = 3;
        }
    }
}
