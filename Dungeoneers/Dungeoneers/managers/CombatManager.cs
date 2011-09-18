using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.entities.components;
using Dungeoneers.entities.action_args;

namespace Dungeoneers.managers
{
    static class CombatManager
    {
        public static void attack(Entity attacker, Entity target)
        {
            // calc dmg
            Weapon weapon = ((Equipment)attacker.GetComponent("Equipment")).MainHand;
            int predamage = RollManager.roll(weapon.Dice_Num, weapon.Dice_Sides, weapon.Roll_Mod);
            // subtract dmg reduction
            Armor armor = ((Equipment)target.GetComponent("Equipment")).Chest;

            int damage = predamage - armor.DmgReduction;
            // reduce hp and shoot messages

            Information targetInfo = target.GetComponent("Information") as Information;
            Information attackerInfo = attacker.GetComponent("Information") as Information;

            if (damage > 0)
            {
                target.DoAction("TakeDamage", new TakeDamageArgs(damage));
                string message = String.Format("{0} swings at {1} for {2} damage.", attackerInfo.Username, targetInfo.Username, damage);
                MessageManager.Instance.addMessage(message);
                if (!((Hitpoints)target.GetComponent("Hitpoints")).Alive)
                    MessageManager.Instance.addMessage(String.Format("{0} dies!", (target.GetComponent("Information") as Information).Username));
            }
            else 
            {
                string message = String.Format("{0} attacks {1}, but it glances off.", attackerInfo.Username, targetInfo.Username, damage);
                MessageManager.Instance.addMessage(message);
            }
        }
    }
}
