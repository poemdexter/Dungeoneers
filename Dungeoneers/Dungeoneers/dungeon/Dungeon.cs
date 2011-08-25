using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Dungeoneers.entities.components;
using Dungeoneers.entities.actions;

namespace Dungeoneers.dungeon
{
    class Dungeon
    {
        public int[][] floor { get; set; }
        private Random random { get; set; }
        public List<Entity> torchList;
        private Dictionary<string, Texture2D> SpriteDict { get; set; }

        public Dungeon(int seed, Dictionary<string, Texture2D> spriteDict)
        {
            this.random = new Random(seed);
            this.SpriteDict = spriteDict;
        }

        public int[][] createDungeon()
        {
            int dwidth = 32;
            int dheight = 32;
            torchList = new List<Entity>();

            // initialize size of dungeon array and sets everything to unvisited (0)
            floor = new int[dwidth][];
            for (int a = 0; a < floor.Length; a++)
            {
                floor[a] = new int[dheight];
            }

            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    floor[x][y] = 0;
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
            // now add torches
            // TODO: ADD DOORS FIRST THEN CHECK TO MAKE SURE TORCH ISN"T ABOVE DOOR
            this.addTorches();

            return floor;
        }

        // randomly add torches to the exposed walls
        public void addTorches()
        {
            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    if (floor[x][y] == 2 && random.Next(10) < 2)
                    {
                        // create torch entity
                        Entity wallTorch = new Entity();
                        wallTorch.AddComponent(new Animation(SpriteDict["wall_torch"], 2, true));
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
