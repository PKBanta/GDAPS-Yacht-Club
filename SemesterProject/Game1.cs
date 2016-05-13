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
            GameOver, // Game Over Screen
            Instructions, //Instructions screen
            GameWon // We may never know what this does
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
        private Texture2D sp_player, sp_khris, sp_zombie, sp_slime;
        private PlayerXState playerXState;      //Player's X direction state
        private PlayerYState playerYState;      //Player's Y direction state
        #endregion Player

        #region Menus/Buttons
        private Texture2D mainMenuImage, pauseImage, gameOverImage, buttonImage,
            quitImage, shadeOverlay/*, streetMenu, sewerMenu, cityMenu*/;
        private List<Texture2D> mainMenuImages;
        private SpriteFont menuFont, buttonFont, healthFont;
        private static Rectangle screen;
        private static Color shadowColor = new Color(200, 200, 200, 255);

        private ListMenu mainMenu, pauseMenu, gameOverMenu, quitMenu, instructionsMenu, gameWinMenu;
        private bool quitActive;

        private Button mainMenu_play, mainMenu_quit, pause_menu, pause_resume,
            pause_quit, quit_no, quit_yes, instructions_Play, instructions_Back, battleButton, winButton;
        private List<Button> mainMenuButtons, pauseButtons, gameOverButtons,
            confirmQuitButtons, instructionsButtons;
        

        private static Vector2 buttonTextLoc = new Vector2(5, 5);
        #endregion Menu/Buttons

        #region Textures/Misc.
        private Texture2D collectibleTexture;
        private Texture2D wallTexture;
        private Texture2D platTexture;
        private Texture2D sewerTexture;
        private Texture2D cityTexture;
        private Texture2D skyLineTexture;
        private Texture2D enemyTexture;
        private Texture2D healthBarBase, healthBarOverlay;
        private Texture2D sewerTexture2;

        private Collectible collectible;
        private Wall wall;
        private Platform platform;
        private MapReader reader;
        
        private Background sewerBG;
        private Background cityBG;
        private Background skyLineBG;

        //the quadtree
        private QuadTreeNode quadTree;
        //the node for Mario
        private QuadTreeNode marioQuad;
        private Random rand;
        //private List<Enemy> enemyList;

        private Enemy killedEnemy;

        #endregion Textures/Misc.

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
            rand = new Random();
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
            
            rand = new Random();
            MapReader.rando = rand;
            
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
            sp_player = Content.Load<Texture2D>("marioSpriteSheet");
            sp_zombie = Content.Load<Texture2D>("ZombieSpriteSheet2");
            sp_slime = Content.Load<Texture2D>("SlimeSpriteSheet");

            // Menu + Misc textures
            mainMenuImages = new List<Texture2D>(3);
            mainMenuImages.Add(Content.Load<Texture2D>("MenuBackground_City"));
            mainMenuImages.Add(Content.Load<Texture2D>("MenuBackground_Sewer"));
            mainMenuImages.Add(Content.Load<Texture2D>("MenuBackground_Skyscraper"));
            mainMenuImage = mainMenuImages[rand.Next(0, mainMenuImages.Count)];

            pauseImage = Content.Load<Texture2D>("pauseMenu");
            //gameOverImage = Content.Load<Texture2D>("mainMenu");
            gameOverImage = mainMenuImages[rand.Next(0, mainMenuImages.Count)];
            quitImage = Content.Load<Texture2D>("QuitMenu");
            buttonImage = Content.Load<Texture2D>("ButtonImage");
            shadeOverlay = Content.Load<Texture2D>("ShadeOverlay");

            menuFont = Content.Load<SpriteFont>("MenuFont");
            buttonFont = Content.Load<SpriteFont>("ButtonFont");

            //Loads map textures
            wallTexture = Content.Load<Texture2D>("wall");
            collectibleTexture = Content.Load<Texture2D>("collectible");
            platTexture = Content.Load<Texture2D>("tile");

            sewerTexture = Content.Load<Texture2D>("sewer bg2");
            sewerTexture2 = Content.Load<Texture2D>("sewer BG");
            cityTexture = Content.Load<Texture2D>("city BG");
            skyLineTexture = Content.Load<Texture2D>("highrise BG");
            enemyTexture = Content.Load<Texture2D>("ghost");

            healthBarBase = Content.Load<Texture2D>("Health Bar Base");
            healthBarOverlay = Content.Load<Texture2D>("Health Bar Overlay");

            healthFont = Content.Load<SpriteFont>("Consolas_9");

            MapReader.healthBarBackground = healthBarBase;
            MapReader.healthBarOverlay = healthBarOverlay;

            Character.Random = rand;

            Player.PlayerSprite = sp_player;
            Player.HealthFont = healthFont;

            Zombie.ZombieSprite = sp_zombie;
            Zombie.HealthFont = healthFont;

            Slime.SlimeSprite = sp_slime;
            Slime.HealthFont = healthFont;

            BattleManager.Texture_BackgroundHealth = healthBarBase;
            BattleManager.Texture_ForegroundHealth = healthBarOverlay;
            BattleManager.ScreenHeight = GraphicsDevice.Viewport.Height;
            BattleManager.ScreenWidth = GraphicsDevice.Viewport.Width;


            //Initializes player and their texture
            player = new Player(Vector2.Zero, healthBarBase, healthBarOverlay);
            player.Health = 14;

            reader.ReadMap("../../../Content/Rooms/room1.txt", quadTree, collectibleTexture, enemyTexture);

            wall = new Wall(0, 0, 25, 25, wallTexture);
            platform = new Platform(0, 0, 25, 25, platTexture);
            collectible = new Collectible(0, 0, 25, 25, collectibleTexture, "item");

            //reader.StoreObjects(platform, wall, collectList, spriteBatch);
            sewerBG = new Background(0, 0, 800, 1200, sewerTexture);
            cityBG = new Background(0, 0, 800, 1200, cityTexture);
            skyLineBG = new Background(0, 0, 800, 1200, skyLineTexture);

            // BUTTONS
            winButton = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2, buttonImage.Width, buttonImage.Height),
                Exit,
                buttonFont,
                "Yay!",
                buttonTextLoc,
                Color.White,
                true,
                true,
                true,
                false
                );

            mainMenu_play = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width - 30, GraphicsDevice.Viewport.Height
                    / 2 - 30, buttonImage.Width, buttonImage.Height),
                ShowInstructions,  // ActivationFunction

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

            instructions_Play = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width / 2 - 350, GraphicsDevice.Viewport.Height / 2
                    + buttonImage.Height / 2 + 10,
                    buttonImage.Width, buttonImage.Height),
                StartGame,  // ActivationFunction

                buttonFont,
                "Play Game",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                false); // linger

            instructions_Back = new Button(
                buttonImage,
                new Rectangle(GraphicsDevice.Viewport.Width / 2
                    - buttonImage.Width / 2 - 25, GraphicsDevice.Viewport.Height / 2
                    + buttonImage.Height / 2 + 10,
                    buttonImage.Width, buttonImage.Height),
                ReturnToMenu,  // ActivationFunction

                buttonFont,
                "Back",
                buttonTextLoc,
                Color.White,

                true,  // active
                true,  // highlightable
                true,  // clickable
                false); // linger

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

            mainMenuButtons = new List<Button>()
            {
                mainMenu_play,
                mainMenu_quit,
                //battleTime
            };

            pauseButtons = new List<Button>()
            {
                pause_resume,
                pause_menu,
                pause_quit
            };

            confirmQuitButtons = new List<Button>()
            {
                quit_no,
                quit_yes
            };

            gameOverButtons = new List<Button>()
            {
                winButton
            };
                

            instructionsButtons = new List<Button>()
            {
                instructions_Back,
                instructions_Play
            };
            
            // Main Menu
            mainMenu = new ListMenu(
                mainMenuImage,
                Vector2.Zero,
                Color.White,

                menuFont,
                "Don't Get Got",
                new Vector2 (GraphicsDevice.Viewport.Width / 2 - 22,
                    GraphicsDevice.Viewport.Height / 4),
                Color.Black,
                
                menuFont,
                "Click or use arrow keys to select an option.",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 150,
                    GraphicsDevice.Viewport.Height / 4 + 20),
                Color.Black,

                mainMenuButtons,
                Keys.Left,
                Keys.Right,
                Keys.Enter,
                -1,
                true);

            instructionsMenu = new ListMenu(
                mainMenuImage,
                Vector2.Zero,
                Color.White,

                menuFont,
                "             Use the WASD keys to move and SPACE to jump.\nReach the final level and defeat the final boss to defeat the game!\n                                                  In Battle:\n        Select an enemy to attack with the buttons on screen.\nBefore an enemy hits you, time hitting Q correctly to avoid damage!\n       Before hitting an enemy, time hitting E to deal extra damage!",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 410,
                    GraphicsDevice.Viewport.Height / 4),
                Color.Black,

                menuFont,
                "Click or use arrow keys to select an option.\n                            Don't get got.",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 330,
                    GraphicsDevice.Viewport.Height / 4 + 180),
                Color.Black,

                instructionsButtons,
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
                gameOverImage,
                Vector2.Zero,
                Color.White,

                menuFont,
                "GAME OVER",
                new Vector2(GraphicsDevice.Viewport.Width / 2
                - pauseImage.Width, GraphicsDevice.Viewport.Height / 5),
                Color.White,

                menuFont,
                "Quit?",
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
            
            gameWinMenu = new ListMenu(
                mainMenuImage,
                Vector2.Zero,
                Color.White,

                menuFont,
                "CONGRATULATIONS!",
                new Vector2(GraphicsDevice.Viewport.Width / 2
                - pauseImage.Width, GraphicsDevice.Viewport.Height / 5),
                Color.Black,

                menuFont,
                "You won, and you didn't get got.",
                new Vector2(GraphicsDevice.Viewport.Width / 2 - 250,
                    GraphicsDevice.Viewport.Height / 2 - 50),
                Color.Black,

                gameOverButtons,
                Keys.Left,
                Keys.Right,
                Keys.Enter,
                0,
                false);

            mainMenu_play.ButtonActivationEvent += mainMenu.Reset;
            pause_resume.ButtonActivationEvent += pauseMenu.Reset;
            pause_menu.ButtonActivationEvent += pauseMenu.Reset;
            quit_no.ButtonActivationEvent += quitMenu.Reset;

            BattleManager.BattleMenu = pauseMenu;
            BattleManager.BattleButton = battleButton;
            BattleManager.gameOverFunction += GameOver;
            BattleManager.battleWinFunction += WinBattle;
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

            marioQuad = quadTree.GetContainingQuad(player.O_Position);

            if (!quitActive)
                switch (gameState)
                {
                    case GameState.Menu:
                        if (!quitActive)
                            mainMenu.Update(mState, previousMState, kbState, previousKBState);
                        break;

                    case GameState.World:
                        if (reader.RoomNumber >= 31)
                        {
                            WinGame();
                        }

                        if (SingleKeyPress(Keys.P)) //Swtch to pause menu
                        {
                            PauseGame();
                        }

                        player.Update(gameTime.ElapsedGameTime.TotalSeconds, kbState, previousKBState);

                        for(int i = 0; i < reader.EnemyList.Count; i++)
                        {
                            if (player.O_Position.Intersects(reader.EnemyList[i].O_Position))
                            {

                                Battle(reader.EnemyList[i]);
                                killedEnemy = reader.EnemyList[i];

                            }
                        }

                        //Put in player collision with enemy here
                        //Player Movement
                        if (kbState.IsKeyDown(Keys.Left))
                        {
                            player.O_X -= 5;
                        }
                        else if (kbState.IsKeyDown(Keys.Right))
                        {
                            player.O_X += 5;
                        }

                        if (kbState.IsKeyDown(Keys.Up))
                        {
                            player.O_Y -= 5;
                        }
                        else if (kbState.IsKeyDown(Keys.Down))
                        {
                            player.O_Y += 5;
                        }

                        //Deals with players x directional movement
                        switch (playerXState)
                        {
                            case PlayerXState.StandRight:
                                if (kbState.IsKeyDown(Keys.D))
                                {
                                    playerXState = PlayerXState.WalkRight;
                                    player.Direction = CharDirection.right;
                                    player.State = CharacterState.o_walk;
                                }

                                else if (kbState.IsKeyDown(Keys.A))
                                {
                                    playerXState = PlayerXState.WalkLeft;
                                    player.Direction = CharDirection.left;
                                    player.State = CharacterState.o_walk;
                                }
                                break;

                            case PlayerXState.WalkRight:
                                player.Move(3);
                                player.UpdateDetectors();

                                if (reader.Right && player.O_X >= GraphicsDevice.Viewport.Width - reader.CurrentRoom.Tile.Width - player.O_Width)
                                {
                                    player.O_X = GraphicsDevice.Viewport.Width - reader.CurrentRoom.Tile.Width - player.O_Width;
                                }
                                
                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Right.Intersects(reader.RectList[i]))
                                    {
                                        player.O_X = reader.RectList[i].X - player.O_Width - 1;
                                        break;
                                    }
                                }

                                if (kbState.IsKeyUp(Keys.D))
                                {
                                    playerXState = PlayerXState.StandRight;
                                    player.State = CharacterState.o_idle;
                                    player.XAcceleration = 1;
                                }
                                break;

                            case PlayerXState.StandLeft:
                                if (kbState.IsKeyDown(Keys.A))
                                {
                                    playerXState = PlayerXState.WalkLeft;
                                    player.Direction = CharDirection.left;
                                    player.State = CharacterState.o_walk;
                                }

                                else if (kbState.IsKeyDown(Keys.D))
                                {
                                    playerXState = PlayerXState.WalkRight;
                                    player.Direction = CharDirection.right;
                                    player.State = CharacterState.o_walk;
                                }
                                break;
                                
                            case PlayerXState.WalkLeft:
                                player.Move(-3);
                                player.UpdateDetectors();
                                //player.State = CharacterState.o_walk;

                                if (reader.Left && player.O_X <= reader.CurrentRoom.Tile.Width)
                                {
                                    player.O_X = reader.CurrentRoom.Tile.Width;
                                }

                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Left.Intersects(reader.RectList[i]))
                                    {
                                        player.O_X = reader.RectList[i].X + player.O_Width + 1;
                                        break;
                                    }
                                }

                                if (kbState.IsKeyUp(Keys.A))
                                {
                                    playerXState = PlayerXState.StandLeft;
                                    player.State = CharacterState.o_idle;
                                    player.XAcceleration = 1;
                                }
                                break;
                        }

                        //Deals with player's y directional movement
                        switch (playerYState)
                        {
                            case PlayerYState.Ground:
                                bool onPlatform = false;

                                for (int i = 0; i < reader.RectList.Count; i++)
                                {
                                    if (player.Below.Intersects(reader.RectList[i]) || player.O_LocY >= GraphicsDevice.Viewport.Height - player.O_Height - reader.CurrentRoom.Tile.Height)
                                    {
                                        onPlatform = true;
                                        break;
                                    }
                                }

                                for (int i = 0; i < reader.ItemList.Count; i++)
                                {
                                    if (player.O_Position.Intersects(reader.ItemList[i].Rect))
                                    {
                                        reader.ItemList[i].Collect(player);
                                    }
                                }

                                if (player.O_LocY >= GraphicsDevice.Viewport.Height - player.O_Height)
                                {
                                    onPlatform = true;
                                }

                                if (!onPlatform)
                                {
                                    player.JumpAcceleration = -2;
                                    playerYState = PlayerYState.Jump;
                                    player.State = CharacterState.o_jump;
                                }

                                if (kbState.IsKeyDown(Keys.Space))
                                {
                                    if (previousKBState.IsKeyUp(Keys.Space))
                                    {
                                        playerYState = PlayerYState.Jump;
                                        player.State = CharacterState.o_jump;
                                    }
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

                                    else if (player.O_Position.Intersects(reader.RectList[i]))
                                    {
                                        player.O_Y = reader.RectList[i].Y - player.O_Height;
                                        player.JumpAcceleration = 17;
                                        playerYState = PlayerYState.Ground;

                                        if (kbState.IsKeyUp(Keys.A) && kbState.IsKeyUp(Keys.D))
                                        {
                                            player.State = CharacterState.o_idle;
                                        }

                                        else
                                        {
                                            if (kbState.IsKeyUp(Keys.A) && kbState.IsKeyDown(Keys.D))
                                            {
                                                player.State = CharacterState.o_walk;
                                                player.Move(5);
                                            }
                                            else if (kbState.IsKeyUp(Keys.D) && kbState.IsKeyDown(Keys.A))
                                            {
                                                player.State = CharacterState.o_walk;
                                                player.Move(-5);
                                            }
                                        }

                                        break;
                                    }
                                }

                                if (reader.Up)
                                {
                                    if(player.O_Y <= reader.CurrentRoom.Tile.Height)
                                    {
                                        player.JumpAcceleration = -2;
                                    }
                                }

                                if (reader.Down)
                                {
                                    if(player.O_Y >= GraphicsDevice.Viewport.Height - player.O_Height - reader.CurrentRoom.Tile.Height)
                                    {
                                        player.O_Y = GraphicsDevice.Viewport.Height - player.O_Height - reader.CurrentRoom.Tile.Height;
                                        playerYState = PlayerYState.Ground;
                                        // IDLE HERE@@@@@@@@@@@@@@@@@@@
                                        player.JumpAcceleration = 17;
                                    }
                                }

                                else if (player.O_Y >= GraphicsDevice.Viewport.Height - player.O_Height)
                                {
                                    player.O_Y = GraphicsDevice.Viewport.Height - player.O_Height;
                                    playerYState = PlayerYState.Ground;
                                    // IDLE HERE@@@@@@@@@@@@@@@@@@@
                                    player.JumpAcceleration = 17;
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
                        if (SingleKeyPress(Keys.P)) //Swtch to pause menu
                        {
                            PauseGame();
                        }

                        BattleManager.Update(gameTime.ElapsedGameTime.TotalSeconds, mState, previousMState, kbState, previousKBState);
                        break;
                    
                    case GameState.GameOver:
                        gameOverMenu.Update(mState, previousMState, kbState,
                            previousKBState);
                        break;
                        
                    case GameState.Instructions:
                        instructionsMenu.Update(mState, previousMState, kbState, previousKBState);
                        break;

                    case GameState.GameWon:
                        gameWinMenu.Update(mState, previousMState, kbState, previousKBState);
                        break;

                }

            if (quitActive)
            {
                quitMenu.Update(mState, previousMState, kbState, previousKBState);
            }
            if (reader.SwitchRoom(player))
            {
                reader.ReadMap("../../../Content/Rooms/room" + reader.RoomNumber +".txt", quadTree, collectibleTexture,enemyTexture);
                if (reader.RoomNumber > 10 && reader.RoomNumber < 21)
                {
                    int temp = rand.Next(2);
                    if (temp == 0)
                    {
                        sewerBG.Tex = sewerTexture;
                    }
                    else
                    {
                        sewerBG.Tex = sewerTexture2;
                    }
                }

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

                case GameState.Instructions:
                    DrawInstructions();
                    break;

                case GameState.GameWon:
                    DrawGameWinYay();
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
            //mainMenuImage = mainMenuImages[rand.Next(0, mainMenuImages.Count)];
            mainMenu.Draw(spriteBatch);

            if (quitActive)
            {
                quitMenu.Draw(spriteBatch);
            }
        }

        private void DrawInstructions()
        {
            instructionsMenu.Draw(spriteBatch);

            if (quitActive)
            {
                quitMenu.Draw(spriteBatch);
            }
        }

        private void DrawGameWinYay()
        {
            gameWinMenu.Draw(spriteBatch);
        }

        /// <summary>
        /// Draw the World state
        /// </summary>
        private void DrawWorld()
        {
            if(reader.RoomNumber < 11)
            {
                cityBG.Draw(spriteBatch);
            }
            else if(reader.RoomNumber >10 && reader.RoomNumber < 21)
            {
                
                sewerBG.Draw(spriteBatch);
            }
            else
            {
                skyLineBG.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);
            reader.DrawMap(platform, wall, collectible, spriteBatch);
            for(int i = 0; i < reader.EnemyList.Count; i++)
            {
                if (reader.EnemyList[i].Health > 0)
                {
                    reader.EnemyList[i].Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Draw the Battle state
        /// </summary>
        private void DrawBattle()
        {
            player.Draw(spriteBatch);
            if(reader.RoomNumber < 11)
            {
                cityBG.Draw(spriteBatch);
            }else if(reader.RoomNumber > 10 && reader.RoomNumber < 21)
            {
                sewerBG.Draw(spriteBatch);
            }
            else
            {
                skyLineBG.Draw(spriteBatch);
            }

            
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
                    //mainMenuImage = mainMenuImages[rand.Next(0, mainMenuImages.Count)];
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
            //int x = rand.Next(0, mainMenuImages.Count);
            mainMenuImage = mainMenuImages[2];

            IsMouseVisible = false;
        }

        private void ShowInstructions()
        {
            previousState = gameState;
            gameState = GameState.Instructions;
        }

        private void ReturnToMenu()
        {
            previousState = gameState;
            gameState = GameState.Menu;
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
            //int i = rand.Next(0, mainMenuImages.Count);
            //int a = rand.Next(0, mainMenuImages.Count);
            //int b = rand.Next(0, mainMenuImages.Count);
            //int x = rand.Next(0, mainMenuImages.Count);
            //mainMenuImage = mainMenuImages[x];

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
            
            //player.O_Location = new Vector2(10, 100);           
            previousState = gameState;
            gameState = GameState.Battle;
            
            IsMouseVisible = true;

            LevelType battleEnvironment;
            if (reader.RoomNumber < 11)
            {
                battleEnvironment = LevelType.street;
            }
            else if (reader.RoomNumber < 21)
            {
                battleEnvironment = LevelType.sewer;
            }
            else
            {
                battleEnvironment = LevelType.skyscraper;
            }

            player.State = CharacterState.b_idle;

            BattleManager.StageBattle(
                player,
                new List<Ally>(),
                enemy1,
                battleEnvironment,
                (/*reader.RoomNumber % 3*/1));
        }

        public void WinBattle()
        {
            player.Health += 1;
            gameState = GameState.World;
            reader.EnemyList.Remove(killedEnemy);
        }

        public void WinGame()
        {
            gameState = GameState.GameWon;
        }

        public void GameOver()
        {
            gameState = GameState.GameOver;
        }
        #endregion Battle
    }
}
