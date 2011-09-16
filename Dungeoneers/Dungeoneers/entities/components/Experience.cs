using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Experience : Component
    {
        public int Next_Level { get; set; }  // amount to hit next level
        public int Current_EXP { get; set; }
        public int Current_Level { get; set; }

        public Experience(int next)
        {
            this.Name = "Experience";
            this.Next_Level = next;
            this.Current_EXP = 0;
            this.Current_Level = 1;
        }
    }
}
