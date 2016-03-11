using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SemesterProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        enum GameState
        {
            Menu, //Main Menu
            World, //Game World
            Battle, //Battle Scene
            Pause, //Pause Screen
            GameOver //Game Over screen
        }
        private GameState state;
        private GameState previousState; //Needed for pause menu

        private KeyboardState kbState;
        private KeyboardState previousKBState;

        private Player player;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Check to see if a single key was pressed just this frame
        /// </summary>
        /// <param name="k">Key pressed</param>
        /// <returns></returns>
        public bool SingleKeyPress(Keys k)
        {
            if (kbState.IsKeyDown(k) && previousKBState.IsKeyUp(k))
            {
                return true;
            }

            return false;
        }

        /*
        public void ChangeRoom(int change)
        {
            if(change < 0)
            {
                //Go to previous room   
            }
            else if(change > 0)
            {
                //Go to next room
            }
        }
        */

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            state = GameState.Menu;
            kbState = Keyboard.GetState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player = new Player(5, 5, 50, 50, 10, 50, Content.Load<Texture2D>("Pokeball"));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            previousKBState = kbState;
            kbState = Keyboard.GetState();

            switch (state)
            {
                case GameState.Menu:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        previousState = state;
                        state = GameState.World;
                        
                    }
                    break;
                case GameState.World:
                    //Pause Menu
                    if (SingleKeyPress(Keys.P)) //Swtch to pause menu
                    {
                        previousState = state;
                        state = GameState.Pause;
                    }

                    //Put in player collision with enemy here

                    //Player Movement
                    if (kbState.IsKeyDown(Keys.Left))
                    {
                        player.Move(-4);
                    }
                    else if (kbState.IsKeyDown(Keys.Right))
                    {
                        player.Move(4);
                    }                    
                    break;
                case GameState.Pause:
                    if (SingleKeyPress(Keys.P))
                    {
                        state = previousState;
                        previousState = GameState.Pause;
                    }

                    if (SingleKeyPress(Keys.Enter))
                    {
                        previousState = state;
                        state = GameState.Menu;
                    }
                    break;
                /*case GameState.Battle: //Most likely will be commented out for this milestone
                    if (SingleKeyPress(Keys.P)) //Switch to pause menu
                    {
                        previousState = state;
                        state = GameState.Pause;
                    }

                    //If battle has finished(Some sort of bool needed here)
                    if (battle.Finished)
                    {
                        previousState = state;
                        state = GameState.World;
                    }
                    break;*/
                case GameState.GameOver:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        previousState = state;
                        state = GameState.Menu;

                    }
                    break;



            }

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
