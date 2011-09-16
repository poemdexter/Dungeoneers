using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dungeoneers.entities.components;
using Dungeoneers.entities.actions;
using Microsoft.Xna.Framework.Graphics;
using Dungeoneers.framework;

namespace Dungeoneers.managers
{
    public class EntityManager
    {
        private Dictionary<Vector2, Entity> doorDict;
        private Dictionary<int, Entity> mobDict;
        private int key = 0;
        public Entity player { get; set; } // ID will be -1
        public Entity StairsUp { get; set; }
        public Entity StairsDown { get; set; }

        public EntityManager()
        {
            doorDict = new Dictionary<Vector2, Entity>();
            mobDict = new Dictionary<int, Entity>();
        }

        public int getNextKey()
        {
            int temp = key;
            key++;
            return temp;
        }

        public void addPlayer(Entity e)
        {
            this.player = e;
            this.player.Id = -1;
        }

        public Vector2 getPlayerPosition()
        {
            Position p = (Position)player.GetComponent("Position");
            return new Vector2(p.X, p.Y);
        }

        public bool canAttackPlayer(int ID)
        {
            Entity attacker = mobDict[ID];
            Position p = (Position)attacker.GetComponent("Position");
            Vector2 attPos = new Vector2(p.X, p.Y);
            Vector2 playerPos = getPlayerPosition();
            bool attackable = false;

            if ((attPos.X <= playerPos.X + 1 && attPos.X >= playerPos.X - 1) &&
                (attPos.Y <= playerPos.Y + 1 && attPos.Y >= playerPos.Y - 1))
            { attackable = true; }

            return attackable;
        }

        public void addStairsUp(Entity e)
        {
            this.StairsUp = e;
        }

        public void addStairsDown(Entity e)
        {
            this.StairsDown = e;
        }

        public void addMob(Component act, Component pos)
        {
            Entity skeleton = new Entity();
            skeleton.AddComponent((Animation)act);
            skeleton.AddComponent((Position)pos);
            skeleton.AddAction(new ChangeAbsPosition());
            skeleton.AddAction(new ChangeDirectionOfAnimation());
            skeleton.AddAction(new MoveTowardsPlayer());

            mobDict.Add(getNextKey(), skeleton);
        }

        public List<Entity> getMobList()
        {
            return new List<Entity>(mobDict.Values);
        }

        public void addDoor(Entity door, Vector2 position)
        {
            doorDict.Add(position, door);
        }

        public Entity getDoor(Vector2 position)
        {
            if (doorDict.ContainsKey(position))
                return doorDict[position];
            else
                return null;
        }

        public bool isDoorAt(Vector2 position)
        {
            if (doorDict.ContainsKey(position))
                return true;
            else
                return false;
        }

        public List<Entity> getDoorList()
        {
            return new List<Entity>(doorDict.Values);
        }

        // will return true if door is open or if door doesn't exist
        public bool isDoorOpen(Vector2 position)
        {
            if (doorDict.ContainsKey(position))
            {
                return ((Openable)doorDict[position].GetComponent("Openable")).Opened;
            }
            else
                return true;
        }
    }
}
