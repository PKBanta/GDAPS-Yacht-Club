using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    abstract class Enemy : Character
    {
        // Fields
        public const double BASE_ENEMY_TIMER = 0.2/*, BASE_DODGE_COOLDOWN = 0.6, BASE_DODGE_TIME = 0.5*/;
        //protected double coolDownTimer; // The dodge cooldown timer. Color should change while dodging, return to normal after, and ability to dodge again returns after cooldown


        // Properties


        // Constructor
        /// <summary>
        /// Create a new Enemy
        /// </summary>
        /// <param name="spriteSheet">All sprites for this Enemy in a single
        /// image. Must have (an) idle, walking, and attack frame(s).</param>
        /// <param name="location">The base location of this Enemy when they are
        /// instantiated</param>
        /// <param name="health">The maximum health value for this Enemy
        /// </param>
        /// <param name="speed">The value used to determine the chronological
        /// order of attack during battles. Lower numbers have higher priority.
        /// </param>
        /// <param name="timePerFrame">The time that a frame of animation draws
        /// for before advancing to the next frame</param>
        /// <param name="healthBarBackground">The background texture of the
        /// health bar texture for this Enemy</param>
        /// <param name="healthBarForeground">The foreground texture of the
        /// health bar texture for this Enemy (the overlay)</param>
        /// <param name="healthFont">The font of the text in the health bar
        /// </param>
        /// <param name="state">The current CharacterState of this Enemy when
        /// it's initiated</param>
        /// <param name="direction">Whether this Enemy should be
        /// instantiated facing right or left</param>
        public Enemy(Texture2D spriteSheet, Vector2 location,
            int health, int speed, Texture2D healthBarBackground,
            Texture2D healthBarForeground, SpriteFont healthFont,
            double timePerFrame = 0.1,
            CharacterState state = CharacterState.o_idle,
            CharDirection direction = CharDirection.left) : base(spriteSheet,
                location, health, speed, timePerFrame, healthBarBackground,
                healthBarForeground, healthFont, state, direction)
        {
            frames = new Dictionary<int, Rectangle>(3);

            frames[IDLE_FRAME] = new Rectangle(0, 0, FRAME_SIZE, FRAME_SIZE);
            frames[1] = new Rectangle(FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);
            frames[2] = new Rectangle(2 * FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);

            walkIndicies = new int[2]
            {
                0, 2
            };
            attackIndicies = new int[5]
            {
                0, 1, 2, 1, 0
            };
            damageCycleIndex = 3;

            o_position.Width = 50;
            o_position.Height = 50;

            b_position = new Rectangle(
                (int)location.X,
                (int)location.Y,
                FRAME_SIZE / BATTLE_PROPORTION,
                FRAME_SIZE / BATTLE_PROPORTION);

            B_LocX = location.X;
            B_LocY = location.Y;

            // Create the QuickTimeEvent for this Enemy
            //quickTimeTimer = BASE_ENEMY_TIMER;
            quickTimeEvent = new QuickTimeEvent(NullifyDamage,
                Microsoft.Xna.Framework.Input.Keys.Q, BASE_ENEMY_TIMER);

            //coolDownTimer = 0;
        }

        // Methods
        /// <summary>
        /// A string representation of what this Character's type is
        /// </summary>
        /// <returns>A string representation of this Character's type</returns>
        public override string ToString()
        {
            return "Enemy";
        }

        /// <summary>
        /// Set damage to zero
        /// </summary>
        protected virtual void NullifyDamage()
        {
            damage = 0;
        }
    }
}
