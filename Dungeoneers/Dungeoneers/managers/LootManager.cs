using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.managers
{
    static class LootManager
    {
        public static List<Entity> getLootDrop(int level, LootAffinity affinity)
        {
            List<Entity> drops = new List<Entity>();

            switch (affinity)
            {
                case LootAffinity.Standard:
                    int y = Meta.random.Next(1, 4);
                    // TODO Make weapons and armor an entity
                    // drops.Add(Weapons[y]);
                    break;
                case LootAffinity.Fighter:
                    break;
                case LootAffinity.Caster:
                    break;
                case LootAffinity.Magical:
                    break;
            }

            return drops;
        }

        public static Dictionary<int, Weapon> Weapons = new Dictionary<int, Weapon>
        {
            {1, new Weapon(1,4,0,true,"dagger")},
            {2, new Weapon(1,5,0,true,"staff")},
            {3, new Weapon(1,6,0,true,"sword")}
        };

        public static Dictionary<int, Armor> Armor = new Dictionary<int, Armor>
        {
            {1, new Armor(1, "robe")},
            {2, new Armor(3, "leather armor")},
            {3, new Armor(5, "chainmail")}
        };
    }


}
