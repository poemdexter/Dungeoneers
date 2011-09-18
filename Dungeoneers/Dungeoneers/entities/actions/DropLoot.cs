using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.action_args;
using Dungeoneers.entities.components;
using Microsoft.Xna.Framework;
using Dungeoneers.managers;

namespace Dungeoneers.entities.actions
{
    class DropLoot : EntityAction
    {
        public DropLoot()
        {
            this.Name = "DropLoot";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is DropLootArgs)
            {
                LootTable lootTable = (LootTable)this.Entity.GetComponent("LootTable");
                Dictionary<Vector2, List<Entity>> floor = ((DropLootArgs)args).floorItems;

                // get items from lootTable and place them into floor
                List<Entity> loot = LootManager.getLootDrop(((Experience)this.Entity.GetComponent("Experience")).Current_Level, lootTable.Affinity);
            }
        }
    }
}
