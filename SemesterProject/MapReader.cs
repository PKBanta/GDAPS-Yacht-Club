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
        int x;
        int y;
        int count;
        bool up;
        bool down;
        bool left;
        bool right;
        char[] charList;
        string line;
        Room room;
        Wall wall;
        Platform platform;
        Collectible[] item;
        SpriteBatch spritebatch;
        Random rando = new Random();
        char[,] tileArray; 


        /// <summary>
        /// pure voodoo magic. might be dangerous
        /// </summary>
        /// <param name="name">name of the text file</param>
        /// <param name="plat">platform object for this particular room</param>
        /// <param name="wa">wall object for this particular room</param>
        /// <param name="i">arrray of items that can potentially be found in this room</param>
        /// <param name="batch">spritebatch</param>
        public void ReadMap(string name)
        {
            
            Stream instream;
            StreamReader input = null;
            

            count = 0;

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
                tileArray = new char[x, y];
                //sets up a new room with the input
                room = new Room(x, y, up, down, left, right);

               
                //saves the actual room into the character array
                while ((line = input.ReadLine()) != null)
                {

                    charList = line.ToCharArray();

                    for (int n = 0; n < y; n++)
                    {
                        tileArray[count,n] = charList[n];

                    }
                    count++;

                }
                       
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (input != null)
                    input.Close();
            }
       }

        public void DrawMap( Platform plat, Wall wa, Collectible[] i, SpriteBatch batch)
        {
            spritebatch = batch;
            platform = plat;
            wall = wa;
            item = i;
            count = 0;

            room.TileXToZero();
            room.TileYToZero();

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
                   
                    else if (tileArray[n,h] == '#')
                    {
                        platform.Rect = room.Tile;
                        platform.Draw(spritebatch);
                        room.IncrementTileX();
                    }
                    else if (tileArray[n, h] == '*')
                    {
                        Collectible randomItem = item[rando.Next(0, item.Length)];
                        randomItem.Rect = room.Tile;
                        randomItem.Draw(spritebatch);
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
                
    }   
}
