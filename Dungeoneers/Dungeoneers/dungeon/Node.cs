using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dungeoneers.dungeon
{
    public class Node
    {
        private static int ideal_room_size = 5;
        private static int max_room_size = 9;
        private static double max_partition_ratio = 1.0;
        static private double homogeneityFactor = .2;

        private int splitPos; // left (x) top (y) corner
        private Node leftChild, rightChild;
        private Random random;
        private string split_dir;
        private Rectangle Rect, roomRect;
        private bool roomBuilt;

        public Node(int left, int top, int width, int height, Random random)
        {
            this.random = random;
            this.roomBuilt = false;
            this.Rect = new Rectangle(left, top, width, height);
        }

        // high homo == sameness in rooms
        private double HomogenizedRandomValue()
        {
            return 0.5 - (random.NextDouble() * homogeneityFactor);
        }

        // see if child should split with small chance we won't split if decent just cuz
        private bool shouldSplit(int size)
        {
            if (size < (ideal_room_size * 2))
                return false;
            else
                return true;
        }

        public void split(int[][] dungeon)
        {
            // try to favor one way if ratio is way out of whack
            if (Rect.Width / Rect.Height > max_partition_ratio)
            {
                split_dir = "vert";
            }
            else if (Rect.Height / Rect.Width > max_partition_ratio)
            {
                split_dir = "horiz";
            }
            else split_dir = (random.Next(2) == 1) ? "vert" : "horiz";

            // split the node
            if (split_dir == "vert")
            {
                // get split position
                splitPos = Rect.Left + (int)(HomogenizedRandomValue() * Rect.Width);

                // create kiddos
                leftChild = new Node(Rect.Left, Rect.Top, splitPos - Rect.Left, Rect.Height, random);
                rightChild = new Node(splitPos + 1, Rect.Top, Rect.Left + Rect.Width - splitPos - 1, Rect.Height, random);

                // if not too small, split
                if (leftChild.shouldSplit(leftChild.Rect.Width))
                    leftChild.split(dungeon);
                if (rightChild.shouldSplit(rightChild.Rect.Width))
                    rightChild.split(dungeon);

            }
            else // horiz split
            {
                splitPos = Rect.Top + (int)(HomogenizedRandomValue() * Rect.Height);

                leftChild = new Node(Rect.Left, Rect.Top, Rect.Width, splitPos - Rect.Top, random);
                rightChild = new Node(Rect.Left, splitPos + 1, Rect.Width, Rect.Top + Rect.Height - splitPos - 1, random);

                if (leftChild.shouldSplit(leftChild.Rect.Height))
                    leftChild.split(dungeon);
                if (rightChild.shouldSplit(rightChild.Rect.Height))
                    rightChild.split(dungeon);
            }
        }

        public void addRooms(int[][] dungeon)
        {
            // recurse down to leaf nodes
            if (leftChild != null || rightChild != null)
            {
                if (leftChild != null)
                    leftChild.addRooms(dungeon);
                if (rightChild != null)
                    rightChild.addRooms(dungeon);
            }
            else // hit a leaf, make a room
            {
                int roomWidth = random.Next(Math.Min(ideal_room_size, Rect.Width), Math.Min(max_room_size, Rect.Width));
                int roomHeight = random.Next(Math.Min(ideal_room_size, Rect.Height), Math.Min(max_room_size, Rect.Height));
                int roomLeft = Rect.Left + (random.Next(Rect.Width - roomWidth));
                int roomTop = Rect.Top + (random.Next(Rect.Height - roomHeight));

                roomRect = new Rectangle(roomLeft, roomTop, roomWidth, roomHeight);
                roomBuilt = true;
                paintRoom(roomRect, dungeon);
            }
        }

        // fill room on dungeon with floor tiles
        public void paintRoom(Rectangle room, int[][] dungeon)
        {
            for (int x = room.Left; x < room.Left + room.Width; x++)
            {
                for (int y = room.Top; y < room.Top + room.Height; y++)
                {
                    dungeon[x][y] = 1;
                }
            }
        }

        public void connectRooms(int[][] dungeon)
        {
            Stack<Node> nodeStack = new Stack<Node>();
            Stack<Node> tempStack = new Stack<Node>();
            tempStack.Push(this);

            // build our breadth first stack
            while (tempStack.Count > 0)
            {
                Node tempNode = tempStack.Pop();
                nodeStack.Push(tempNode);
                if (tempNode.leftChild != null)
                    tempStack.Push(tempNode.leftChild);
                if (tempNode.rightChild != null)
                    tempStack.Push(tempNode.rightChild);
            }

            // build hallways
            while (nodeStack.Count > 0)
            {
                Node currentNode = nodeStack.Pop();

                if (currentNode.leftChild != null && currentNode.rightChild != null && (currentNode.leftChild.roomBuilt || currentNode.rightChild.roomBuilt))
                {

                    if (currentNode.leftChild == null || !currentNode.leftChild.roomBuilt)
                    {
                        currentNode.roomRect = currentNode.rightChild.roomRect;
                        currentNode.roomBuilt = true;
                    }
                    else if (currentNode.rightChild == null || !currentNode.rightChild.roomBuilt)
                    {
                        currentNode.roomRect = currentNode.leftChild.roomRect;
                        currentNode.roomBuilt = true;
                    }
                    else
                    {

                        // if the nodes overlap either horizontally or vertically by at least 3 tiles then
                        // we can join then with a straight hall else we'll need to use a L or Z shape hall
                        if (currentNode.split_dir == "vert")
                        {
                            int minOverlappingY = (int)MathHelper.Max(currentNode.leftChild.roomRect.Top, currentNode.rightChild.roomRect.Top);
                            int maxOverlappingY = (int)MathHelper.Min(currentNode.leftChild.roomRect.Bottom, currentNode.rightChild.roomRect.Bottom);

                            if (maxOverlappingY - minOverlappingY >= 3) // we can draw straight hall
                            {
                                // determine range of Y axis values we can use to dig and randomly pick one
                                int hallY = minOverlappingY + 1 + random.Next(maxOverlappingY - minOverlappingY - 2);

                                // start in middle of two leaves and go left and right until we hit another floor block
                                int a = 0;
                                bool hit = false;
                                while (!hit)
                                {
                                    if (dungeon[currentNode.splitPos + a][hallY] != 1)
                                    {
                                        dungeon[currentNode.splitPos + a][hallY] = 1;
                                        a++;
                                    }
                                    else hit = true;
                                }
                                a = 1;
                                hit = false;
                                while (!hit)
                                {
                                    if (dungeon[currentNode.splitPos - a][hallY] != 1)
                                    {
                                        dungeon[currentNode.splitPos - a][hallY] = 1;
                                        a++;
                                    }
                                    else hit = true;
                                }
                            }
                            else
                            {
                                // they don't line up so dig L shaped halls
                                connectWith_L_ShapedHall(currentNode, dungeon);

                            }
                        }
                        else if (currentNode.split_dir == "horiz")
                        {
                            int minOverlappingX = (int)MathHelper.Max(currentNode.leftChild.roomRect.Left, currentNode.rightChild.roomRect.Left);
                            int maxOverlappingX = (int)MathHelper.Min(currentNode.leftChild.roomRect.Right, currentNode.rightChild.roomRect.Right);

                            if (maxOverlappingX - minOverlappingX >= 3) // we can draw straight hall
                            {
                                // determine range of X axis values we can use to dig and randomly pick one
                                int hallX = minOverlappingX + 1 + random.Next(maxOverlappingX - minOverlappingX - 2);

                                // start in middle of two leaves and go up and down until we hit another floor block
                                int a = 0;
                                bool hit = false;
                                while (!hit)
                                {
                                    if (dungeon[hallX][currentNode.splitPos + a] != 1)
                                    {
                                        dungeon[hallX][currentNode.splitPos + a] = 1;
                                        a++;
                                    }
                                    else hit = true;
                                }
                                a = 1;
                                hit = false;
                                while (!hit)
                                {
                                    if (dungeon[hallX][currentNode.splitPos - a] != 1)
                                    {
                                        dungeon[hallX][currentNode.splitPos - a] = 1;
                                        a++;
                                    }
                                    else hit = true;
                                }
                            }
                            else
                            {
                                // they don't line up so dig L shaped halls
                                connectWith_L_ShapedHall(currentNode, dungeon);
                            }
                        }

                        // union 2 children room rectangles to form parent room rectangle
                        currentNode.roomRect = Rectangle.Union(currentNode.leftChild.roomRect, currentNode.rightChild.roomRect);
                        currentNode.roomBuilt = true;
                    }
                }
            }
        }

        // we can't draw a straight corridor so let's do an L shape
        public void connectWith_L_ShapedHall(Node currentNode, int[][] dungeon)
        {
            int meetX, meetY;
            Node tempL, tempR;
            bool isRight;

            //   L     R    L   R
            //    R   L    R     L
            //   T=   FS   F=   TS

            if (currentNode.leftChild.roomRect.Left < currentNode.rightChild.roomRect.Left)
            {
                if (currentNode.leftChild.roomRect.Top < currentNode.rightChild.roomRect.Top)
                {
                    isRight = true; // L above and left R
                    tempL = currentNode.leftChild;
                    tempR = currentNode.rightChild;
                }
                else
                {
                    isRight = false; // L below and left R
                    tempL = currentNode.rightChild;
                    tempR = currentNode.leftChild;
                }
            }
            else
            {
                if (currentNode.leftChild.roomRect.Top < currentNode.rightChild.roomRect.Top)
                {
                    isRight = false; // L above and right R
                    tempL = currentNode.leftChild;
                    tempR = currentNode.rightChild;
                }
                else
                {
                    isRight = true; // L below and right R
                    tempL = currentNode.rightChild;
                    tempR = currentNode.leftChild;
                }
            }

            if (currentNode.split_dir == "vert")  // down
            {
                if (!isRight)
                {
                    // up and left
                    //        _____
                    //        |   |
                    //        | L |
                    //        |___|
                    //  _____   |
                    //  |   |   |
                    //  | R |___|X
                    //  |___|
                    meetX = random.Next(Math.Max(tempR.roomRect.Right + 1, tempL.roomRect.Left - 1), tempL.roomRect.Right);
                    meetY = random.Next(Math.Max(tempL.roomRect.Bottom + 1, tempR.roomRect.Top + 1), tempR.roomRect.Bottom);

                    dungeon[meetX][meetY] = 1;

                    // go up
                    int a = 1;
                    bool hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX][meetY - a] != 1)
                        {
                            dungeon[meetX][meetY - a] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                    // go left
                    a = 1;
                    hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX - a][meetY] != 1)
                        {
                            dungeon[meetX - a][meetY] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                }
                else
                {
                    // up and right
                    //    _____
                    //    |   |
                    //    | L |
                    //    |___|
                    //      |    _____
                    //      |    |   |
                    //     X|____| R |
                    //           |___|
                    meetX = random.Next(tempL.roomRect.Left + 1, Math.Min(tempL.roomRect.Right, tempR.roomRect.Left - 1));
                    meetY = random.Next(Math.Max(tempL.roomRect.Bottom + 1, tempR.roomRect.Top + 1), tempR.roomRect.Bottom);

                    dungeon[meetX][meetY] = 1;

                    // go up
                    int a = 1;
                    bool hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX][meetY - a] != 1)
                        {
                            dungeon[meetX][meetY - a] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                    // go right
                    a = 1;
                    hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX + a][meetY] != 1)
                        {
                            dungeon[meetX + a][meetY] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                }
            }
            else // up
            {
                if (!isRight)
                {
                    // down and right
                    //        _____
                    //   X____|   |
                    //    |   | L |
                    //    |   |___|
                    //  __|__
                    //  |   |
                    //  | R |
                    //  |___|
                    meetX = random.Next(tempR.roomRect.Left + 1, Math.Min(tempR.roomRect.Right - 1, tempL.roomRect.Left));
                    meetY = random.Next(tempL.roomRect.Top + 1, tempL.roomRect.Bottom);

                    dungeon[meetX][meetY] = 1;

                    // go down
                    int a = 1;
                    bool hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX][meetY + a] != 1)
                        {
                            dungeon[meetX][meetY + a] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                    // go right
                    a = 1;
                    hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX + a][meetY] != 1)
                        {
                            dungeon[meetX + a][meetY] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                }
                else
                {
                    // down and left
                    //    _____
                    //    |   |____X
                    //    | L |   |
                    //    |___|   |
                    //          __|__
                    //          |   |
                    //          | R |
                    //          |___|
                    meetX = random.Next(tempR.roomRect.Left + 1, tempR.roomRect.Right);
                    meetY = random.Next(tempL.roomRect.Top + 1, Math.Min(tempL.roomRect.Bottom - 1, tempR.roomRect.Top));

                    dungeon[meetX][meetY] = 1;

                    // go down
                    int a = 1;
                    bool hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX][meetY + a] != 1)
                        {
                            dungeon[meetX][meetY + a] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                    // go left
                    a = 1;
                    hit = false;
                    while (!hit)
                    {
                        if (dungeon[meetX - a][meetY] != 1)
                        {
                            dungeon[meetX - a][meetY] = 1;
                            a++;
                        }
                        else hit = true;
                    }
                }
            }
        } 
    }
}
