using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Dungeoneers.entities.components;
using Dungeoneers.entities.actions;
using Microsoft.Xna.Framework.Graphics;
using Dungeoneers.framework;
using Dungeoneers.entities.action_args;

namespace Dungeoneers.managers
{
    public class EntityManager
    {
        private Dictionary<Vector2, Entity> doorDict;
        private Dictionary<int, Entity> mobDict;
        public Dictionary<Vector2, List<Entity>> floorItems;
        private int key = 0;
        public Entity player { get; set; } // ID will be -1
        public Entity StairsUp { get; set; }
        public Entity StairsDown { get; set; }

        public EntityManager()
        {
            doorDict = new Dictionary<Vector2, Entity>();
            mobDict = new Dictionary<int, Entity>();
            floorItems = new Dictionary<Vector2, List<Entity>>();
        }

        public int getNextKey()
        {
            int temp = key;
            key++;
            return temp;
        }

        public void attackPhase(Entity attacker, Entity target)
        {
            CombatManager.attack(attacker, target);

            if (!((Hitpoints)target.GetComponent("Hitpoints")).Alive)
            {
                if (target.Id == -1) // player dead
                {
                    // deal with player death
                }
                else
                {
                    // give exp
                    attacker.DoAction("GainExperience", new GainExperienceArgs((target.GetComponent("LootTable") as LootTable).ExpOnDeath));
                    // drop loot
                    target.DoAction("DropLoot", new DropLootArgs(floorItems));
                }
            }
        }

        public bool isMobAliveInList(int ID)
        {
            if ((mobDict[ID].GetComponent("Hitpoints") as Hitpoints).Alive)
                return true;
            else
                return false;
        }

        public bool isMobAliveAtPos(Vector2 pos)
        {
            foreach (Entity e in getMobList())
            {
                if (((Position)e.GetComponent("Position")).X == pos.X &&
                    ((Position)e.GetComponent("Position")).Y == pos.Y)
                {
                    if (((Hitpoints)e.GetComponent("Hitpoints")).Alive)
                        return true;
                }
            }
            return false;
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

        public void addMob(Entity mob)
        {
            mobDict.Add(getNextKey(), mob);
        }

        public List<Entity> getMobList()
        {
            return new List<Entity>(mobDict.Values);
        }

        public bool isMobAt(Vector2 pos)
        {
            foreach (Entity mob in getMobList())
            {
                Position position = mob.GetComponent("Position") as Position;
                if (position.X == pos.X && position.Y == pos.Y)
                    return true;
            }
            return false;
        }

        public Entity getMobAt(Vector2 pos)
        {
            foreach (Entity mob in getMobList())
            {
                Position position = mob.GetComponent("Position") as Position;
                if (position.X == pos.X && position.Y == pos.Y)
                    return mob;
            }
            return null;
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
