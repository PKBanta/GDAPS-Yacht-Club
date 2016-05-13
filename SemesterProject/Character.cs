using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    internal enum CharDirection
    {
        left,
        right
    }

    internal enum CharacterState
    {
        // Overworld States
        o_idle,
        o_walk,
        o_jump,
        o_attack,

        // Battle States
        b_idle,
        b_approach, // Approaching the enemy in battle
        b_attack,   // Quicktime window opens during this state
        b_retreat   // Returning to the place where this Character stances in battle
    }

    abstract class Character
    {
        // Fields
        #region Fields
        protected Texture2D spriteSheet;
        protected Color color;

        protected Rectangle o_position, b_position;

        private Vector2 b_location, o_location, healthBarLocation;
        private int o_width, o_height, xAcceleration;
        private float o_posX, o_posY, b_posX, b_posY;

        protected int healthMax, health, speed, damage, currentFrame,
            frameCycleIndex, damageCycleIndex;
        protected StatusBar healthBar;
        private static Random rand;

        private CharacterState state;
        private CharDirection direction;
        private SpriteEffects flipSprite;

        protected Dictionary<int, Rectangle> frames;
        protected double timePerFrame, baseTimePerFrame, timeCounter,
            quickTimeTimer, totalTime;
        protected static double fps;

        protected QuickTimeEvent quickTimeEvent;
        protected bool quickEventLocked;

        protected const int IDLE_FRAME = 0, FRAME_SIZE = 256,
            BATTLE_PROPORTION = 3;

        /// <summary>
        /// These values must be set for every object that inherits Character
        /// </summary>
        protected int[] walkIndicies, attackIndicies;
        #endregion


        #region Properties
        // Properties
        /// <summary>
        /// The sheet of all sprites for this Character
        /// </summary>
        public Texture2D SpriteSheet
        {
            get { return spriteSheet; }
            set
            {
                if (value != null)
                    spriteSheet = value;
            }
        }

        /// <summary>
        /// The Color of this Character
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        #region Locational Properties
        /// <summary>
        /// The on-screen position for this Character in the Overworld
        /// </summary>
        public Rectangle O_Position
        {
            get { return o_position; }
        }

        /// <summary>
        /// The on-screen position for this Character in the Battle Stage
        /// </summary>
        public Rectangle B_Position
        {
            get { return b_position; }
        }


        /// <summary>
        /// The on-screen location for this Character in the Overworld that's
        /// drawn to the screen
        /// </summary>
        public Vector2 O_Location
        {
            get { return new Vector2(o_position.X, o_position.Y); }
            set
            {
                o_location = value;

                o_position.X = (int)Math.Round(o_location.X);
                o_position.Y = (int)Math.Round(o_location.Y);
            }
        }

        /// <summary>
        /// The on-screen location for this Character on the Battle Stage that's
        /// drawn to the screen
        /// </summary>
        public Vector2 B_Location
        {
            get { return new Vector2(b_position.X, b_position.Y); }
            set
            {
                b_location = value;

                b_position.X = (int)Math.Round(b_location.X);
                b_position.Y = (int)Math.Round(b_location.Y);

                healthBar.X += b_position.X - healthBar.X;
                healthBar.Y += b_position.Y + (FRAME_SIZE / BATTLE_PROPORTION) + 10 - healthBar.Y;
            }
        }

        /// <summary>
        /// The X value of the Character's Overworld position
        /// </summary>
        public int O_X
        {
            get { return o_position.X; }
            set
            {
                o_posX = value;
                o_position.X = value;
                o_location.X = value;
                healthBar.X += value - healthBar.X;
            }
        }

        /// <summary>
        /// The Y value of the Character's Overworld position
        /// </summary>
        public int O_Y
        {
            get { return o_position.Y; }
            set
            {
                o_posY = value;
                o_position.Y = value;
                o_location.Y = value;
            }
        }

        /// <summary>
        /// The X value of the Character's Battle position
        /// </summary>
        public int B_X
        {
            get { return b_position.X; }
            set
            {
                b_posX = value;
                b_position.X = value;
                b_location.X = value;

                healthBar.X += value - healthBar.X;
            }
        }

        /// <summary>
        /// The Y value of the Character's Battle position
        /// </summary>
        public int B_Y
        {
            get { return b_position.Y; }
            set
            {
                b_posY = value;
                b_position.Y = value;
                b_location.Y = value;

                healthBar.Y += value + (FRAME_SIZE / BATTLE_PROPORTION) + 10 - healthBar.Y;
            }
        }

        /// <summary>
        /// The float X value of the character's overworld position
        /// </summary>
        public float O_LocX
        {
            get { return o_location.X; }
            set
            {
                o_posX = value;
                o_location.X = value;
                o_position.X = (int)Math.Round(o_posX);
            }
        }

        /// <summary>
        /// The float Y value of the character's overworld position
        /// </summary>
        public float O_LocY
        {
            get { return o_location.Y; }
            set
            {
                o_posY = value;
                o_location.Y = value;
                o_position.Y = (int)Math.Round(o_posY);
            }
        }

        /// <summary>
        /// The float X value of the character's battle position
        /// </summary>
        public float B_LocX
        {
            get { return b_location.X; }
            set
            {
                b_posX = value;
                b_location.X = value;
                b_position.X = (int)Math.Round(b_posX);

                healthBar.X += b_position.X - healthBar.X;
            }
        }

        /// <summary>
        /// The float Y value of the character's battle position
        /// </summary>
        public float B_LocY
        {
            get { return b_location.Y; }
            set
            {
                b_posY = value;
                b_location.Y = value;
                b_position.Y = (int)Math.Round(b_posY);

                healthBar.Y += b_position.Y + (FRAME_SIZE / BATTLE_PROPORTION) + 10 - healthBar.Y;
            }
        }

        /// <summary>
        /// This Character's overworld width
        /// </summary>
        public int O_Width
        {
            get { return o_position.Width; }
            set { o_position.Width = value; }
        }

        /// <summary>
        /// This Character's overworld height
        /// </summary>
        public int O_Height
        {
            get { return o_position.Height; }
            set { o_position.Height = value; }
        }
        #endregion

        #region Statistical Properties
        /// <summary>
        /// The maximum health value for this Character.
        /// Current health will never be greater than this value.
        /// </summary>
        public int MaxHealth
        {
            get { return healthMax; }
            set
            {
                if (value > 0)
                {
                    healthMax = value;
                    healthBar.MaxValue = value;

                    if (health < healthMax)
                    {
                        health = healthMax;
                    }
                }
            }
        }

        /// <summary>
        /// The current health value for this Character.
        /// Cannot be greater than maximum health.
        /// </summary>
        public int Health
        {
            get { return health; }
            set
            {
                if (value > healthMax)
                {
                    health = healthMax;
                }

                else if (value < 0)
                {
                    health = 0;
                }

                else
                {
                    health = value;
                }

                healthBar.CurrentValue = value;
            }
        }

        /// <summary>
        /// The amount of damage that this Character deals to other Characters
        /// during battles
        /// </summary>
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        /// <summary>
        /// This Character's chronological battle indexer (lower values have
        /// higher priority)
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public int XAcceleration
        {
            get { return xAcceleration; }
            set { xAcceleration = value; }
        }

        /// <summary>
        /// Random object
        /// </summary>
        public static Random Random
        {
            get { return rand; }
            set { rand = value; }
        }

        /// <summary>
        /// This Character's current state
        /// </summary>
        public virtual CharacterState State
        {
            get { return state; }
            set
            {
                timeCounter = 0;
                frameCycleIndex = 0;
                timePerFrame = baseTimePerFrame;
                quickTimeTimer = int.MaxValue;
                quickEventLocked = false;

                // Leave hastened retreat state
                if (state == CharacterState.b_retreat
                    && value != CharacterState.b_retreat)
                {
                    timePerFrame *= 2;

                    // Change directions
                    if (flipSprite == SpriteEffects.FlipHorizontally)
                        flipSprite = SpriteEffects.None;
                    else if (flipSprite == SpriteEffects.None)
                        flipSprite = SpriteEffects.FlipHorizontally;
                }

                // Enter hastened retreat state
                else if (state != CharacterState.b_retreat
                    && value == CharacterState.b_retreat)
                {
                    timePerFrame /= 2;

                    // Change directions
                    if (flipSprite == SpriteEffects.FlipHorizontally)
                        flipSprite = SpriteEffects.None;
                    else if (flipSprite == SpriteEffects.None)
                        flipSprite = SpriteEffects.FlipHorizontally;
                }

                // If the state has an animation loop, set the current frame to
                // the first frame in that cycle
                if (value == CharacterState.b_attack
                    || value == CharacterState.o_attack)
                {
                    ResetDamage();
                    currentFrame = attackIndicies[frameCycleIndex];
                }

                if (value == CharacterState.o_walk
                    || value == CharacterState.b_approach
                    || value == CharacterState.b_retreat)
                {
                    currentFrame = walkIndicies[frameCycleIndex];
                }

                state = value;
            }
        }

        /// <summary>
        /// The direction the Character is facing
        /// </summary>
        public CharDirection Direction
        {
            get { return direction; }
            set
            {
                if (value == CharDirection.left)
                {
                    flipSprite = SpriteEffects.FlipHorizontally;
                    direction = value;
                }

                if (value == CharDirection.right)
                {
                    flipSprite = SpriteEffects.None;
                    direction = value;
                }
            }
        }

        /// <summary>
        /// The SpriteEffect of this Character
        /// </summary>
        public SpriteEffects FlipSprite
        {
            get { return flipSprite; }
        }

        /// <summary>
        /// The StatusBar representing this Character's Health value
        /// </summary>
        public StatusBar HealthBar
        {
            get { return healthBar; }
            set { healthBar = value; }
        }
        #endregion

        #region Frames
        /// <summary>
        /// The index of the current frame. Setting this values behaves in a
        /// cyclical manner.
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set
            {
                if (value < 0)
                {
                    currentFrame = -value % frames.Count;
                }
                else if (value > frames.Count - 1)
                {
                    currentFrame = value % frames.Count;
                }
                else
                {
                    currentFrame = value;
                }
            }
        }

        /// <summary>
        /// The index of the Attack cycle frame during which damage is dealt
        /// </summary>
        public int DamageIndex
        {
            get { return damageCycleIndex; }
        }

        /// <summary>
        /// The index in the currently drawn animation cycle
        /// </summary>
        public int FrameCycleIndex
        {
            get { return frameCycleIndex; }
        }

        /// <summary>
        /// How many frames are in this Character's spritesheet
        /// </summary>
        public int NumFrames
        {
            get { return frames.Count; }
        }

        /// <summary>
        /// An array of walking frame indicies
        /// </summary>
        public int[] WalkFrames
        {
            get { return walkIndicies; }
        }

        /// <summary>
        /// An array of walking frame indicies
        /// </summary>
        public int[] AttackFrames
        {
            get { return attackIndicies; }
        }

        /// <summary>
        /// The source rectangle of a specified frame
        /// </summary>
        /// <param name="frameNum">The index of the desired frame</param>
        /// <returns>A soruce rectangle from the spritesheet</returns>
        public Rectangle this[int frameNum]
        {
            get { return frames[frameNum]; }
            set { frames[frameNum] = value; }
        }

        /// <summary>
        /// The amount of time Characters have for each frame
        /// </summary>
        public double TimePerFrame
        {
            get { return timePerFrame; }
            set
            {
                if (value > 0)
                {
                    fps = 1 / value;
                    timePerFrame = value;
                }
            }
        }

        /// <summary>
        /// The original amount of time Characters have for each frame
        /// </summary>
        public virtual double BaseTimePerFrame
        {
            get { return baseTimePerFrame; }
            set
            {
                if (value > 0)
                {
                    fps = 1 / value;
                    baseTimePerFrame = value;
                }
            }
        }

        /// <summary>
        /// The counter of time that has passed for this Character
        /// </summary>
        public double TimeCounter
        {
            get { return timeCounter; }
            //set { timeCounter = value; }
        }

        /// <summary>
        /// The QuickTimeEvent for this Character that's executed in this battle
        /// </summary>
        public QuickTimeEvent QuickTimeAction
        {
            get { return quickTimeEvent; }
        }
        
        public bool QuickEventLocked
        {
            get { return quickEventLocked; }
            set
            {
                if (value == true)
                {
                    CancelQuickEvent();
                }
                else
                {
                    quickEventLocked = value;
                }
            }
        }
        #endregion
        #endregion


        // Constructor
        /// <summary>
        /// A Character object
        /// </summary>
        /// <param name="spriteSheet">All sprites for this Character in a single
        /// image. Must have (an) idle, walking, and attack frame(s).</param>
        /// <param name="location">The base location of this Character in the
        /// overworld, regardless of sprite sheet size.</param>
        /// <param name="health">The maximum health value for this Character
        /// </param>
        /// <param name="speed">The value used to determine the chronological
        /// order of attack during battles. Lower numbers have higher priority.
        /// </param>
        /// <param name="timePerFrame">The time that a frame of animation draws
        /// for before advancing to the next frame</param>
        /// <param name="healthBarBackground">The background texture of the
        /// health bar texture for this Character</param>
        /// <param name="healthBarForeground">The foreground texture of the
        /// health bar texture for this Character (the overlay)</param>
        /// <param name="healthFont">The font of the text in the health bar
        /// </param>
        /// <param name="state">The current CharacterState of this Character
        /// when it's initiated</param>
        /// <param name="direction">Whether this Character should be
        /// instantiated facing right or left</param>
        /// <param name="sourceFrames">A collection of Rectangles to use as
        /// frames for drawing</param>
        public Character(Texture2D spriteSheet, Vector2 location, int health,
            int speed, double timePerFrame, Texture2D healthBarBackground,
            Texture2D healthBarForeground, SpriteFont healthFont,
            CharacterState state = CharacterState.o_idle,
            CharDirection direction = CharDirection.right,
            List<Rectangle> sourceFrames = null)
        {
            this.spriteSheet = spriteSheet;
            color = Color.White;

            currentFrame = 0;
            frameCycleIndex = 0;
            damageCycleIndex = 0;

            this.timePerFrame = timePerFrame;
            baseTimePerFrame = timePerFrame;

            timeCounter = 0;
            totalTime = 0;
            quickTimeTimer = int.MaxValue;
            quickEventLocked = false;

            State = state;
            Direction = direction;

            int o_width = 50, o_height = 50;

            if (sourceFrames != null)
            {
                frames = new Dictionary<int, Rectangle>(sourceFrames.Count);
                foreach (Rectangle rect in sourceFrames)
                {
                    frames.Add(frames.Count, rect);
                }
            }

            else
            {
                frames = new Dictionary<int, Rectangle>();
                frames.Add(0, new Rectangle(0, 0, spriteSheet.Width, spriteSheet.Height));
                o_width = (int)(frames[IDLE_FRAME].Width * ((double)50 / frames[IDLE_FRAME].Width));
                o_height = (int)(frames[IDLE_FRAME].Height * ((double)50 / frames[IDLE_FRAME].Height));
            }

            o_position = new Rectangle(
                (int)location.X,
                (int)location.Y,
                o_width,
                o_height);

            b_position = new Rectangle(
                (int)location.X,
                (int)location.Y,
                frames[IDLE_FRAME].Width,
                frames[IDLE_FRAME].Height);

            o_location = new Vector2(o_position.Location.X, o_position.Location.Y);
            b_location = new Vector2(b_position.Location.X, b_position.Location.Y);

            o_posX = location.X;
            o_posY = location.Y;
            b_posX = location.X;
            b_posY = location.Y;

            healthMax = health;
            this.health = healthMax;
            damage = 1;
            this.speed = speed;

            healthBar = new StatusBar(
                healthBarBackground,
                healthBarForeground,
                new Rectangle(
                    b_position.X - b_position.Width / 2 - 52,
                    b_position.Y + b_position.Height + 10,
                    104,
                    13),
                new Rectangle(
                    2,
                    2,
                    100,
                    9),
                healthFont,
                healthMax);


        }


        // Methods
        /// <summary>
        /// Update this Character
        /// Allows the character to attack and decrease the target's health by character's damage stat
        /// </summary>
        /// <param name="elapsedTime">The elapsed time (in seconds) that have
        /// passed since the last frame</param>
        public virtual void Update(double elapsedTime, KeyboardState keyState,
            KeyboardState prevKeyState)
        {
            // Update the total overall time that has passed while updating
            // this Character
            totalTime += elapsedTime;

            // If the window of opportunity hasn't opened, and it's time
            // for it to open, then open it.
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

            // FSM to handle CharacterState-based logic
            switch (state)
            {
                case CharacterState.o_walk:
                    timeCounter += elapsedTime;
                    if (timeCounter > timePerFrame)
                    {
                        NextWalkFrame();
                    }
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
                    if (timeCounter >= timePerFrame)
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
        /// Draw the Character to the screen
        /// Allows the character to move in the x direction, adding a constant distance to travel multiplied by an increasing acceleration
        /// to the X value of the rectangle
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to draw with</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case CharacterState.o_idle:
                    O_DrawIdle(spriteBatch);
                    break;

                case CharacterState.o_walk:
                    O_DrawWalking(spriteBatch);
                    break;

                case CharacterState.o_attack:   // Used for testing purposes
                    O_DrawAttack(spriteBatch);
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
            }
        }
        
        /// <summary>
        /// Draw this Character in their idle overworld state.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void O_DrawIdle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                o_position,
                frames[IDLE_FRAME],
                color,
                0,
                Vector2.Zero,
                flipSprite,
                0);
        }

        /// <summary>
        /// Draw this Character in their idle battle state.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void B_DrawIdle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                b_position,
                frames[IDLE_FRAME],
                color,
                0,
                Vector2.Zero,
                flipSprite,
                0);

            healthBar.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw the Character with walking frames in Overworld state
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void O_DrawWalking(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                o_position,
                frames[currentFrame],
                color,
                0,
                Vector2.Zero,
                flipSprite,
                0);
        }

        /// <summary>
        /// Draw the Character with walking frames in Battle state
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void B_DrawWalking(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                b_position,
                frames[currentFrame],
                color,
                0,
                Vector2.Zero,
                flipSprite,
                0);
        }

        /// <summary>
        /// Draw the Character with attack animation frames
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void O_DrawAttack(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                o_position,
                frames[currentFrame],
                color,
                0,
                Vector2.Zero,
                flipSprite,
                0);
        }

        /// <summary>
        /// Draw the Character with attack animation frames
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public virtual void B_DrawAttack(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                b_position,
                frames[currentFrame],
                color,
                0,
                Vector2.Zero,
                flipSprite,
                0);
        }

        /// <summary>
        /// A string representation of what this Character's type is
        /// </summary>
        /// <returns>A string representation of this Character's type</returns>
        public override string ToString()
        {
            return "Character";
        }

        /// <summary>
        /// Convert between overworld and battle states. Sets state to idle.
        /// </summary>
        public virtual void SwitchStage()
        {
            if (state <= CharacterState.o_attack)
            {
                state = CharacterState.b_idle;
            }

            else
            {
                state = CharacterState.o_idle;
            }
        }

        /// <summary>
        /// Set the current frame to the next attack frame in the attack frame
        /// cycle, then proceed to enter the B_Retreat state.
        /// </summary>
        protected virtual void NextAttackFrame()
        {
            frameCycleIndex++;
            timeCounter = 0;

            if (frameCycleIndex >= attackIndicies.Length)
            {
                frameCycleIndex = 0;
                State = CharacterState.b_retreat;
                ResetDamage();
            }

            if (frameCycleIndex == damageCycleIndex - 1)
            {
                quickTimeTimer = timeCounter + 0.1;
            }

            currentFrame = attackIndicies[frameCycleIndex];
        }

        /// <summary>
        /// Set the current frame to the next walk frame in the walk frame
        /// cycle.
        /// </summary>
        protected virtual void NextWalkFrame()
        {
            frameCycleIndex++;
            timeCounter = 0;

            if (frameCycleIndex >= walkIndicies.Length)
            {
                frameCycleIndex = 0;
            }

            currentFrame = walkIndicies[frameCycleIndex];
        }


        /// <summary>
        /// "Target Chaser" - Move this Character x units towards, but not past,
        /// another location.
        /// </summary>
        /// <param name="target">Target location</param>
        /// <param name="speed">Number of units to move per second</param>
        /// <param name="totalSeconds">Time passed since last frame</param>
        public void MoveTowards(Vector2 target, float speed, double totalSeconds)
        {
            if (state <= CharacterState.o_attack && target != o_location)
            {
                // The original target trajectory, and the trajectory tester
                Vector2 original = target;
                Vector2 difference = target - o_location;

                // Get the direction towards the target
                target -= o_location;
                target.Normalize();
                target *= (float)totalSeconds * speed;

                // Test the trajectory of the trajectory were this character to
                // move towards the target
                difference -= target;
                difference.Normalize();
                difference *= (float)totalSeconds * speed;

                // Ensure that the target vector does not travel past the target
                // If it does, set position to the original target trajectory.
                if ((Math.Round((difference + target).X) == 0
                    && (Math.Round((difference + target).Y) == 0)))
                {
                    O_LocX = original.X;
                    O_LocY = original.Y;
                }

                else
                {
                    O_LocX += target.X;
                    O_LocY += target.Y;
                }
            }

            else if (state >= CharacterState.b_idle && target != b_location)
            {
                // The original target trajectory, and the trajectory tester
                Vector2 original = target;
                Vector2 difference = target - b_location;

                // Get the direction towards the target
                target -= b_location;
                target.Normalize();
                target *= (float)totalSeconds * speed;

                // Test the trajectory of the trajectory were this character to
                // move towards the target
                difference -= target;
                difference.Normalize();
                difference *= (float)totalSeconds * speed;

                // Ensure that the target vector does not travel past the target
                // If it does, set position to the original target trajectory.
                if ((Math.Round((difference + target).X) == 0
                    && (Math.Round((difference + target).Y) == 0)))
                {
                    B_LocX = original.X;
                    B_LocY = original.Y;
                }

                else
                {
                    B_LocX += target.X;
                    B_LocY += target.Y;
                }
            }
        }

        /// <summary>
        /// Allows the character to move in the x direction, adding a constant
        /// distance to travel multiplied by an increasing acceleration to the X
        /// value of the rectangle
        /// </summary>
        /// <param name="dist">Distance for the character to move per frame
        /// </param>
        public virtual void Move(int dist)
        {
            O_LocX += (dist * xAcceleration);
            if (xAcceleration < 3)
            {
                xAcceleration++;
            }
        }

        /// <summary>
        /// Set the damage to its default value
        /// </summary>
        protected abstract void ResetDamage();

        /// <summary>
        /// Cancel the window of opportunity for the Character's quickTimeEvent,
        /// and prevent this Character from being able to activate their
        /// quickTimeEvent during the current cycle
        /// </summary>
        protected virtual void CancelQuickEvent()
        {
            quickTimeTimer = int.MaxValue;
            quickTimeEvent.WindowOpen = false;
            quickEventLocked = true;
        }
    }
}