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
        public LootAffinity Affinity { get; set; }

        public LootTable(int exp, LootAffinity affinity)
        {
            this.Name = "LootTable";
            this.ExpOnDeath = exp;
            this.Affinity = affinity;
        }
    }
}
