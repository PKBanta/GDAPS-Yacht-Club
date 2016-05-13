using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Room
    {
        // Fields
        int width;
        int height;
        bool up;
        bool down;
        bool left;
        bool right;
        Rectangle tile;

        // Properties

        public Rectangle Tile
        {
            get { return tile; }
            set { tile = value; }
        }
        

        // Constructors
        public Room(int x, int y, bool u, bool d, bool l, bool r)
        {
            width = x;
            height = y;
            up = u;
            down = d;
            left = l;
            right = r;

            tile = new Rectangle(0, 0, 25, 25);
        }
        /// <summary>
        /// moves the tile one space to the right
        /// </summary>
        public void IncrementTileX()
        {
            tile.X += 25;
        }

        /// <summary>
        /// resets the x position to zero and moves the tile one space down
        /// </summary>
        public void IncrementTileY()
        {
            
            tile.Y += 25;
        }

        public void TileXToZero()
        {
            tile.X = 0;
        }
        public void TileYToZero()
        {
            tile.Y = 0;
        }

        // Methods
    }
}
