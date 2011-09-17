using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class LootTable : Component
    {
        public int ExpOnDeath { get; set; }

        public LootTable(int exp)
        {
            this.Name = "LootTable";
            this.ExpOnDeath = exp; 
        }
    }
}
