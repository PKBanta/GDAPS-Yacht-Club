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
        bool up;
        bool down;
        bool left;
        bool right;
        char[] charList;
        string line;
        Room room;
        Platform platform;
        Collectible item;
        SpriteBatch spritebatch;

        /// <summary>
        /// pure voodoo magic. might be dangerous
        /// </summary>
        /// <param name="name">name of the text file</param>
        /// <param name="plat">platform object for this particular room</param>
        /// <param name="i">item that can be found in this room</param>
        /// <param name="batch">spritebatch</param>
        public void ReadMap(string name, Platform plat, Collectible i, SpriteBatch batch)
        {
            spritebatch = batch;
            platform = plat;
            item = i;
            Stream instream;
            StreamReader input = null;

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
                up = (charList[0] != 0);
                down = (charList[1] != 0);
                left = (charList[2] != 0);
                right = (charList[3] != 0);

                //sets up a new room with the input
                room = new Room(x, y, up, down, left, right);
                while ((line = input.ReadLine()) != null)
                {
                   
                        charList = line.ToCharArray();
                        for (int n = 0; n < x; n++)
                        {
                            if (charList[n] == '#')
                            {
                                platform.Draw(spritebatch);
                            }
                            else if (charList[n] == '*')
                            {
                                item.Draw(spritebatch);
                            }
                        }
                    
                    
                }
            }
            catch(Exception e)
            {
                
            }
            finally
            {
                if (input != null)
                    input.Close();
            }
       }
    }
}
