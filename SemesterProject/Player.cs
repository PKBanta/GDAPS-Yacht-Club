using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    class Player : Character, IFriendly
    {
        // Fields
        #region Fields
        public const int JUMP_FRAME = 1, PLAYER_HEALTH = 10, PLAYER_SPEED = 4,
            PLAYER_DAMAGE = 3;
        public const double PLAYER_TIMER = 0.15;
        private static Texture2D playerSprite;
        private static SpriteFont font;
        public static Color chargeColor = new Color(236, 167, 37), dodgeColor = new Color(83, 100, 188);

        private int jumpSpeed;
        private int jumpAcceleration;
        private Rectangle above;
        private Rectangle below;
        private Rectangle right;
        private Rectangle left;
        #endregion

        // Properties

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
        
        /// <summary>
        /// The Sprite of all Player objects
        /// </summary>
        public static Texture2D PlayerSprite
        {
            get { return playerSprite; }
            set { playerSprite = value; }
        }

        /// <summary>
        /// The SpriteFont of all Player HealthBars
        /// </summary>
        public static SpriteFont HealthFont
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// The Player's current state
        /// </summary>
        public override CharacterState State
        {
            get { return base.State; }
            set
            {
                base.State = value;

                // Setup required fields for the Player battle attack state
                if (value == CharacterState.b_attack)
                {
                    //ResetDamage();
                    timePerFrame = baseTimePerFrame * 10;
                }
            }
        }

        // Constructor
        /// <summary>
        /// Create a new Player object
        /// </summary>
        /// <param name="location">The location to draw the Player at when it's
        /// first instantiated</param>
        /// <param name="healthBarBackground">The background texture of this
        /// Player's health bar</param>
        /// <param name="healthBarForeground">The foreground texture of this
        /// Player's health bar (the overlay texture)</param>
        public Player(Vector2 location, Texture2D healthBarBackground,
            Texture2D healthBarForeground) : base(playerSprite, location,
                PLAYER_HEALTH, PLAYER_SPEED, 0.1, healthBarBackground,
                healthBarForeground, font)
        {
            o_position.Width = 25;
            o_position.Height = 50;

            frames[IDLE_FRAME] = new Rectangle(spriteSheet.Width - 561, 0, 561, 900);
            frames[JUMP_FRAME] = new Rectangle(spriteSheet.Width - 1913 - 610, 0, 605, 900);
            frames[2] = new Rectangle(spriteSheet.Width - 561 - 640, 0, 561, 900);
            frames[3] = new Rectangle(0, 0, 823, 901);

            walkIndicies = new int[2] { 2, 0 };
            attackIndicies = new int[5] { 0, 1, 3, 1, 0 };

            b_position = new Rectangle(
                (int)location.X,
                (int)location.Y,
                FRAME_SIZE / BATTLE_PROPORTION,
                FRAME_SIZE / BATTLE_PROPORTION);

            B_LocX = location.X;
            B_LocY = location.Y;

            damage = PLAYER_DAMAGE;
            damageCycleIndex = 2;

            // Create the QuickTimeEvent for this Player
            quickTimeEvent = new QuickTimeEvent(new List<ActivationFunction>
                { ExtraDamage, NextAttackFrame }, Keys.E, PLAYER_TIMER);

            jumpSpeed = 4;
            jumpAcceleration = 17;
            above = new Rectangle(this.O_X, this.O_Y - 11, this.O_Width, 10);
            below = new Rectangle(this.O_X, this.O_Y + this.O_Height, this.O_Width, 10);
            right = new Rectangle(this.O_X + this.O_Width, this.O_Y, 10, this.O_Height);
            left = new Rectangle(this.O_X - 11, this.O_Y, 10, this.O_Height);
        }

        //Moves character up as they jump
        //Using a constant "speed" variable, an "acceleration" is added to the jump speed to change Y position
        //The acceleration decreases until it is -25 to allow the player to jump and then fall after a small amount of time
        public void Jump()
        {
            this.O_Y -= (jumpSpeed * (jumpAcceleration/2));
            if (jumpSpeed * (jumpAcceleration / 2) >= -25)
            {
                jumpAcceleration -= 2;
            }
        }

        //Updates the locations of the collision detection rectangles
        public void UpdateDetectors()
        {
            above.X = this.O_X;
            above.Y = this.O_Y - 1;

            below.X = this.O_X;
            below.Y = this.O_Y + this.O_Height + 1;

            right.X = this.O_X + this.O_Width;
            right.Y = this.O_Y;

            left.X = this.O_X - 11;
            left.Y = this.O_Y;
        }


        // Methods
        /// <summary>
        /// Update the Player
        /// </summary>
        /// <param name="elapsedTime">The elapsed time in seconds since the last
        /// frame</param>
        public override void Update(double elapsedTime, KeyboardState keyState, KeyboardState prevKeyState)
        {
            // Update the overall total time passed
            totalTime += elapsedTime;



            // If the window of opportunity hasn't opened, and it's time
            // for it to open, then, well... Just open it, I guess. Idk.
            if (!quickTimeEvent.WindowOpen)
            {
                if (totalTime > quickTimeTimer)
                {
                    quickTimeEvent.OpenWindow();
                }
                else if (keyState.IsKeyDown(quickTimeEvent.ActivationKey))
                {
                    CancelQuickEvent();
                }
            }

            // Update the QuickTimeEvent if the window of opportunity is
            // open
            else if (quickTimeEvent.WindowOpen)
            {
                quickTimeEvent.Update(elapsedTime, keyState,
                    prevKeyState);

                if (!quickTimeEvent.WindowOpen)
                {
                    quickTimeTimer = int.MaxValue;
                }
            }

            // Update the Character-State-dependent turn-based battle logic for
            // the Player
            switch (State)
            {
                case CharacterState.o_walk:
                    timeCounter += elapsedTime;
                    if (timeCounter > timePerFrame)
                    {
                        NextWalkFrame();
                    }
                    break;


                case CharacterState.o_jump:
                    break;


                case CharacterState.o_attack:
                    timeCounter += elapsedTime;
                    if (timeCounter > timePerFrame)
                    {
                        NextAttackFrame();
                    }
                    break;


                case CharacterState.b_approach:
                    timeCounter += elapsedTime;
                    if (timeCounter > timePerFrame)
                    {
                        NextWalkFrame();
                    }
                    break;


                case CharacterState.b_attack:
                    timeCounter += elapsedTime;

                    //// If the window of opportunity hasn't opened, and it's time
                    //// for it to open, then open it
                    //if (!quickTimeEvent.WindowOpen && timeCounter > quickTimeTimer)
                    //{
                    //    quickTimeEvent.OpenWindow();
                    //}

                    //// Update the QuickTimeEvent if the window of opportunity is
                    //// open
                    //else if (quickTimeEvent.WindowOpen)
                    //{
                    //    quickTimeEvent.Update(elapsedTime, keyState,
                    //        prevKeyState);
                    //}

                    if (timeCounter > timePerFrame)
                    {
                        NextAttackFrame();
                    }
                    break;


                case CharacterState.b_retreat:
                    timeCounter += elapsedTime;
                    if (timeCounter > timePerFrame)
                    {
                        NextWalkFrame();
                    }
                    break;
            }
        }

        /// <summary>
        /// Draw the Player to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (State)
            {
                case CharacterState.o_idle:
                    O_DrawIdle(spriteBatch);
                    break;

                case CharacterState.o_walk:
                    O_DrawWalking(spriteBatch);
                    break;

                case CharacterState.b_idle:
                    B_DrawIdle(spriteBatch);
                    break;

                case CharacterState.b_approach:
                case CharacterState.b_retreat:
                    B_DrawWalking(spriteBatch);
                    break;

                case CharacterState.b_attack:
                    B_DrawAttack(spriteBatch);
                    break;

                case CharacterState.o_jump:
                    DrawJump(spriteBatch);
                    break;
            }
        }

        /// <summary>
        /// Draw the Player in a jumping state
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public void DrawJump(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                o_position,
                frames[JUMP_FRAME],
                color,
                0,
                Vector2.Zero,
                FlipSprite,
                0);
        }

        /// <summary>
        /// Draw the Player through the walking frames in a walking state in the
        /// Overworld
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void O_DrawWalking(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                o_position,
                frames[currentFrame],
                color,
                0,
                Vector2.Zero,
                FlipSprite,
                0);
        }

        /// <summary>
        /// Draw the Player through the attack frames in the Battle Stage
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void B_DrawAttack(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                b_position,
                frames[currentFrame],
                color,
                0,
                Vector2.Zero,
                FlipSprite,
                0);
        }

        /// <summary>
        /// Advance to the next attack frame in the Player's attack frame
        /// cycle
        /// </summary>
        protected override void NextAttackFrame()
        {
            frameCycleIndex++;
            timeCounter = 0;

            if (frameCycleIndex >= attackIndicies.Length)
            {
                ResetDamage();
                timePerFrame = baseTimePerFrame * 10;
                frameCycleIndex = 0;
                if (State == CharacterState.b_attack)
                    State = CharacterState.b_retreat;
            }

            else if (frameCycleIndex == 1)
            {
                timePerFrame = baseTimePerFrame * (50.0 / 7.0);
                quickTimeTimer = totalTime + timePerFrame - PLAYER_TIMER;
            }

            else if (frameCycleIndex == 2)
            {
                timePerFrame = baseTimePerFrame * (100.0 / 63.0);
            }

            else if (frameCycleIndex == 3)
            {
                timePerFrame = baseTimePerFrame * (20.0 / 189.0);
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
            return "Player";
        }

        /// <summary>
        /// Move a horizontal distance
        /// </summary>
        /// <param name="distance">Units to move by</param>
        public void Walk(float distance)
        {
            O_LocX += distance;
        }

        /// <summary>
        /// Set the Player's damage to extra its default value and have the
        /// Player glow gold to indicate something special happened
        /// </summary>
        protected void ExtraDamage()
        {
            damage = PLAYER_DAMAGE + 3;
            color = new Color(236, 167, 37);
        }

        /// <summary>
        /// Set Damage back to its default value - intended for use in the
        /// QuickTimeEvent
        /// </summary>
        protected override void ResetDamage()
        {
            damage = PLAYER_DAMAGE;
            Color = Color.White;
        }

        /// <summary>
        /// Get the default amount of damage that the Player deals
        /// </summary>
        /// <returns>Player damage</returns>
        public int GetDamage()
        {
            return PLAYER_DAMAGE;
        }
    }
}