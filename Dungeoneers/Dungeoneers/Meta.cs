using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeoneers
{
    public static class Rand
    {
        public static Random random = new Random();
    }

    public enum Tiles : int
    {
        Floor = 1,
        Wall_Exposed = 2,
        Wall_SideBottom = 3
    }

    public enum Slots
    {
        MainHand = 1,
        OffHand = 2,
        Shield = 3,
        Chest = 4
    }
}
