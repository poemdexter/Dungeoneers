using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Experience : Component
    {
        public int Current_EXP { get; set; }
        public int Current_Level { get; set; }

        public Experience(int level, int exp)
        {
            this.Name = "Experience";
            this.Current_EXP = exp;
            this.Current_Level = level;
        }
    }
}
