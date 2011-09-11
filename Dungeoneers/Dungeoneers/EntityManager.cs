using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dungeoneers
{
    public class EntityManager
    {
        private Dictionary<Vector2, Entity> doorDict;

        public EntityManager()
        {
            doorDict = new Dictionary<Vector2, Entity>();

        }

        public void addDoor(Entity door, Vector2 position)
        {
            doorDict.Add(position, door);
        }

        public Entity getDoor(Vector2 position)
        {
            return doorDict[position];
        }

        public List<Entity> getDoorList()
        {
            return new List<Entity>(doorDict.Values);
        }
    }
}
