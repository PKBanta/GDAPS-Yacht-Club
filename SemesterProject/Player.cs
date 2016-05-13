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
        private Rectangle right;
        private Rectangle left;

        /// <summary>
        /// Constructor inherits from Character class and sets up detector rectangles and jump variables
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Rectangle width</param>
        /// <param name="height">Rectangle height</param>
        /// <param name="speed">Speed that determines when the player will attack in battle</param>
        /// <param name="damage">Damage the player will deal in battle</param>
        /// <param name="maxHealth">Maximum amount of health the player can have</param>
        /// <param name="tex">Texture</param>
        public Player(int x, int y, int width, int height, int speed, int damage, int maxHealth, Texture2D tex)
            :base(x, y, width, height, speed, damage, maxHealth, tex)
        {
            jumpSpeed = 4;
            jumpAcceleration = 17;
            above = new Rectangle(this.X, this.Y - 11, this.Width, 10);
            below = new Rectangle(this.X, this.Y + this.Height, this.Width, 10);
            right = new Rectangle(this.X + this.Width, this.Y, 10, this.Height);
            left = new Rectangle(this.X - 11, this.Y, 10, this.Height);
        }

        //A variable that decreases over time to make the player jump and fall
        public int JumpAcceleration
        {
            get { return jumpAcceleration; }
            set { jumpAcceleration = value; }
        }

        //Small rectangles that detect collisions with map objects
        public Rectangle Above
        {
            get { return above; }
        }

        public Rectangle Below
        {
            get { return below; }
        }

        public Rectangle Right
        {
            get { return right; }
        }

        public Rectangle Left
        {
            get { return left; }
        }

        //Using a constant "speed" variable, an "acceleration" is added to the jump speed to change Y position
        //The acceleration decreases until it is -25 to allow the player to jump and then fall after a small amount of time
        public void Jump()
        {
            this.Y -= (jumpSpeed * (jumpAcceleration/2));
            if (jumpSpeed * (jumpAcceleration / 2) >= -25)
            {
                jumpAcceleration -= 2;
            }
        }

        //Updates the locations of the collision detection rectangles
        public void UpdateDetectors()
        {
            above.X = this.X;
            above.Y = this.Y - 1;

            below.X = this.X;
            below.Y = this.Y + this.Height + 1;

            right.X = this.X + this.Width;
            right.Y = this.Y;

            left.X = this.X - 11;
            left.Y = this.Y;
        }
    }
}
