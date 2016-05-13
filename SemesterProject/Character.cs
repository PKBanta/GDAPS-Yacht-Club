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
        private int xAcceleration;
        private int width;
        private int height;
        private Rectangle rect;
        private Texture2D tex;

        //Combat related fields
        private int damage;
        private int maxHealth;
        private int health;
        private int speed;

        //Properties
        public int X
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        public int Y
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
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
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        
        public int XAcceleration
        {
            get { return xAcceleration; }
            set { xAcceleration = value; }
        }

        public int Speed
        {
            get { return speed; }
        }

        public Texture2D Texture
        {
            get { return tex; }
            set { tex = value; }
        }

        //Constructor
        public Character(int x, int y, int width, int height, int speed, int damage, int maxHealth, Texture2D tex)
        {
            this.width = width;
            this.height = height;
            rect = new Rectangle(x, y, width, height);
            this.tex = tex;
            this.speed = speed;
            this.damage = damage;
            this.maxHealth = maxHealth;
            health = maxHealth;
            xAcceleration = 1;
        }

        // Methods
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
            if(xAcceleration < 3)
            {
                xAcceleration++;
            }
        }

        public virtual void Update()
        {

        }

        /// <summary>
        /// Draw this character
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, rect, Color.White);
        }

        public virtual bool CollisionCheck(Rectangle r1, Rectangle r2)
        {
            if (r1.Intersects(r2))
            {
                return true;
            }

            return false;
        }
    }
}
