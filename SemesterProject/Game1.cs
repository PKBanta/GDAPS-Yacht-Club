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

        enum PlayerXState
        {
            StandRight,     //Standing facing right
            WalkRight,      //Walking right
            StandLeft,      //Standing facing left
            WalkLeft,       //Walking left
        }

        enum PlayerYState
        {
            Jump,           //Jumping
            Fall,           //Falling
            Ground          //No vertical movement
        }

        private GameState state;
        private GameState previousState; //Needed for pause menu

        private KeyboardState kbState;
        private KeyboardState previousKBState;

        private Player player;
        private Texture2D playerTexture;        //Player's texture
        private PlayerXState playerXState;      //Player's X direction state
        private PlayerYState playerYState;      //Player's Y direction state

        private Texture2D mainMenuImage, pauseImage, gameOverImage;
        private SpriteFont menuFont;

        private Texture2D buttonImage;

        private Menu mainMenu, pauseMenu, gameOverMenu;


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
            state = GameState.Menu;
            kbState = Keyboard.GetState();
            IsMouseVisible = true;
            

            //Initializes player and their texture
            playerXState = PlayerXState.StandRight;
            playerYState = PlayerYState.Ground;
            
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

            mainMenuImage = Content.Load<Texture2D>("mainMenu");
            pauseImage = Content.Load<Texture2D>("pauseMenu");
            gameOverImage = Content.Load<Texture2D>("mainMenu");

            buttonImage = Content.Load<Texture2D>("ButtonImage");

            menuFont = Content.Load<SpriteFont>("MenuFont");

            //Loads the player's texture
            playerTexture = Content.Load<Texture2D>("mario");

            //Initializes player and their texture
            player = new Player(0, 0, playerTexture.Width, playerTexture.Height, 10, 20, playerTexture);

            // Create default buttons
            //// Activatable & clickable button
            //Button button0 = new Button(
            //    buttonImage,
            //    new Rectangle(GraphicsDevice.Viewport.Width - 100, 0,
            //        buttonImage.Width, buttonImage.Height),
            //    menuFont,
            //    "button0",
            //    Vector2.Zero,
            //    Color.White);

            //// Activatable but unclickable button
            //Button button1 = new Button(
            //    buttonImage,
            //    new Rectangle(GraphicsDevice.Viewport.Width - 100, 50,
            //        buttonImage.Width, buttonImage.Height),
            //    menuFont,
            //    "button1",
            //    Vector2.Zero,
            //    Color.White,
            //    true, false);

            //// Inactivatable but clickable button
            //Button button2 = new Button(
            //    buttonImage,
            //    new Rectangle(GraphicsDevice.Viewport.Width - 100, 100,
            //        buttonImage.Width, buttonImage.Height),
            //    menuFont,
            //    "button2",
            //    Vector2.Zero,
            //    Color.White,
            //    false, true);

            //// Inactivatable and unclickable button
            //Button button3 = new Button(
            //    buttonImage,
            //    new Rectangle(GraphicsDevice.Viewport.Width - 100, 150,
            //        buttonImage.Width, buttonImage.Height),
            //    menuFont,
            //    "button3",
            //    Vector2.Zero,
            //    Color.White,
            //    false, false);

            // Default menu buttons

            Button mainMenuButton = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width - 100, 0,
                    buttonImage.Width, buttonImage.Height),
                menuFont,
                "Main Menu",
                Vector2.Zero,
                Color.White);

            Button resumeButton = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width - 100, 50,
                    buttonImage.Width, buttonImage.Height),
                menuFont,
                "Resume",
                Vector2.Zero,
                Color.White);

            Button playButton = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width - 100, 0,
                    buttonImage.Width, buttonImage.Height),
                menuFont,
                "Play Game",
                Vector2.Zero,
                Color.White);

            //List<Button> testingButtons = new List<Button>()
            //{
            //    button0, button1, button2, button3
            //};

            List<Button> pauseButtons = new List<Button>()
            {
                mainMenuButton, resumeButton
            };

            List<Button> menuButtons = new List<Button>(){ playButton };

            List<Button> gameOverButtons = new List<Button>()
            {
                mainMenuButton
            };


            // Main Menu
            mainMenu = new Menu(
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
                Color.White,

                menuButtons);

            // Pause Menu
            pauseMenu = new Menu(
                pauseImage,
                new Vector2(20, 20),

                menuFont,
                "PAUSED",
                new Vector2(10, 10),
                Color.White,

                menuFont,
                "Press Enter to return to Main Menu\nPress P to resume",
                new Vector2(15, 30),
                Color.White,

                pauseButtons);

            // GameOver Menu
            gameOverMenu = new Menu(
                pauseImage,
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
                Color.White,

                gameOverButtons);
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

            int jumpTime = 5;
            int jumpCounter = 0;

            switch (state)
            {
                case GameState.Menu:
                    mainMenu.Update(Mouse.GetState());

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
                    pauseMenu.Update(Mouse.GetState());

                    if (SingleKeyPress(Keys.P))
                    {
                        state = previousState;
                        previousState = GameState.Pause;
                    }
                    break;

                case GameState.Battle:
                    if (SingleKeyPress(Keys.P))
                    {
                        previousState = state;
                        state = GameState.Pause;
                    }

                    /*
                    //If battle has finished(Some sort of bool needed here)
                    if (battle.Finished)
                    {
                        previousState = state;
                        state = GameState.World;
                    }*/

                    break;
                    
                case GameState.GameOver:
                    gameOverMenu.Update(Mouse.GetState());

                    if (SingleKeyPress(Keys.Enter))
                    {
                        previousState = state;
                        state = GameState.Menu;

                    }
                    break;

                

            }

            //Deals with players x directional movement
            switch (playerXState)
            {
                case PlayerXState.StandRight:
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        playerXState = PlayerXState.WalkRight;
                    }

                    else if (kbState.IsKeyDown(Keys.A))
                    {
                        playerXState = PlayerXState.WalkLeft;
                    }
                    break;

                case PlayerXState.WalkRight:
                    player.Move(20);

                    if (kbState.IsKeyUp(Keys.Right))
                    {
                        playerXState = PlayerXState.StandRight;
                    }
                    break;

                case PlayerXState.StandLeft:
                    if (kbState.IsKeyDown(Keys.A))
                    {
                        playerXState = PlayerXState.WalkLeft;
                    }

                    else if (kbState.IsKeyDown(Keys.D))
                    {
                        playerXState = PlayerXState.WalkRight;
                    }
                    break;

                case PlayerXState.WalkLeft:
                    player.Y = GraphicsDevice.Viewport.Height;

                    player.Move(-20);

                    if (kbState.IsKeyUp(Keys.Left))
                    {
                        playerXState = PlayerXState.StandLeft;
                    }
                    break;
            }

            //Deals with player's y directional movement
            switch (playerYState)
            {
                case PlayerYState.Ground:
                    player.Y = GraphicsDevice.Viewport.Height;

                    if (kbState.IsKeyDown(Keys.Space))
                    {
                        playerYState = PlayerYState.Jump;
                    }
                    break;

                case PlayerYState.Jump:
                    if (jumpCounter < jumpTime)
                    {
                        player.Jump();
                    }

                    else playerYState = PlayerYState.Fall;
                    break;

                case PlayerYState.Fall:
                    jumpCounter = 0;

                    if(player.Y < GraphicsDevice.Viewport.Height)
                    {
                        player.Fall();
                    }

                    else
                    {
                        playerYState = PlayerYState.Ground;
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
            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin();

            switch (state)
            {
                case GameState.Menu:
                    mainMenu.Draw(spriteBatch);
                    break;

                case GameState.World:
                    player.Draw(spriteBatch);
                    // Draw the contents of the current room here
                    break;

                case GameState.Battle:
                    player.Draw(spriteBatch); // DO NOT call this in the final
                    break;

                case GameState.Pause:
                    pauseMenu.Draw(spriteBatch);
                    break;

                case GameState.GameOver:
                    gameOverMenu.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
