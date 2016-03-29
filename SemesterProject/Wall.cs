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
    class Wall : MapObject
    {
        public Wall(int x, int y, int h, int w, Texture2D t) : base(x, y, h, w, t)
        {

        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Tex, Rect, Color.White);
        }
    }
}
