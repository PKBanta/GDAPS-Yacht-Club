using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace SemesterProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        #region GameStates        
        enum GameState
        {
            Menu,    // Main ListMenu
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
        
        private GameState gameState;
        private GameState previousState; //Needed for pause menu
        #endregion GameStates

        #region KB/Mouse States
        private KeyboardState kbState;
        private KeyboardState previousKBState;
        
        private MouseState mState;
        private MouseState previousMState;
        #endregion KB/Mouse States

        #region Player
        private Player player;
        private Texture2D playerTexture;        //Player's texture
        private PlayerXState playerXState;      //Player's X direction state
        private PlayerYState playerYState;      //Player's Y direction state
        private Vector2 preBattlePosition;
        #endregion Player

        #region Menus/Buttons
        private Texture2D mainMenuImage, pauseImage, gameOverImage, buttonImage,
            quitImage, shadeOverlay;
        private SpriteFont menuFont, buttonFont, healthFont;
        private static Rectangle screen;
        private static Color shadowColor = new Color(200, 200, 200, 255);

        private ListMenu mainMenu, pauseMenu, gameOverMenu, quitMenu;
        private bool quitActive;

        private Button mainMenu_play, mainMenu_quit, pause_menu, pause_resume,
            pause_quit, quit_no, quit_yes, battleTime, battleButton;
        private List<Button> mainMenuButtons, pauseButtons, gameOverButtons,
            confirmQuitButtons;
        
        private static Vector2 buttonTextLoc = new Vector2(5, 5);
        #endregion Menu/Buttons

        #region Textures/Misc.
        private Texture2D collectibleTexture, wallTexture, platTexture,
            sewerTexture, healthBarBase, healthBarOverlay;

        private Collectible collectible;
        private Wall wall;
        private Platform platform;
        private MapReader reader;
        
        private Background sewerBG;

        //the quadtree
        private QuadTreeNode quadTree;
        //the node for Mario
        private QuadTreeNode marioQuad;

        private double timeCounter, timePerFrame;

        #endregion Textures/Misc.

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
        }

        #region Single Mouse/Key Press
        /// <summary>
        /// Check to see if a single key was pressed just this frame
        /// </summary>
        /// <param name="k">Key pressed</param>
        /// <returns>True if pressed this frame</returns>
        private bool SingleKeyPress(Keys k)
        {
            return (kbState.IsKeyDown(k) && previousKBState.IsKeyUp(k));
        }

        /// <summary>
        /// Check to see if the left mouse button was pressed just this frame
        /// </summary>
        /// <returns>True if pressed this frame</returns>
        private bool SingleMouseClick()
        {
            return (mState.LeftButton == ButtonState.Pressed
                && previousMState.LeftButton == ButtonState.Released);
        }
        #endregion Single Mouse/Key Press

        #region Initialize/Load Content
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = GameState.Menu;
            kbState = Keyboard.GetState();
            IsMouseVisible = true;
            quitActive = false;
            reader = new MapReader();
            quadTree = new QuadTreeNode(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Initializes player and their texture
            playerXState = PlayerXState.StandRight;
            playerYState = PlayerYState.Ground;

            screen = new Rectangle(0, 0, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            timeCounter = 0;
            timePerFrame = 10;

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
            
            //Loads the player's texture
            playerTexture = Content.Load<Texture2D>("mario");

            // Menu + Misc textures
            mainMenuImage = Content.Load<Texture2D>("mainMenu");
            pauseImage = Content.Load<Texture2D>("pauseMenu");
            gameOverImage = Content.Load<Texture2D>("mainMenu");
            quitImage = Content.Load<Texture2D>("QuitMenu");
            buttonImage = Content.Load<Texture2D>("ButtonImage");
            shadeOverlay = Content.Load<Texture2D>("ShadeOverlay");

            menuFont = Content.Load<SpriteFont>("MenuFont");
            buttonFont = Content.Load<SpriteFont>("ButtonFont");
            healthFont = Content.Load<SpriteFont>("Consolas_9");

            healthBarBase = Content.Load<Texture2D>("Health Bar Base");
            healthBarOverlay = Content.Load<Texture2D>("Health Bar Overlay");

            //Loads map textures
            wallTexture = Content.Load<Texture2D>("wall");
            collectibleTexture = Content.Load<Texture2D>("collectible");
            platTexture = Content.Load<Texture2D>("tile");
            sewerTexture = Content.Load<Texture2D>("sewer BG");

            //Initializes player and their texture
            player = new Player(0, 300, 25, 50, 1, 10, 20, playerTexture);
            player.Health = 14;

            reader.ReadMap("room.txt",quadTree,collectibleTexture);
            
            wall = new Wall(0, 0, 25, 25, wallTexture);
            platform = new Platform(0, 0, 25, 25, platTexture);
            collectible = new Collectible(0, 0, 25, 25, collectibleTexture, "Horseshit");
            
            //reader.StoreObjects(platform, wall, collectList, spriteBatch);
            sewerBG = new Background(0, 0, 800, 1200,sewerTexture);
            
            #region Buttons
            mainMenu_play = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width - 30, GraphicsDevice.Viewport.Height
                    / 2 - 30, buttonImage.Width, buttonImage.Height),
                StartGame,  // ActivationFunction

                buttonFont,
                "Play",
                buttonTextLoc,
                Color.White,

                true,   // active
                true,   // highlightable
                true,   // clickable
                true);  // linger

            mainMenu_quit = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    + 30, GraphicsDevice.Viewport.Height / 2 - 30,
                    buttonImage.Width, buttonImage.Height),
                ShowQuitMenu,  // ActivationFunction

                buttonFont,
                "Exit",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                true); // linger

            pause_menu = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width - buttonImage.Width,
                buttonImage.Height, buttonImage.Width, buttonImage.Height),
                MainMenu,  // ActivationFunction

                buttonFont,
                "Main Menu",
                buttonTextLoc,
                Color.White,

                true,   // active
                true,   // highlightable
                true,   // clickable
                true); // linger

            pause_resume = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width - 100, 0,
                    buttonImage.Width, buttonImage.Height),
                UnpauseGame,  // ActivationFunction

                buttonFont,
                "Resume",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                true); // linger

            pause_quit = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width - buttonImage.Width,
                2 * buttonImage.Height, buttonImage.Width, buttonImage.Height),
                ShowQuitMenu,  // ActivationFunction

                buttonFont,
                "Quit game",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                true); // linger

            quit_no = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width - 10, GraphicsDevice.Viewport.Height
                    / 2, buttonImage.Width, buttonImage.Height),
                DenyQuit,  // ActivationFunction

                buttonFont,
                "No",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                true); // linger

            quit_yes = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    + 10, GraphicsDevice.Viewport.Height / 2,
                    buttonImage.Width, buttonImage.Height),
                Exit,  // ActivationFunction

                buttonFont,
                "Yes",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                true); // linger
            /*
            battleTime = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width / 2, GraphicsDevice.Viewport.Height / 2
                    + buttonImage.Height / 2 + 10,
                    buttonImage.Width, buttonImage.Height),
                Battle(new Enemy(0,0,50,50,5, 10, 20, collectible.Tex)),  // ActivationFunction

                buttonFont,
                "Battle Time",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                false); // linger
            */
            battleButton = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width / 2, GraphicsDevice.Viewport.Height / 2
                    + buttonImage.Height / 2 + 10,
                    buttonImage.Width, buttonImage.Height),
                new List<ActivationFunction>() { },  // ActivationFunction

                buttonFont,
                "Battle",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                false); // linger
            #endregion

            #region Button Lists
            mainMenuButtons = new List<Button>()
            {
                mainMenu_play,
                mainMenu_quit,
                battleTime
            };

            pauseButtons = new List<Button>()
            {
                pause_resume,
                pause_menu,
                pause_quit
            };

            gameOverButtons = new List<Button>()
            {
                
            };

            confirmQuitButtons = new List<Button>()
            {
                quit_no,
                quit_yes
            };
            #endregion

            #region Menus
            // Main Menu
            mainMenu = new ListMenu(
                mainMenuImage,
                Vector2.Zero,
                Color.White,

                menuFont,
                "TITLE",
                new Vector2 (GraphicsDevice.Viewport.Width / 2 - 22,
                    GraphicsDevice.Viewport.Height / 4),
                Color.White,
                
                menuFont,
                "Click or use arrow keys to select an option.",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 150,
                    GraphicsDevice.Viewport.Height / 4 + 20),
                Color.White,

                mainMenuButtons,
                Keys.Left,
                Keys.Right,
                Keys.Enter,
                -1,
                true);

            // Pause Menu
            pauseMenu = new ListMenu(
                pauseImage,
                new Vector2(20, 20),
                Color.White,

                menuFont,
                "PAUSED",
                new Vector2(5, 7),
                Color.White,

                menuFont,
                "Select an option w/ arrows or mouse\nPress P to resume",
                new Vector2(10, 30),
                Color.White,

                pauseButtons,
                Keys.Up,
                Keys.Down,
                Keys.Enter,
                0,
                true);

            // GameOver Menu
            gameOverMenu = new ListMenu(
                pauseImage,
                Vector2.Zero,
                Color.White,

                menuFont,
                "GAME OVER",
                new Vector2(GraphicsDevice.Viewport.Width / 2
                - pauseImage.Width, GraphicsDevice.Viewport.Height / 5),
                Color.White,

                menuFont,
                "Press Enter to return to menu",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 250,
                    GraphicsDevice.Viewport.Height / 2),
                Color.White,

                gameOverButtons,
                Keys.Left,
                Keys.Right,
                Keys.Enter,
                0,
                false);

            // GameOver Menu
            quitMenu = new ListMenu(
                quitImage,
                new Vector2(GraphicsDevice.Viewport.Width / 2
                - quitImage.Width / 2, GraphicsDevice.Viewport.Height / 2
                - quitImage.Height / 2),
                Color.White,

                menuFont,
                "Are you sure you want to quit?",
                new Vector2(25, 30),
                Color.White,

                confirmQuitButtons,
                Keys.Left,
                Keys.Right,
                Keys.Enter,
                0,
                false);
            #endregion
            
            mainMenu_play.ButtonActivationEvent += mainMenu.Reset;
            battleTime.ButtonActivationEvent += mainMenu.Reset;
            pause_resume.ButtonActivationEvent += pauseMenu.Reset;
            pause_menu.ButtonActivationEvent += pauseMenu.Reset;
            quit_no.ButtonActivationEvent += quitMenu.Reset;
        }
        #endregion Initialize/Load Content
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
           // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
             //   Exit();

            previousKBState = kbState;
            kbState = Keyboard.GetState();

            previousMState = mState;
            mState = Mouse.GetState();

            marioQuad = quadTree.GetContainingQuad(player.Rect);

            if (!quitActive)
                switch (gameState)
                {
                    case GameState.Menu:
                        if (!quitActive)
                            mainMenu.Update(mState, previousMState, kbState, previousKBState);
                        break;

                    case GameState.World:
                        timeCounter += gameTime.ElapsedGameTime.TotalMilliseconds;
                    
                        if (timeCounter >= timePerFrame)
                        {
                            timeCounter = 0;
                            //collectible.Rotation += .009f;
                        }
                    
                        if (SingleKeyPress(Keys.P)) //Swtch to pause menu
                        {
                            PauseGame();
                        }

                        //Put in player collision with enemy here

                        //Player Movement
                        if (kbState.IsKeyDown(Keys.Left))
                        {
                            player.Move(-5);
                        }
                        else if (kbState.IsKeyDown(Keys.Right))
                        {
                            player.Move(5);
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
                                player.Move(5);
                                player.UpdateDetectors();

                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Rect.Intersects(reader.RectList[i]))
                                    {
                                        player.X = reader.RectList[i].X - player.Width;
                                        break;
                                    }
                                }

                                if (kbState.IsKeyUp(Keys.D))
                                {
                                    playerXState = PlayerXState.StandRight;
                                    player.XAcceleration = 1;
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
                                player.Move(-5);
                                player.UpdateDetectors();

                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Rect.Intersects(reader.RectList[i]))
                                    {
                                        player.X = reader.RectList[i].X + player.Width;
                                        break;
                                    }
                                }
                                
                                if (kbState.IsKeyUp(Keys.A))
                                {
                                    playerXState = PlayerXState.StandLeft;
                                    player.XAcceleration = 1;
                                }
                                break;
                        }

                        //Deals with player's y directional movement
                        switch (playerYState)
                        {
                            case PlayerYState.Ground:
                                //player.Y = GraphicsDevice.Viewport.Height - player.Height;
                                bool onPlatform = false;

                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Below.Intersects(reader.RectList[i]))
                                    {
                                        onPlatform = true;
                                        break;
                                    }
                                }

                                //for (int i = 0; i < collectList.Length; i++)
                                //{
                                //        if (player.Rect.Intersects(collectList[i].Rect))
                                //        {
                                //            collectList[i].Collect(player);
                                //        }
                                //}

                                for (int i = 0; i < reader.ItemList.Count; i++)
                                {
                                        if (player.Rect.Intersects((reader.ItemList)[i].Rect))
                                        {
                                            reader.ItemList[i].Collect(player);
                                        }
                                }
                            
                                if (player.Y >= GraphicsDevice.Viewport.Height - player.Height)
                                {
                                    onPlatform = true;
                                }

                                if (!onPlatform)
                                {
                                    player.JumpAcceleration = -2;
                                    playerYState = PlayerYState.Jump;
                                }

                                if (kbState.IsKeyDown(Keys.Space))
                                {
                                    playerYState = PlayerYState.Jump;
                                }
                                break;

                            case PlayerYState.Jump:
                                player.Jump();
                                player.UpdateDetectors();

                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Above.Intersects(reader.RectList[i]))
                                    {
                                        player.JumpAcceleration = -2;
                                        break;
                                    }

                                    else if (player.Rect.Intersects(reader.RectList[i]))
                                    {
                                        player.Y = reader.RectList[i].Y - player.Height;
                                        player.JumpAcceleration = 20;
                                        playerYState = PlayerYState.Ground;
                                        break;
                                    }
                                }

                                if (player.Y >= GraphicsDevice.Viewport.Height - player.Height)
                                {
                                    playerYState = PlayerYState.Ground;
                                    player.JumpAcceleration = 20;
                                }
                                break;
                        }
                        break;

                    case GameState.Pause:
                        if (!quitActive)
                        {
                            pauseMenu.Update(mState, previousMState, kbState,
                                previousKBState);

                            if (SingleKeyPress(Keys.P))
                            {
                                UnpauseGame();
                            }
                        }
                        break;
                    

                    case GameState.Battle:

                        BattleManager.Update(mState, previousMState);
                    
                        if(BattleManager.AllDead())
                        {
                            gameState = GameState.Menu;
                            player.X = (int)preBattlePosition.X;
                            player.Y = (int)preBattlePosition.Y;
                        }

                        if(player.Health <= 0)
                            {
                                gameState = GameState.Menu;
                            }

                        if (SingleKeyPress(Keys.P))
                        {
                            PauseGame();
                        }

                        break;
                    
                    case GameState.GameOver:
                        gameOverMenu.Update(mState, previousMState, kbState,
                            previousKBState);
                        break;
                }

            if (quitActive)
            {
                quitMenu.Update(mState, previousMState, kbState, previousKBState);
            }
            if (reader.SwitchRoom(player))
            {
                reader.ReadMap("../../../Content/Rooms/room" + reader.RoomNumber +".txt", quadTree, collectibleTexture);
                
            }
            base.Update(gameTime);
        }
        #endregion Update

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Menu:
                    DrawMainMenu();
                    break;


                case GameState.World:
                    DrawWorld();
                    
                    break;


                case GameState.Battle:
                    DrawBattle();
                    break;


                case GameState.Pause:
                    DrawPause();
                    break;


                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion Draw

        #region DrawStates
        /// <summary>
        /// Draw the Main Menu state
        /// </summary>
        private void DrawMainMenu()
        {
            mainMenu.Draw(spriteBatch);

            if (quitActive)
            {
                quitMenu.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Draw the World state
        /// </summary>
        private void DrawWorld()
        {
            sewerBG.Draw(spriteBatch);
            player.Draw(spriteBatch);
            reader.DrawMap(platform, wall, collectible, spriteBatch);
        }

        /// <summary>
        /// Draw the Battle state
        /// </summary>
        private void DrawBattle()
        {
            player.Draw(spriteBatch);
            sewerBG.Draw(spriteBatch);
            BattleManager.Draw(spriteBatch);
        }
        
        /// <summary>
        /// Draw the Pause state
        /// </summary>
        private void DrawPause()
        {
            DrawState(previousState);

            spriteBatch.Draw(
                shadeOverlay,
                screen,
                shadowColor);

            pauseMenu.Draw(spriteBatch);

            if (quitActive)
            {
                quitMenu.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Draw the Game Over state
        /// </summary>
        private void DrawGameOver()
        {
            gameOverMenu.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw a game state
        /// </summary>
        /// <param name="state">Game state to draw</param>
        private void DrawState(GameState state)
        {
            switch (state)
            {
                case GameState.Menu:
                    DrawMainMenu();
                    break;

                case GameState.World:
                    DrawWorld();
                    break;

                case GameState.Battle:
                    DrawBattle();
                    break;

                case GameState.Pause:
                    DrawPause();
                    break;

                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }
        }
        #endregion DrawStates

        // BUTTON FUNCTIONS
        /// <summary>
        /// Set the game state to the game World and start the game from
        /// beginning
        /// </summary>
        private void StartGame()
        {
            previousState = gameState;
            gameState = GameState.World;

            IsMouseVisible = false;
        }

        #region Pause/Unpause
        /// <summary>
        /// Pause the game
        /// </summary>
        private void PauseGame()
        {
            if (gameState != GameState.Pause)
            {
                previousState = gameState;
                gameState = GameState.Pause;

                IsMouseVisible = true;
            }
            
        }

        /// <summary>
        /// Unpause the game
        /// </summary>
        private void UnpauseGame()
        {
            if (gameState == GameState.Pause)
            {
                gameState = previousState;
                previousState = GameState.Pause;

                IsMouseVisible = false;
            }
        }
        #endregion Pause/Unpause
        /// <summary>
        /// Set the game state to the Main Menu
        /// </summary>
        private void MainMenu()
        {
            previousState = gameState;
            gameState = GameState.Menu;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Confirm with the user if they want to quit the game
        /// </summary>
        private void ShowQuitMenu()
        {
            IsMouseVisible = true;
            quitActive = true;
        }

        /// <summary>
        /// If the user does not want to quit the game
        /// </summary>
        private void DenyQuit()
        {
            quitActive = false;
        }

        #region Battle
        private void Battle(Enemy enemy1)
        {
            preBattlePosition = new Vector2(player.X, player.Y);
            player.X = 10;
            player.Y = 100;            
            previousState = gameState;
            gameState = GameState.Battle;
            
            IsMouseVisible = true;

            Enemy one = enemy1;
            Enemy two = new Enemy(GraphicsDevice.Viewport.Width - 100, GraphicsDevice.Viewport.Height - 200, 300, 100, 3, 10, 20, enemy1.Texture);
            Enemy three = new Enemy(GraphicsDevice.Viewport.Width - 100, GraphicsDevice.Viewport.Height - 300, 300, 100, 2, 10, 20, enemy1.Texture);

            //Ally ally = new Ally(10, 50, 50, 20, 3, 50, 50, collectible.Tex);

            List<Enemy> enemies = new List<Enemy>();
            enemies.Add(one);
            enemies.Add(two);
            enemies.Add(three);

            /*List<Ally> allies = new List<Ally>();
            allies.Add(ally);
            */
            BattleManager.StageBattle(
                player,                
                enemies,
                new Menu(pauseMenu.Texture,
                    new Vector2(0, 300),
                    Color.White),
                battleButton,
                GraphicsDevice);
        }
        #endregion Battle
    }
}
