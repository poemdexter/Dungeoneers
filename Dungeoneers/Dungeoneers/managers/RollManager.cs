﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeoneers.managers
{
    public static class RollManager
    {
        public static int roll(int dNum, int dSide, int mod)
        {
            int total = 0;
            for (int x = 1; x <= dNum; x++)
            {
                total += Meta.random.Next(1, dSide + 1);
            }
            total += mod;
            return total;
        }
    }
}
