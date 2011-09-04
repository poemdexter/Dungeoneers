using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Dungeoneers.entities.components;
using Dungeoneers.entities.actions;
using Microsoft.Xna.Framework;

namespace Dungeoneers.dungeon
{
    class Dungeon
    {
        public int[][] floor { get; set; }
        public int[][] floorObjects { get; set; }
        private Random random { get; set; }
        public List<Entity> torchList;
        public List<Entity> doorList;
        private Dictionary<string, Texture2D> SpriteDict { get; set; }
        int dwidth = 64;
        int dheight = 48;

        public Vector2 stairsUp { get; set; }

        public EntityManager manager;

        public Dungeon(int seed, Dictionary<string, Texture2D> spriteDict)
        {
            this.random = new Random(seed);
            this.SpriteDict = spriteDict;
        }

        public int[][] createDungeon()
        {

            torchList = new List<Entity>();
            doorList = new List<Entity>();

            manager = new EntityManager();

            // initialize size of dungeon array and sets everything to unvisited (0)
            floor = new int[dwidth][];
            floorObjects = new int[dwidth][];
            for (int a = 0; a < floor.Length; a++)
            {
                floor[a] = new int[dheight];
                floorObjects[a] = new int[dheight];
            }

            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    floor[x][y] = 0;
                    floorObjects[x][y] = 0;
                }
            }

            // start with single node then recursively split into areas until done
            Node wholeDungeon = new Node(1, 1, dwidth - 2, dheight - 2, random);
            wholeDungeon.split(floor);
            // now add rooms
            wholeDungeon.addRooms(floor);
            // now connect the rooms
            wholeDungeon.connectRooms(floor);
            // now draw walls
            this.paintWalls();
            // now add doors
            this.addDoors();
            // now add torches
            this.addTorches();
            // now add upstairs;
            stairsUp = this.addStairs();

            return floor;
        }

        // add stairs up
        // *** stairs up is value 2 in floorObjects array ***
        public Vector2 addStairs()
        {
            int x = 1;
            int y = 1;
            bool found = false;
            Vector2 ret = Vector2.Zero;

            while (!found)
            {
                if (floor[x][y] == 1)
                {
                    floorObjects[x][y] = 2;
                    ret = new Vector2(x, y);
                    found = true;
                }
                else if (x < dwidth && x < dheight)
                {
                    x++;
                    y++;
                }
                else
                {
                    found = true;
                }
            }
            return ret;
        }

        // add doors to the rooms
        // *** doors are value 1 in floorObjects array ***
        public void addDoors()
        {
            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    if (floor[x][y] == 1)
                    {
                        // door faces: (N or S = 1), (W = 2), (E = 3) 
                        int dir = isValidDoorPlacement(x, y);
                        if (dir == 1)
                        {
                            Entity door = new Entity();
                            door.AddComponent(new Animation(SpriteDict["door_wood_ns"], 2, false, SpriteEffects.None));
                            door.AddAction(new NextFrameOfAnimation());
                            door.AddComponent(new Position(x, y));
                            door.AddComponent(new Openable());
                            door.AddAction(new ChangeStateOfOpenable());

                            manager.addDoor(door, new Vector2(x, y));

                            floorObjects[x][y] = 1;
                        }
                        else if (dir == 2)
                        {
                            Entity door = new Entity();
                            door.AddComponent(new Animation(SpriteDict["door_wood_we"], 2, false, SpriteEffects.FlipHorizontally));
                            door.AddAction(new NextFrameOfAnimation());
                            door.AddComponent(new Position(x, y));
                            door.AddComponent(new Openable());
                            door.AddAction(new ChangeStateOfOpenable());

                            manager.addDoor(door, new Vector2(x, y));

                            floorObjects[x][y] = 1;
                        }
                        else if (dir == 3)
                        {
                            Entity door = new Entity();
                            door.AddComponent(new Animation(SpriteDict["door_wood_we"], 2, false, SpriteEffects.None));
                            door.AddAction(new NextFrameOfAnimation());
                            door.AddComponent(new Position(x, y));
                            door.AddComponent(new Openable());
                            door.AddAction(new ChangeStateOfOpenable());

                            manager.addDoor(door, new Vector2(x, y));

                            floorObjects[x][y] = 1;
                        }
                    }
                }
            }
        }

        private int isValidDoorPlacement(int x, int y)
        {
            // checks in all cardinal directions rotated around d:
            // x   x
            // x   x
            // w d w
            //   x   
            int ret = -1;
            // check for proper walls
            if ((floor[x - 1][y] == 2 || floor[x - 1][y] == 3) && (floor[x + 1][y] == 2 || floor[x + 1][y] == 3))
            {
                // check facing north
                if (y - 2 >= 0) // don't check beyond top
                {
                    if ((floor[x - 1][y - 1] == 1 && floor[x - 1][y - 2] == 1) || // check  diagonal 2
                        (floor[x + 1][y - 1] == 1 && floor[x + 1][y - 2] == 1))   // check other diagonal 2
                    {
                        ret = 1;
                    }
                }
                // check facing south
                if (y + 2 <= floor[x].Length) // don't check beyond bottom
                {
                    if ((floor[x - 1][y + 1] == 1 && floor[x - 1][y + 2] == 1) || // check diagonal 2
                         (floor[x + 1][y + 1] == 1 && floor[x + 1][y + 2] == 1))   // check other diagonal 2
                    {
                        ret = 1;
                    }
                }
            }
            else if ((floor[x][y - 1] == 2 || floor[x][y - 1] == 3) && (floor[x][y + 1] == 2 || floor[x][y + 1] == 3))
            {
                // check facing west
                if (x + 2 <= floor.Length) // don't check beyond right
                {
                    if ((floor[x + 1][y + 1] == 1 && floor[x + 2][y + 1] == 1) || // check diagonal 2
                        (floor[x + 1][y - 1] == 1 && floor[x + 2][y - 1] == 1))   // check other diagonal 2
                    {
                        ret = 2;
                    }
                }
                // check facing east
                if (x - 2 >= 0) // don't check beyond left
                {
                    if ((floor[x - 1][y - 1] == 1 && floor[x - 2][y - 1] == 1) || // same as above
                        (floor[x - 1][y + 1] == 1 && floor[x - 2][y + 1] == 1))   // same as above
                    {
                        ret = 3;
                    }
                }
            }

            return ret;
        }

        // randomly add torches to the exposed walls
        public void addTorches()
        {
            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    // if a wall surrounded by walls
                    if (floor[x][y] == 2 &&
                        (floor[x - 1][y] == 2 || floor[x - 1][y] == 3) &&
                        (floor[x + 1][y] == 2 || floor[x + 1][y] == 3) &&
                        random.Next(100) < 15)
                    {
                        // create torch entity
                        Entity wallTorch = new Entity();
                        wallTorch.AddComponent(new Animation(SpriteDict["wall_torch"], 2, true, SpriteEffects.None));
                        wallTorch.AddAction(new NextFrameOfAnimation());
                        wallTorch.AddComponent(new Position(x, y));
                        torchList.Add(wallTorch);
                    }
                }
            }
        }

        // spruce up the place by drawing the walls
        public void paintWalls()
        {
            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    if (floor[x][y] == 0) // we're in black
                    {
                        if (y + 1 < floor[x].Length && floor[x][y + 1] == 1) // floor tile below us, draw exposed wall
                        {
                            floor[x][y] = 2;
                        }
                        else
                        {
                            // begin checks for the other 7 possible positions that are sides or bottom of floor tiles
                            //
                            //    Check order:
                            //    2 3 4
                            //    1 C 5
                            //    7 X 6

                            if (x - 1 > 0 && floor[x - 1][y] == 1)
                            {
                                floor[x][y] = 3;
                            }
                            else if (x - 1 > 0 && y - 1 > 0 && floor[x - 1][y - 1] == 1)
                            {
                                floor[x][y] = 3;
                            }
                            else if (y - 1 > 0 && floor[x][y - 1] == 1)
                            {
                                floor[x][y] = 3;
                            }
                            else if (x + 1 < floor.Length && y - 1 > 0 && floor[x + 1][y - 1] == 1)
                            {
                                floor[x][y] = 3;
                            }
                            else if (x + 1 < floor.Length && floor[x + 1][y] == 1)
                            {
                                floor[x][y] = 3;
                            }
                            else if (x + 1 < floor.Length && y + 1 < floor[x].Length && floor[x + 1][y + 1] == 1)
                            {
                                floor[x][y] = 3;
                            }
                            else if (x - 1 > 0 && y + 1 < floor[x].Length && floor[x - 1][y + 1] == 1)
                            {
                                floor[x][y] = 3;
                            }
                        }
                    }
                }
            }
        }
    }
}
