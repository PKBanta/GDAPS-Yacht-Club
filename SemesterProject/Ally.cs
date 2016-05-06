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
        public Ally(int x, int y, int width, int height, int speed, int damage, int maxHealth, Texture2D tex)
            : base(x, y, width, height, speed, damage, maxHealth, tex)
        {
            

        }

        /// <summary>
        /// Allows the ally to follow behind the player on the overworld
        /// This can be improved later
        /// </summary>
        /// <param name="p">The player</param>
        public void Follow(Player p)
        {
            X = p.X - 30;
            Y = p.Y;
        }
    }
}
