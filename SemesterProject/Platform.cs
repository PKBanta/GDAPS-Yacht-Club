using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Platform : MapObject
    {
        public Platform (int x, int y, int h, int w, Texture2D t) : base(x, y, h, w, t)
        {

        }
    }
}
