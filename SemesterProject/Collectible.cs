using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Collectible : MapObject
    {
        //fields
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Collectible(int x, int y, int h, int w, Texture2D t, string n) : base(x, y, h, w, t)
        {
            name = n;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Tex, Rect, Color.White);
        }
    }
}
