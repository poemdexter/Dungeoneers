using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.managers
{
    static class InspectManager
    {
        public static Vector2 Position { get; set; }

        public static void resetInspection(Entity player)
        {
            int x = (int)((Position)player.GetComponent("Position")).X;
            int y = (int)((Position)player.GetComponent("Position")).Y;
            Position = new Vector2(x, y);
        }

        public static void inspect(EntityManager manager)
        {
            Entity entity = manager.getAliveMobAt(Position);
            if (entity != null)
            {
                // do display mob info
            }

            List<Entity> entities = manager.floorItems[Position];
            if (entities != null && entities.Count > 0)
            {
                // do display floor items
            }
        }
    }
}
