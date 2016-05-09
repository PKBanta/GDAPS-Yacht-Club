using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace SemesterProject
{
    class Enemy : Character
    {
        public Enemy(int x, int y, int width, int height, int speed, int damage, int maxHealth, Texture2D tex)
            : base(x, y, width, height, speed, damage, maxHealth, tex)
        {


        }

        /// <summary>
        /// Chooses the character to attack base on the health of each party member
        /// They will go after the character with the most health
        /// </summary>
        /// <param name="playerParty">List of the player and their allies</param>
        /// <returns>The character to attack</returns>
        public Character Target(List<Character> playerParty)
        {
            Character targ = playerParty[0];

            for(int i = 0; i < playerParty.Count; i++)
            {
                if (playerParty[i].Health > targ.Health) targ = playerParty[i];
            }

            return targ;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Texture, Rect, Color.White);
        }
    }
}
