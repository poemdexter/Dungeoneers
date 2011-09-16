using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.action_args;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.actions
{
    class EquipItem : EntityAction
    {
        EquipWeaponArgs weaponArgs;
        EquipArmorArgs armorArgs;

        public EquipItem()
        {
            this.Name = "EquipItem";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && (args is EquipWeaponArgs || args is EquipArmorArgs))
            {
                bool isWeapon = false;
                if (args is EquipWeaponArgs)
                {
                    weaponArgs = args as EquipWeaponArgs;
                    isWeapon = true;
                }
                else if (args is EquipArmorArgs)
                    armorArgs = args as EquipArmorArgs;

                Equipment equipment = this.Entity.GetComponent("Equipment") as Equipment;

                if (isWeapon)
                {
                    switch (weaponArgs.Slot)
                    {
                        case (int)Slots.MainHand:
                            equipment.MainHand = weaponArgs.Equipable;
                            break;
                        default: break;
                    }
                }
                else
                {
                    switch (armorArgs.Slot)
                    {
                        case (int)Slots.Chest:
                            equipment.Chest = armorArgs.Equipable;
                            break;
                        default: break;
                    }
                }
            }
        }
    }
}
