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
        //Player fields
        private int jumpSpeed;
        private int jumpAcceleration;
        private Rectangle above;
        private Rectangle below;

        //Constructor
        public Player(int x, int y, int width, int height, int speed, int damage, int maxHealth, Texture2D tex)
            :base(x, y, width, height, speed, damage, maxHealth, tex)
        {
            jumpSpeed = 4;
            jumpAcceleration = 17;
            above = new Rectangle(this.X, this.Y - 11, this.Width, 10);
            below = new Rectangle(this.X, this.Y + this.Height, this.Width, 10);
        }

        public int JumpAcceleration
        {
            get { return jumpAcceleration; }
            set { jumpAcceleration = value; }
        }

        public Rectangle Above
        {
            get { return above; }
        }

        public Rectangle Below
        {
            get { return below; }
        }

        //Moves character up as they jump
        public void Jump()
        {
            this.Y -= (jumpSpeed * (jumpAcceleration/2));
            if (jumpSpeed * (jumpAcceleration / 2) >= -25)
            {
                jumpAcceleration -= 2;
            }
        }

        public void UpdateDetectors()
        {
            above.X = this.X;
            above.Y = this.Y - 1;

            below.X = this.X;
            below.Y = this.Y + this.Height + 1;
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
