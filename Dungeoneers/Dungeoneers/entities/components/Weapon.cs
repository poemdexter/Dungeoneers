using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Weapon : Component
    {
        public int Dice_Num { get; set; }
        public int Dice_Sides { get; set; }
        public int Roll_Mod { get; set; }
        public bool Melee {get; set; } // is a melee weapon?

        // 2d8 + 10 == adb + c
        public Weapon(int a, int b, int c, bool melee)
        {
            this.Dice_Num = a;
            this.Dice_Sides = b;
            this.Roll_Mod = c;
            this.Melee = melee;
        }
    }
}
