using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.action_args
{
    class EquipWeaponArgs : ActionArgs
    {
        public Weapon Equipable { get; set; }
        public int Slot { get; set; }

        public EquipWeaponArgs(Weapon e, int slot)
        {
            this.Equipable = e;
            this.Slot = slot;
        }
    }
}
