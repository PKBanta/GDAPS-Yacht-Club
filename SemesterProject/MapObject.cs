using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace SemesterProject
{
    class MapObject
    {
        //location fields
        private int xpos;
        private int ypos;

        //appearance fields
        private int height;
        private int width;
        private Texture2D tex;
        private Rectangle rect;

        //properties

        public int Xpos
        {
            get { return xpos; }
            set { xpos = value; }
        }
        public int Ypos
        {
            get { return ypos; }
            set { ypos = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public Texture2D Tex
        {
            get { return tex; }
            set { tex = value; }
        }

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }
        //constructor
        public MapObject(int x, int y, int h, int w, Texture2D t)
        {
            xpos = x;
            ypos = y;
            height = h;
            width = w;
            tex = t;

            rect = new Rectangle(xpos, ypos, width, height);
        }

       
    }
}
