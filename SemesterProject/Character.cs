using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Character
    {
        //Movement and appearance related fields
        private int x;
        private int y;
        private int xAcceleration;
        private int width;
        private int height;
        private Rectangle rect;
        private Texture2D tex;

        //Combat related fields
        private int damage;
        private int maxHealth;
        private int health;

        //Properties
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value;
                rect.Y = y;
                }
        }

        public int Damage
        {
            get { return damage; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int XAcceleration
        {
            get { return xAcceleration; }
            set { xAcceleration = value; }
        }

        //Constructor
        public Character(int x, int y, int width, int height, int damage, int maxHealth, Texture2D tex)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            rect = new Rectangle(x, y, width, height);
            this.tex = tex;
            this.damage = damage;
            this.maxHealth = maxHealth;
            health = maxHealth;
            xAcceleration = 1;
        }

        /// <summary>
        /// Allows the character to attack
        /// </summary>
        /// <param name="target">Target to attack</param>
        public virtual void Attack(Character target)
        {
            target.Health -= damage;
        }

        /// <summary>
        /// Allows the character to move
        /// </summary>
        /// <param name="dist">Distance for the character to move per frame</param>
        public virtual void Move(int dist)
        {
            rect.X += (dist * xAcceleration);
            if(xAcceleration < 5)
            {
                xAcceleration++;
            }
        }

        /// <summary>
        /// Draw this character
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, rect, Color.White);
        }
    }
}
