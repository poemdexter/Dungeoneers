using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;
using Microsoft.Xna.Framework;

namespace Dungeoneers.entities.action_args
{
    class DropLootArgs : ActionArgs
    {
        public Dictionary<Vector2, List<Entity>> floorItems { get; set; }

        public DropLootArgs(Dictionary<Vector2, List<Entity>> items)
        {
            floorItems = items;
        }
    }
}
