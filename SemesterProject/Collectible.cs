using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Collectible : MapObject
    {
        //fields
        public string name;
        private bool active;
        private float rotation;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Collectible(int x, int y, int h, int w, Texture2D t, string n) : base(x, y, h, w, t)
        {
            name = n;
            active = true;
            rotation = 0;
        }

        public void SetInactive()
        {
            active = false;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (active)
            {
                spritebatch.Draw(Tex,Rect,Color.White);
            }
        }

        public void Collect(Player p)
        {
            if (p.Health <= p.MaxHealth - 5)
            {
                p.Health += 5;
            }

            else
            {
                p.Health = p.MaxHealth;
            }

            active = false;
        }
    }
}
