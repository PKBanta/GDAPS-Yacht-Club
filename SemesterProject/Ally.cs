using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SemesterProject
{
    class Ally : Character
    {
        public Ally(int x, int y, int width, int height, int damage, int maxHealth, Texture2D tex)
            : base(x,y,width,height,damage,maxHealth,tex)
        {


        }
    }
}
