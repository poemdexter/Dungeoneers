using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.action_args
{
    class EquipArmorArgs : ActionArgs
    {
        public Armor Equipable { get; set; }
        public int Slot { get; set; }

        public EquipArmorArgs(Armor e, int slot)
        {
            this.Equipable = e;
            this.Slot = slot;
        }
    }
}
