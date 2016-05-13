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
    class Slime : Enemy
    {
        // Fields
        private static Texture2D slimeSprite;
        private static SpriteFont font;
        public const int SLIME_HEALTH = 5, SLIME_SPEED = 3, SLIME_DAMAGE = 2;
        public const double SLIME_TIMER = .3;
        private int chargeCycles, currentCharge;


        // Properties
        /// <summary>
        /// The Spritesheet of all Slime Characters
        /// </summary>
        public static Texture2D SlimeSprite
        {
            get { return slimeSprite; }
            set { slimeSprite = value; }
        }

        /// <summary>
        /// The SpriteFont of all Slime HealthBars
        /// </summary>
        public static SpriteFont HealthFont
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// This Slime's current state
        /// </summary>
        public override CharacterState State
        {
            get { return base.State; }
            set
            {
                base.State = value;

                // Setup required fields for the Slime battle attack state
                if (value == CharacterState.b_attack)
                {
                    timePerFrame = baseTimePerFrame * 5;
                    chargeCycles = Random.Next(1, 7);
                    currentCharge = 0;

                    //double addition = timePerFrame;
                    quickTimeTimer = timePerFrame;
                    for (int i = 0; i < (chargeCycles * 4) - 2; i++)
                    {
                        double addition = timePerFrame;
                        for (int c = 0; c <= i; c++)
                        {
                            addition /= 1.3;
                        }
                        /*addition*/quickTimeTimer += addition;
                    }
                    quickTimeTimer += totalTime - SLIME_TIMER; // THe ending on this mIGHT cause a problem
                    //quickTimeTimer = addition + (timePerFrame / (1.3 * ((4 * chargeCycles) - 2))) * 2;
                }
            }
        }


        // Constructor
        /// <summary>
        /// Create a new Slime object
        /// </summary>
        /// <param name="location">The location to draw this Slime in when
        /// instantiated</param>
        /// <param name="healthBarBackground">The background texture of
        /// this Slime's health bar</param>
        /// <param name="healthBarForeground">The foreground texture of
        /// this Slime's health bar</param>
        /// <param name="health">The maximum health value of this Slime</param>
        /// <param name="speed">The chronological identifier for this Slime in
        /// battle</param>
        /// <param name="state">The state to instantiate this Slime in</param>
        /// <param name="direction">The direction to draw this Slime in</param>
        public Slime(Vector2 location, Texture2D healthBarBackground,
            Texture2D healthBarForeground, int health = SLIME_HEALTH,
            int speed = SLIME_SPEED, CharacterState state 
            = CharacterState.o_idle, CharDirection direction
            = CharDirection.left) : base(slimeSprite, location, health, speed,
                healthBarBackground, healthBarForeground, font, 0.1, state,
                direction)
        {
            damage = SLIME_DAMAGE;
            damageCycleIndex = 5;
            chargeCycles = 4;

            frames[IDLE_FRAME] = new Rectangle(0, 0, FRAME_SIZE, FRAME_SIZE);
            frames[1] = new Rectangle(FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);
            frames[2] = new Rectangle(2 * FRAME_SIZE, 0, FRAME_SIZE,
                FRAME_SIZE);
            frames[3] = new Rectangle(3 * FRAME_SIZE, 0, FRAME_SIZE,
                FRAME_SIZE);
            frames[4] = new Rectangle(0, FRAME_SIZE, FRAME_SIZE, FRAME_SIZE);
            frames[5] = new Rectangle(FRAME_SIZE, FRAME_SIZE, FRAME_SIZE,
                FRAME_SIZE);
            frames[6] = new Rectangle(2 * FRAME_SIZE, FRAME_SIZE, FRAME_SIZE,
                FRAME_SIZE);

            attackIndicies = new int[8] { 0, 1, 2, 3, 4, 5, 6, 0 };
            walkIndicies = new int[4] { 1, 2, 3, 0 };

            // Setup the QuickTimeEvent for this Slime
            //quickTimeTimer = SLIME_TIMER;
            quickTimeEvent.WindowLength = SLIME_TIMER;
        }


        // Methods
        /// <summary>
        /// Set the current frame to the next attack frame in the attack frame
        /// cycle for the Slime's charge attack
        /// </summary>
        protected override void NextAttackFrame()
        {
            frameCycleIndex++;
            timeCounter = 0;

            if (frameCycleIndex >= attackIndicies.Length)
            {
                ResetDamage();
                timePerFrame = baseTimePerFrame * 5;
                frameCycleIndex = 0;

                if (State == CharacterState.b_attack)
                    State = CharacterState.b_retreat;
            }

            else if (frameCycleIndex == 0)
            {
                timePerFrame /= 1.3;
            }

            else if (frameCycleIndex == 1)
            {
                timePerFrame /= 1.3;
            }
            else if (frameCycleIndex == 2)
            {
                timePerFrame /= 1.3;
            }

            else if (frameCycleIndex == 3)
            {
                // Repeat charging cycle until all charges are complete
                if (currentCharge < chargeCycles)
                {
                    timePerFrame /= 1.3;
                    currentCharge++;
                    frameCycleIndex = 0;
                }

                // Move on past the charge cycle
                else
                {
                    timePerFrame = baseTimePerFrame / 2;
                }
            }

            else if (frameCycleIndex == 7)
            {
                timePerFrame *= 5;
            }

            if (frameCycleIndex == damageCycleIndex)
            {
                quickTimeEvent.WindowOpen = false;
            }

            currentFrame = attackIndicies[frameCycleIndex];
        }

        /// <summary>
        /// A string representation of what this Character's type is
        /// </summary>
        /// <returns>A string representation of this Character's type</returns>
        public override string ToString()
        {
            return "Blub";
        }

        /// <summary>
        /// Set Damage back to its default value
        /// </summary>
        protected override void ResetDamage()
        {
            damage = SLIME_DAMAGE;
        }
    }
}
