// Jared White
// May 2, 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    class Zombie : Enemy
    {
        // Fields
        private static Texture2D zombieSprite;
        private static SpriteFont font;
        public const int ZOM_HEALTH = 8, ZOM_SPEED = 10, ZOM_DAMAGE = 2;
        public const double ZOM_TIMER = .3;
        private const int BASE_CHARGE = 7;
        private double chargeTime;


        // Properties
        /// <summary>
        /// The Spritesheet of all Zombie Characters
        /// </summary>
        public static Texture2D ZombieSprite
        {
            get { return zombieSprite; }
            set { zombieSprite = value; }
        }

        /// <summary>
        /// The SpriteFont of all Zombie HealthBars
        /// </summary>
        public static SpriteFont HealthFont
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// The baseline time per frame for this Zombie - affects animation
        /// speed for this Zombie
        /// </summary>
        public override double BaseTimePerFrame
        {
            get { return base.BaseTimePerFrame; }
            set
            {
                base.BaseTimePerFrame = value;
                chargeTime = value;
            }
        }

        /// <summary>
        /// This Zombie's current state
        /// </summary>
        public override CharacterState State
        {
            get { return base.State; }
            set
            {
                base.State = value;

                // Setup required fields for the Zombie battle attack state
                //if (value == CharacterState.b_attack)
                //{
                //    damage = ZOM_DAMAGE;
                //}
            }
        }


        // Constructor
        /// <summary>
        /// Create a new Zombie object
        /// </summary>
        /// <param name="location">The location to draw this Zombie in when
        /// instantiated</param>
        /// <param name="healthBarBackground">The background texture of
        /// this Zombie's health bar</param>
        /// <param name="healthBarForeground">The foreground texture of
        /// this Zombie's health bar</param>
        /// <param name="health">The maximum health value of this Zombie</param>
        /// <param name="speed">The chronological identifier for this Zombie in
        /// battle</param>
        /// <param name="state">The state to instantiate this Zombie in</param>
        /// <param name="direction">The direction to draw this Zombie in</param>
        public Zombie(Vector2 location, Texture2D healthBarBackground,
            Texture2D healthBarForeground, int health = ZOM_HEALTH,
            int speed = ZOM_SPEED, CharacterState state = CharacterState.o_idle,
            CharDirection direction = CharDirection.left)
            : base(zombieSprite, location, health, speed, healthBarBackground,
                  healthBarForeground, font, 0.2, state, direction)
        {
            damage = ZOM_DAMAGE;
            damageCycleIndex = 4;
            chargeTime = BASE_CHARGE;

            frames[3] = new Rectangle(3 * FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);
            frames[4] = new Rectangle(0, FRAME_SIZE, FRAME_SIZE, FRAME_SIZE);

            walkIndicies = new int[2] { 4, 0 };
            attackIndicies = new int[6] { 0, 1, 2, 3, 1, 0 };

            // Setup the QuickTimeEvent for this Zombie
            //quickTimeTimer = ZOM_TIMER;
            quickTimeEvent.WindowLength = ZOM_TIMER;
        }


        // Methods
        /// <summary>
        /// Set the current frame to the next attack frame in the attack frame
        /// cycle.
        /// </summary>
        protected override void NextAttackFrame()
        {
            frameCycleIndex++;
            timeCounter = 0;

            if (frameCycleIndex >= attackIndicies.Length)
            {
                ResetDamage();
                frameCycleIndex = 0;
                timePerFrame = baseTimePerFrame;
                if (State == CharacterState.b_attack)
                    State = CharacterState.b_retreat;
            }

            else if (frameCycleIndex == 2)
            {
                // Charge the attack by waiting a brief random period of time
                //before executing the attack
                timePerFrame = baseTimePerFrame
                    * ((Random.NextDouble() * (1 - -1) - 1) + chargeTime);

                // Set the time to open this Zombie's Battle QuickTimeEvent
                // window
                quickTimeTimer = totalTime + timePerFrame - ZOM_TIMER;
            }

            else if (frameCycleIndex == 3)
            {
                timePerFrame = baseTimePerFrame * 0.5;
            }

            else if (frameCycleIndex == 4)
            {
                timePerFrame = baseTimePerFrame * 0.7;
            }

            if (frameCycleIndex == damageCycleIndex)
            {
                quickTimeEvent.WindowOpen = false;
            }

            // Change the source frame that's currently being drawn
            currentFrame = attackIndicies[frameCycleIndex];
        }

        /// <summary>
        /// A string representation of what this Character's type is
        /// </summary>
        /// <returns>A string representation of this Character's type</returns>
        public override string ToString()
        {
            return "Zombie";
        }

        /// <summary>
        /// Set Damage back to its default value
        /// </summary>
        protected override void ResetDamage()
        {
            damage = ZOM_DAMAGE;
        }
    }
}
