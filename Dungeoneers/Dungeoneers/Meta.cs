using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;

namespace Dungeoneers
{
    public static class Meta
    {
        public static Random random = new Random();
        //public static Random random = new Random(420);

        public static Dictionary<int, int> ExpLevel = new Dictionary<int, int>
        {
            {0,0},
            {1,100},
            {2,300},
            {3,600},
            {4,1000},
            {5,1500},
            {6,2100},
            {7,2800},
            {8,3600},
            {9,4500},
            {10,5500}
        };
    }

    public enum Tiles
    {
        Floor = 1,
        Wall_Exposed = 2,
        Wall_SideBottom = 3
    }

    public enum LootAffinity
    {
        Standard = 1,
        Fighter = 2,
        Caster = 3,
        Magical = 4
    }

    public enum Slots
    {
        MainHand = 1,
        OffHand = 2,
        Shield = 3,
        Chest = 4
    }

}
