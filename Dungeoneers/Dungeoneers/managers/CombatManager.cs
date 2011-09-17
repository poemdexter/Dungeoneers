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
            // reduce hp
            if (damage > 0)
            {
                target.DoAction("TakeDamage", new TakeDamageArgs(damage));

                Console.WriteLine("swung for {0}, hit for {1}.", predamage, damage);
                Console.WriteLine("entity has {0} HP left.", ((Hitpoints)target.GetComponent("Hitpoints")).Current_HP);
            }
            else 
            {
                Console.WriteLine("swung for {0} but glanced", predamage);
                Console.WriteLine("entity has {0} HP left.", ((Hitpoints)target.GetComponent("Hitpoints")).Current_HP);
            }
        }
    }
}
