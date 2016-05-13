using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SemesterProject
{
    abstract class Ally : Character, IFriendly
    {
        // Fields
        public const int JUMP_FRAME = 1;
        public const double BASE_ALLY_TIMER = 0.2;


        // Properties


        // Constructor
        /// <summary>
        /// Create a new Ally
        /// </summary>
        /// <param name="spriteSheet">All sprites for this Ally in a single
        /// image. Must have (an) idle, walking, and attack frame(s).</param>
        /// <param name="location">The base location of this Ally when they are
        /// instantiated</param>
        /// <param name="health">The maximum health value for this Ally
        /// </param>
        /// <param name="speed">The value used to determine the chronological
        /// order of attack during battles. Lower numbers have higher priority.
        /// </param>
        /// <param name="timePerFrame">The time that a frame of animation draws
        /// for before advancing to the next frame</param>
        /// <param name="healthBarBackground">The background texture of the
        /// health bar texture for this Ally</param>
        /// <param name="healthBarForeground">The foreground texture of the
        /// health bar texture for this Ally (the overlay)</param>
        /// <param name="healthFont">The font of the text in the health bar
        /// </param>
        /// <param name="state">The current CharacterState of this Ally when
        /// it's initiated</param>
        /// <param name="direction">Whether this Ally should be
        /// instantiated facing right or left</param>
        public Ally(Texture2D spriteSheet, Vector2 location,
            int health, int speed, Texture2D healthBarBackground,
            Texture2D healthBarForeground, SpriteFont healthFont,
            double timePerFrame = 0.1,
            CharacterState state = CharacterState.o_idle,
            CharDirection direction = CharDirection.right) : base(spriteSheet,
                location, health, speed, timePerFrame, healthBarBackground,
                healthBarForeground, healthFont, state, direction)
        {
            frames = new Dictionary<int, Rectangle>(3);

            frames[IDLE_FRAME] = new Rectangle(0, 0, FRAME_SIZE, FRAME_SIZE);
            frames[JUMP_FRAME] = new Rectangle(FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);
            frames[2] = new Rectangle(2 * FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);

            walkIndicies = new int[2] { 0, 2 };
            attackIndicies = new int[5] { 0, 1, 2, 1, 0 };

            o_position.Width = 25;
            o_position.Height = 50;

            b_position = new Rectangle(
                (int)location.X,
                (int)location.Y,
                FRAME_SIZE / 3,
                FRAME_SIZE / 3);

            // Create the QuickTimeEvent for this Ally
            //quickTimeTimer = BASE_ALLY_TIMER;
            quickTimeEvent = new QuickTimeEvent(
                new List<ActivationFunction> { ExtraDamage, NextAttackFrame },
                Microsoft.Xna.Framework.Input.Keys.E, BASE_ALLY_TIMER);
        }


        // Methods
        /// <summary>
        /// A string representation of what this Character's type is
        /// </summary>
        /// <returns>A string representation of this Character's type</returns>
        public override string ToString()
        {
            return "Ally";
        }

        /// <summary>
        /// Get the default amount of damage that this Character deals
        /// </summary>
        /// <returns></returns>
        public virtual int GetDamage()
        {
            return damage;
        }

        /// <summary>
        /// Set damage to extra its default value
        /// </summary>
        protected abstract void ExtraDamage();

        /// <summary>
        /// Allows the ally to follow behind the player on the overworld
        /// This can be improved later
        /// </summary>
        /// <param name="p">The player</param>
        public void Follow(Player p)
        {
            O_X = p.O_X - 30;
            O_Y = p.O_Y;
        }
    }
}