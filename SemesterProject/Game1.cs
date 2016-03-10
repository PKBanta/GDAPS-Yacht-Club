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
            Menu,    // Main Menu
            World,   // Game World
            Battle,  // Battle Scene
            Pause,   // Pause Screen
            GameOver // Game Over Screen
        }

        private GameState state;
        private GameState previousState; //Needed for pause menu

        private KeyboardState kbState;
        private KeyboardState previousKBState;

        private Player player;
        private Texture2D mainMenuImage, pauseImage, gameOverImage;
        private SpriteFont menuFont;

        private Menu MAIN_MENU, PAUSE_MENU, GAME_OVER_MENU;


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

            // mainMenuImage = Content.Load<Texture2D>(filename);
            // pauseImage = Content.Load<Texture2D>(filename);

            // gameOverImage = Content.Load<Texture2D>(filename);
            // menuFont = Content.Load<SpriteFont>(filename);

            // Main Menu
            MAIN_MENU = new Menu(
                mainMenuImage,
                Vector2.Zero,

                menuFont,
                "TITLE",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 100,
                    GraphicsDevice.Viewport.Height / 5),
                Color.White,

                menuFont,
                "Press Enter to play",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 200,
                    GraphicsDevice.Viewport.Height / 2),
                Color.White);

            // Pause Menu
            PAUSE_MENU = new Menu(
                pauseImage,
                new Vector2(20, 20),

                menuFont,
                "PAUSED",
                new Vector2(10, 10),
                Color.White,

                menuFont,
                "Press Enter to return to Main Menu\nPress P to resume",
                new Vector2(20, 50),
                Color.White);

            // GameOver Menu
            GAME_OVER_MENU = new Menu(
                gameOverImage,
                Vector2.Zero,

                menuFont,
                "GAME OVER",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 100,
                    GraphicsDevice.Viewport.Height / 5),
                Color.White,

                menuFont,
                "Press Enter to return to menu",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 250,
                    GraphicsDevice.Viewport.Height / 2),
                Color.White);
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
                        player.X -= 4;
                    }
                    else if (kbState.IsKeyDown(Keys.Right))
                    {
                        player.X += 4;
                    }                    
                    break;

                case GameState.Pause:
                    if (SingleKeyPress(Keys.P))
                    {
                        state = previousState;
                        previousState = GameState.Pause;
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
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (state)
            {
                case GameState.Menu:
                    MAIN_MENU.Draw(spriteBatch);
                    break;

                case GameState.World:
                    break;

                case GameState.Battle:
                    break;

                case GameState.Pause:
                    PAUSE_MENU.Draw(spriteBatch);
                    break;

                case GameState.GameOver:
                    GAME_OVER_MENU.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
