using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Player: Character
    {
        //Constructor
        public Player(int x, int y, int width, int height, int damage, int maxHealth, Texture2D tex)
            :base(x, y, width, height, damage, maxHealth, tex)
        {

        }

        //Moves character up as they jump
        public void Jump()
        {
            this.Y -= 20;
        }
        
        //Moves character down as they fall
        public void Fall()
        {
            this.Y += 20;
        }

        /*
        /// <summary>
        /// Detects if player collides with another character
        /// </summary>
        /// <param name="c">Character with which the player collides</param>
        public void Collide(Character c)
        {

        }
        */
    }
}
