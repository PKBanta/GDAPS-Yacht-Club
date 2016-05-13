using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class MapReader
    {
        //fields
        int roomNumber = 1;
        int x;
        int y;
        int count;
        bool up;
        bool down;
        bool left;
        bool right;
        char transRectChar;
        char[] charList;
        string line;
        Room room;
        Wall wall;
        Platform platform;
        Collectible item;
        SpriteBatch spritebatch;
        public static Random rando;// = new Random();
        char[,] tileArray;
        List<Rectangle> rectList;
        List<Collectible> itemList;
        List<MapObject> objList;
        List<Enemy> enemyList;
        QuadTreeNode quadtree;
        Rectangle previousTransitionRect;
        Rectangle transitionRect;
        public static Texture2D healthBarBackground, healthBarOverlay;
        //private LevelType levelType;
        //public static Background cityBG, sewerBG, skyscraperBG;


        /// <summary>
        /// returns the list of rectangles of all the platforms in a room
        /// </summary>
        public List<Rectangle> RectList
        {
            get { return rectList; }

        }
        public List<Collectible> ItemList
        {
            get { return itemList; }
        }

        public List<MapObject> ObjList
        {
            get { return objList; }
        }
        public List<Enemy> EnemyList
        {
            get { return enemyList; }
        }
        public int RoomNumber
        {
            get { return roomNumber; }
        }
        public bool Up
        {
            get { return up; }
        }
        public bool Down
        {
            get { return down; }
        }
        public bool Left
        {
            get { return left; }
        }
        public bool Right
        {
            get { return right; }
        }
        public Room CurrentRoom
        {
            get { return room; }
        }
        

        /// <summary>
        /// pure voodoo magic. might be dangerous
        /// </summary>
        /// <param name="name">name of the text file</param>
        /// <param name="plat">platform object for this particular room</param>
        /// <param name="wa">wall object for this particular room</param>
        /// <param name="i">arrray of items that can potentially be found in this room</param>
        /// <param name="batch">spritebatch</param>
        public void ReadMap(string name, QuadTreeNode qt,Texture2D collectText,Texture2D enemyText)
        {
            quadtree = qt;
            Stream instream;
            StreamReader input = null;
            roomNumber++;
            count = 0;

            previousTransitionRect = transitionRect;
            

            try
            {
                instream = File.OpenRead(name);
                input = new StreamReader(instream);

                //reads the first line, parses the info to the correct variables
                line = input.ReadLine();
                string[] newLine = line.Split(',');
                x = int.Parse(newLine[0]);
                y = int.Parse(newLine[1]);
                charList = newLine[2].ToCharArray();
                up = (charList[0].Equals('1'));
                left = (charList[1].Equals('1'));
                down = (charList[2].Equals('1'));
                right = (charList[3].Equals('1'));
                transRectChar = (newLine[3].ToCharArray()[0]);
                tileArray = new char[x, y];
                //sets up a new room with the input
                room = new Room(x, y, up, down, left, right);
                rectList = new List<Rectangle>();
                objList = new List<MapObject>();
                itemList = new List<Collectible>();
                enemyList = new List<Enemy>();
                Rectangle leftrect = new Rectangle(0, 0, 50, 800);
                Rectangle rightrect = new Rectangle(1150, 0, 50, 800);
                Rectangle uprect = new Rectangle(0, 0, 1200, 50);
                Rectangle downrect = new Rectangle(0, 750, 1200, 50);

                //finds out where to go to load the next map
                if (transRectChar == 'd')
                {
                    transitionRect = downrect;
                }
                else if (transRectChar == 'u')
                {
                    transitionRect = uprect;
                }
                else if (transRectChar == 'r')
                {
                    transitionRect = rightrect;
                }

                //saves the actual room into the character array
                while ((line = input.ReadLine()) != null)
                {

                    charList = line.ToCharArray();

                    for (int n = 0; n < y; n++)
                    {
                        tileArray[count, n] = charList[n];
                        
                        if (tileArray[count, n] == '#')
                        {
                            rectList.Add(new Rectangle(count * 25, n * 25, 25, 25));
                        }
                        if(tileArray[count,n] == '*')
                        {
                            itemList.Add(new Collectible(count * 25, n * 25, 25, 25,collectText, "item"));
                        }
                        if(tileArray [count,n] == 'e')
                        {
                            //need stats on enemies
                            enemyList.Add(new Zombie(
                                new Vector2(count * 25, n * 25),
                                healthBarBackground,
                                healthBarOverlay,
                                Zombie.ZOM_HEALTH + rando.Next(3)));
                        }
                        
                    }
                    count++;

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (input != null)
                    input.Close();
            }
        }
        
        /*public void StoreObjects(Platform plat, Wall wa, Collectible[] i, SpriteBatch batch)
        {
            for (int h = 0; h < y; h++)
            {
                for (int n = 0; n < x; n++)
                {
                    
                    if (tileArray[n, h] == '#')
                    {
                        objList.Add(new Platform(n * 25, h * 25, 25, 25, plat.Tex));
                        quadtree.AddObject(new Platform(n * 25, h * 25, 25, 25, plat.Tex));
                    }
                    else if (tileArray[n, h] == '*')
                    {
                        objList.Add(new Collectible(n * 25, h * 25, 25, 25, i[0].Tex,""));
                        quadtree.AddObject(new Collectible(n * 25, h * 25, 25, 25, i[0].Tex, ""));
                    }
                    if (tileArray[n,h] == '#')
                    {
                        rectList.Add(new Rectangle(count * 25, n * 25, 25, 25));
                    }
                }
            }
        }
        */
    
        public void DrawMap( Platform plat, Wall wa, Collectible i, SpriteBatch batch)
        {
            spritebatch = batch;
            platform = plat;
            wall = wa;
            item = i;
            count = 0;

            room.TileXToZero();
            room.TileYToZero();

            int enemyCount = enemyList.Count;
            int collectCount = itemList.Count;

            //makes an upper wall if necessary
            if (up)
            {
                for (int n = 0; n < x; n++)
                {
                    wall.Rect = room.Tile;
                    wall.Draw(spritebatch);
                    room.IncrementTileX();
                }
            }
            for(int h = 0; h< y-1; h++)
            {
                for (int n = 0; n < x; n++)
                {
                    if (n == 0 && left)
                    {
                        wall.Rect = room.Tile;
                        wall.Draw(spritebatch);
                        room.IncrementTileX();
                    }

                    else if (tileArray[n, h] == '#')
                    {
                        platform.Rect = room.Tile;
                        platform.Draw(spritebatch);
                        room.IncrementTileX();
                    }
                    
                    else if (tileArray[n, h] == '*')
                    {
                        if(collectCount > 0)
                        {
                            itemList[collectCount-1].Draw(spritebatch);
                            collectCount--;
                        }
                       
                        room.IncrementTileX();
                    }
                    
                    else if(tileArray[n,h] == 'e')
                    {
                        if(enemyCount > 0)
                        {
                            enemyList[enemyCount-1].Draw(spritebatch);
                            enemyCount--;
                        }
                        
                        room.IncrementTileX();
                    }
                    
                    else if (tileArray[n,h] == '=')
                    {
                        room.IncrementTileX();
                    }
                    else if (n==x-1 && !right)
                    {
                        room.TileXToZero();
                        room.IncrementTileY();
                    }
                    else if (n == x-1 && right)
                    {
                        wall.Rect = room.Tile;
                        wall.Draw(spritebatch);
                        room.TileXToZero();
                        room.IncrementTileY();
                    }
                }
            }
            //draws the floor if necessary (probably will always be necessary)
            if (down)
            {
                for (int n = 0; n < x; n++)
                {
                    wall.Rect = room.Tile;
                    wall.Draw(spritebatch);
                    room.IncrementTileX();
                }
            }

        }               
        
        public bool SwitchRoom(Player player)
        {
            if (player.O_Position.Intersects(transitionRect) && transRectChar == 'd')
            {
                player.O_Y = 50;

                return true;
            }
            else if (player.O_Position.Intersects(transitionRect) && transRectChar == 'u')
            {
                player.O_Y = 750;
                return true;
            }

            else if (player.O_Position.Intersects(transitionRect) && transRectChar == 'r')
            {
                player.O_X = 50;
                
                return true;               

            }
            
            return false;
        }
                
    }   
}
