using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    class Player: Character
    {
        public Player(int x, int y, int width, int height, int damage, int maxHealth, Texture2D tex)
            : base(x,y,width,height,damage,maxHealth,tex)
        {

        }
        /*
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, rect, Color.White);
        }*/
    }
}
