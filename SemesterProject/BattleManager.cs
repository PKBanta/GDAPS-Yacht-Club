using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    /// <summary>
    /// A non-instantiable manager that handles all turn-based battle logic
    /// </summary>
    static class BattleManager
    {
        // Fields
        #region Fields
        private static Player player;
        private static List<Character> roster;  // List of Characters on stage
        private static List<Enemy> enemyRoster; // List of Enemies on stage
        private static List<Ally> allyRoster;   // List of Allies on stage
        private static LevelType environment;

        private static Texture2D healthForeground, healthBackground;

        private static ListMenu enemySelectMenu;
        private static Button battleButton;
        private static Menu menu;

        private static Random rand = new Random();
        private static int currentTurn, subIndex, targetIndex,
            screenHeight = 100, screenWidth = 100;
        private static double totalTime, timeCounter, pauseCounter;
        private static bool battleStarted, turnInProgress, attackExecuted,
            gameOver, battleWon;

        private static Vector2[]
            friendlyLocations
                = new Vector2[3] { Vector2.Zero, Vector2.Zero, Vector2.Zero },
            enemyLocations
                = new Vector2[3] { Vector2.Zero, Vector2.Zero, Vector2.Zero };
        private static Vector2 target, currentBase;

        public static event ActivationFunction gameOverFunction, battleWinFunction;
        #endregion

        // Properties
        #region Properties
        /// <summary>
        /// The list of all Characters currently alive in the battle
        /// </summary>
        public static List<Character> Roster
        {
            get { return roster; }
        }

        /// <summary>
        /// The list of all Enemy Characters currently alive in the battle
        /// </summary>
        public static List<Enemy> EnemyRoster
        {
            get { return enemyRoster; }
        }

        /// <summary>
        /// The list of all Ally Characters currently alive in the battle
        /// </summary>
        public static List<Ally> AllyRoster
        {
            get { return allyRoster; }
        }

        /// <summary>
        /// The background texture of the health bar of newly created Enemies
        /// </summary>
        public static Texture2D Texture_BackgroundHealth
        {
            get { return healthBackground; }
            set { healthBackground = value; }
        }

        /// <summary>
        /// The foreground texture of the health bar of newly created Enemies
        /// </summary>
        public static Texture2D Texture_ForegroundHealth
        {
            get { return healthForeground; }
            set { healthForeground = value; }
        }

        /// <summary>
        /// The width of the screen - used to determine where to draw Enemies
        /// </summary>
        public static int ScreenWidth
        {
            get { return screenWidth; }
            set { screenWidth = value; }
        }

        /// <summary>
        /// The height of the screen - used to determine where to draw Enemies
        /// </summary>
        public static int ScreenHeight
        {
            get { return screenHeight; }
            set { screenHeight = value; }
        }

        public static bool GameOver
        {
            get { return gameOver; }
        }

        /// <summary>
        /// Did the Player win the Battle?
        /// </summary>
        public static bool BattleWon
        {
            get { return battleWon; }
        }

        /// <summary>
        /// The Menu for the battle selection menu
        /// </summary>
        public static Menu BattleMenu
        {
            get { return menu; }
            set { menu = value; }
        }

        /// <summary>
        /// The Button for the battle selection menu
        /// </summary>
        public static Button BattleButton
        {
            get { return battleButton; }
            set { battleButton = value; }
        }
        #endregion


        // Methods
        #region Methods
        /// <summary>
        /// Set up the stage for a new battle scene.
        /// </summary>
        /// <param name="player">The Player</param>
        /// <param name="allies">A list of Allies companioning Player</param>
        /// <param name="enemy">The Enemy that incited the battle beginning
        /// </param>
        /// <param name="levelType">The level type that the battle is taking
        /// place in. Used to determine what types of enemies to add to the
        /// battle, and what background should be drawn.</param>
        /// <param name="numEnemies">The number of enemies to start in this
        /// battle</param>
        public static void StageBattle(Player player, List<Ally> allies,
            Enemy enemy, LevelType levelType, int numEnemies = 3)
        {
            // Ensure that the correct number of enemies are made
            if (numEnemies > 3)
            {
                numEnemies = 3;
            }
            else if (numEnemies < 0)
            {
                numEnemies = 0;
            }

            // Miscellaneous data
            environment = levelType;

            target = Vector2.Zero;
            currentBase = Vector2.Zero;

            currentTurn = -1;
            //targetIndex = -1;
            //subIndex = 0; // The sub-roster index of the current Character

            // Timers
            totalTime = 0;
            timeCounter = 0;
            pauseCounter = 0.63;
            turnInProgress = false;
            attackExecuted = false;
            battleStarted = false;
            gameOver = false;
            battleWon = false;

            // Create the appropriate rosters
            roster = new List<Character>();
            enemyRoster = new List<Enemy>(numEnemies);
            allyRoster = new List<Ally>(allies.Count);

            // Insert the Player into their rosters
            player.Direction = CharDirection.right;
            player.State = CharacterState.b_idle;
            BattleManager.player = player;
            roster.Add(BattleManager.player);

            // Insert the Allies into their rosters
            for (int i = 0; i < allies.Count; i++)
            {
                allies[i].Direction = CharDirection.right;
                allies[i].State = CharacterState.b_idle;

                InsertActor(allies[i] as Ally);
            }

            // Insert the enemies into their rosters
            enemy.Direction = CharDirection.left;
            enemy.State = CharacterState.b_idle;
            InsertActor(enemy);

            while (enemyRoster.Count < numEnemies)
            {
                PopulateByEnvironment();
            }

            // Calculate the locations that all characters should be placed in,
            // and put them where they belong
            CalculateFriendlyBases();
            CalculateEnemyBases(numEnemies);

            player.B_Location = friendlyLocations[friendlyLocations.Length - 1];
            for (int i = 0; i < friendlyLocations.Length - 1; i++)
            {
                allyRoster[i].B_Location = friendlyLocations[i];
            }

            for (int i = 0; i < enemyLocations.Length; i++)
            {
                enemyRoster[i].B_Location = enemyLocations[i];
            }


            // Populate the Buttons
            #region Buttons & Menus
            List<Button> selectButtons = new List<Button>();

            Button button0 = new Button(
                battleButton.Texture,
                new Rectangle(
                    0,
                    screenHeight - battleButton.Texture.Height,
                    battleButton.Texture.Width + 25,
                    battleButton.Texture.Height),
                Select0,
                battleButton.Font,
                enemyRoster[0].ToString() + " " + enemyRoster[0].Health + " / "
                    + enemyRoster[0].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                true,
                true,
                true,
                false);

            selectButtons.Add(button0);

            if (enemyRoster.Count > 1)
            {
                Button button1 = new Button(
                battleButton.Texture,
                new Rectangle(
                    (battleButton.Texture.Width + 25) + 10,
                    screenHeight - battleButton.Texture.Height,
                    battleButton.Texture.Width + 25,
                    battleButton.Texture.Height),
                Select1,
                battleButton.Font,
                enemyRoster[1].ToString() + " " + enemyRoster[1].Health + " / "
                    + enemyRoster[1].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                true,
                true,
                true,
                false);

                selectButtons.Add(button1);

                if (enemyRoster.Count > 2)
                {
                    Button button2 = new Button(
                        battleButton.Texture,
                        new Rectangle(
                            (battleButton.Texture.Width + 25) * 2 + 20,
                            screenHeight - battleButton.Texture.Height,
                            battleButton.Texture.Width + 25,
                            battleButton.Texture.Height),
                        Select2,
                        battleButton.Font,
                        enemyRoster[2].ToString() + " " + enemyRoster[2].Health
                            + " / " + enemyRoster[2].MaxHealth,
                        new Vector2(10, 10),
                        Color.White,
                        true,
                        true,
                        true,
                        false);

                    selectButtons.Add(button2);
                }
            }

            enemySelectMenu = new ListMenu(
                menu,
                selectButtons,
                Keys.Left,
                Keys.Right,
                Keys.Enter,
                -1,
                false);

            DeactivateAllButtons();
            #endregion

            // Set the first turn to zero and setup all appropriate variables
            IncrementTurn();

            // Deactivate all the buttons if the first turn is an Enemy
            if (roster[currentTurn] is Enemy)
            {
                currentBase = enemyLocations[subIndex];
            }

        }

        #region Insert/Remove Methods
        /// <summary>
        /// Inset an actor into the proper location within the roster based upon
        /// their speed stat
        /// </summary>
        /// <param name="actor">Character to add to the battle</param>
        public static void InsertActor(Character actor)
        {
            // If roster is empty, simply add it to the lists.
            if (roster.Count == 0)
            {
                if (actor is Player)
                {
                    roster.Add(actor as Player);
                    player = actor as Player;
                }

                else if (actor is Ally)
                {
                    //else
                    //{
                        roster.Add(actor as Ally);
                        allyRoster.Add(actor as Ally);
                    //}
                }

                else if (actor is Enemy)
                {
                    if (actor is Zombie)
                    {
                        roster.Add(actor as Zombie);
                        enemyRoster.Add(actor as Zombie);
                    }

                    else if (actor is Slime)
                    {
                        roster.Add(actor as Slime);
                        enemyRoster.Add(actor as Slime);
                    }

                    else
                    {
                        roster.Add(actor as Enemy);
                        enemyRoster.Add(actor as Enemy);
                    }
                }
            }

            else
            {
                // Sort the actor into the roster of all Characters
                for (int i = roster.Count - 1; i >= 0; i--)
                {
                    if (roster[i].Speed < actor.Speed)
                    {
                        if (actor is Player)
                        {
                            roster.Insert(i + 1, actor as Player);
                            player = actor as Player;
                            break;
                        }

                        else if (actor is Ally)
                        {
                            roster.Insert(i + 1, actor as Ally);
                            allyRoster.Add(actor as Ally);
                        }

                        else if (actor is Enemy)
                        {
                            if (actor is Zombie)
                            {
                                roster.Insert(i + 1, actor as Zombie);
                                enemyRoster.Add(actor as Zombie);
                            }

                            else if (actor is Slime)
                            {
                                roster.Insert(i + 1, actor as Slime);
                                enemyRoster.Add(actor as Slime);
                            }

                            else
                            {
                                roster.Insert(i + 1, actor as Enemy);
                                enemyRoster.Add(actor as Enemy);
                            }
                            break;
                        }
                    }

                    // If there are contesting speed values, place Enemies into
                    // the list before Friendlies.
                    else if (roster[i].Speed == actor.Speed)
                    {
                        if (actor is Player)
                        {
                            roster.Insert(i + 1, actor as Player);
                            player = actor as Player;
                            break;
                        }

                        else if (actor is Ally)
                        {
                            //else
                            //{
                                roster.Insert(i + 1, actor as Ally);
                                allyRoster.Add(actor as Ally);
                            //}
                            //break;
                        }

                        else if (actor is Enemy)
                        {
                            if (actor is Zombie)
                            {
                                roster.Insert(i, actor as Zombie);
                                enemyRoster.Add(actor as Zombie);
                            }

                            else if (actor is Slime)
                            {
                                roster.Insert(i, actor as Slime);
                                enemyRoster.Add(actor as Slime);
                            }

                            else
                            {
                                roster.Insert(i, actor as Enemy);
                                enemyRoster.Add(actor as Enemy);
                            }
                            break;
                        }
                    }

                    // If the actor doesn't fit anywhere else, insert them into
                    // the beginning of the roster.
                    else if (i == 0)
                    {
                        if (actor is Player)
                        {
                            roster.Insert(0, actor as Player);
                            player = actor as Player;
                            break;
                        }

                        else if (actor is Ally)
                        {
                            //else
                            //{
                                roster.Insert(0, actor as Ally);
                                allyRoster.Add(actor as Ally);
                            //}
                            //break;
                        }

                        else if (actor is Enemy)
                        {
                            if (actor is Zombie)
                            {
                                roster.Insert(0, actor as Zombie);
                                enemyRoster.Add(actor as Zombie);
                            }

                            else if (actor is Slime)
                            {
                                roster.Insert(0, actor as Slime);
                                enemyRoster.Add(actor as Slime);
                            }

                            else
                            {
                                roster.Insert(0, actor as Enemy);
                                enemyRoster.Add(actor as Enemy);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove the Character at index from the battle, and then update
        /// the fields determining whether the battle should continue
        /// </summary>
        /// <param name="index">Index of the Character to remove</param>
        private static void RemoveActor(int index)
        {
            if (roster[index] is IFriendly)
            {
                if (roster[index] is Ally)
                {
                    allyRoster.RemoveAt(FriendlySubIndex(index) - 1);
                }

                else if (roster[index] is Player)
                {
                    gameOver = true;
                }

                roster.RemoveAt(index);

                if (!gameOver)
                {
                    CalculateFriendlyBases();
                }
            }

            else if (roster[index] is Enemy)
            {
                int sub = EnemySubIndex(index);
                enemyRoster.RemoveAt(sub);
                roster.RemoveAt(index);

                if (enemyRoster.Count == 0)
                {
                    battleWon = true;
                    gameOver = true;
                }

                else
                {
                    for (int i = 0; i < enemyRoster.Count; i++)
                    {
                        enemySelectMenu[i].Text =
                            enemyRoster[i].ToString() + " "
                            + enemyRoster[i].Health + " / "
                            + enemyRoster[i].MaxHealth;
                    }

                    CalculateEnemyBases(enemyRoster.Count);
                }
            }
        }
        #endregion


        #region Update/Draw
        /// <summary>
        /// Update everything contained within the BattleManager
        /// </summary>
        /// <param name="elapsedTime">The passed time since the last frame in
        /// seconds</param>
        /// <param name="mouse">The current mouse state</param>
        /// <param name="prevMouse">The previous mouse state</param>
        /// <param name="keyState">The current state of the Keyboard</param>
        /// <param name="prevKeyState">The previous state of the Keyboard
        /// </param>
        public static void Update(double elapsedTime, MouseState mouse,
            MouseState prevMouse, KeyboardState keyState,
            KeyboardState prevKeyState)
        {
            if (!gameOver)
            {
                // Update the total time that has passed
                totalTime += elapsedTime;

                // Once the battle starts, do this stuff
                if (battleStarted)
                {
                    // In between turns/a turn is NOT in progress, pause for a
                    // brief moment before advancing to the next turn
                    if (!turnInProgress)
                    {
                        // If Enemy
                        if (roster[currentTurn] is Enemy)
                        {
                            SelectRandomFriendly();
                        }

                        // If Friendly
                        else if (roster[currentTurn] is IFriendly)
                        {
                            enemySelectMenu.Update(mouse, prevMouse, keyState,
                                prevKeyState);
                        }
                    }

                    // While a turn is in progres, update the attack logistics
                    else
                    {
                        roster[currentTurn].QuickTimeAction.Update(elapsedTime, keyState, prevKeyState);
                        //if (!roster[currentTurn].QuickTimeAction.WindowOpen)
                        //{

                        //}@@@@@@@@@@@@@@@@@@@@@@@@@@WHAT Was I doing here? Was I maybe cancelling the event? We may never know. happy May twelfth: may the force be with you

                        UpdateAttack(elapsedTime);

                        if (attackExecuted)
                        {
                            if (totalTime > timeCounter
                                && roster[targetIndex].Health > 0
                                && roster[targetIndex].Color != Color.White)
                            {
                                roster[targetIndex].Color = Color.White;
                                roster[targetIndex].HealthBar.CurrentColor
                                    = Color.LawnGreen;
                            }

                            else if (roster[targetIndex].Health == 0)
                            {
                                roster[targetIndex].Color = new Color(
                                    roster[targetIndex].Color.R - 1,
                                    roster[targetIndex].Color.G - 1,
                                    roster[targetIndex].Color.B - 1,
                                    roster[targetIndex].Color.A - 1);

                                roster[targetIndex].HealthBar.BaseColor = new Color(
                                    roster[targetIndex].HealthBar.BaseColor.R - 1,
                                    roster[targetIndex].HealthBar.BaseColor.G - 1,
                                    roster[targetIndex].HealthBar.BaseColor.B - 1,
                                    roster[targetIndex].HealthBar.BaseColor.A - 1);

                                roster[targetIndex].HealthBar.TextColor = new Color(
                                    roster[targetIndex].HealthBar.TextColor.R - 1,
                                    roster[targetIndex].HealthBar.TextColor.G - 1,
                                    roster[targetIndex].HealthBar.TextColor.B - 1,
                                    roster[targetIndex].HealthBar.TextColor.A - 1);
                            }
                        }

                    }

                    // If it's not the enemy's turn, update the menu select
                    // buttons to check to see if they've been activated.
                    if (!turnInProgress && roster[currentTurn] is IFriendly)
                    {
                        enemySelectMenu.Update(mouse, prevMouse, keyState,
                            prevKeyState);
                    }

                    // Update all the Characters in the Battle
                    for (int i = 0; i < roster.Count; i++)
                    {
                        roster[i].Update(elapsedTime, keyState, prevKeyState);
                    }
                }

                // The pre-battle time buffer before the battle starts
                else
                {
                    if (totalTime > pauseCounter)
                    {
                        battleStarted = true;

                        if (roster[currentTurn] is IFriendly)
                            ActivateAllButtons();
                    }
                }
            }

            // End the battle when gameOver is true
            else
            {
                if (battleWon)
                    battleWinFunction();
                else
                    gameOverFunction();
            }
        }


        /// <summary>
        /// Run through the motions of updating an attack of the current turn
        /// Character upon a target
        /// </summary>
        /// <param name="elapsedSeconds">Seconds passed since last frame</param>
        private static void UpdateAttack(double elapsedSeconds)
        {
            switch (roster[currentTurn].State)
            {
                // Approach the target until current reaches the target location
                case CharacterState.b_approach:
                    roster[currentTurn].MoveTowards(
                        target,
                        400,
                        elapsedSeconds);

                    // Once the target is reached stop approach & start attack
                    if (roster[currentTurn].B_Location == target)
                    {
                        roster[currentTurn].State = CharacterState.b_attack;
                    }
                    break;


                // Attack the target, dealing damage to the target after the
                // animation is looped through
                case CharacterState.b_attack:
                    if (!attackExecuted && roster[currentTurn].FrameCycleIndex
                        == roster[currentTurn].DamageIndex)
                    {
                        roster[targetIndex].Health
                            -= roster[currentTurn].Damage;
                        attackExecuted = true;

                        // Turn target & their health bar red if damage is taken
                        if (roster[currentTurn].Damage > 0)
                        {
                            // If the current turn is a Friendly and they
                            // successfully executed a charge, turn them gold.
                            if (roster[currentTurn] is IFriendly)
                            {
                                if (roster[currentTurn] is Player)
                                {
                                    if (roster[currentTurn].Damage
                                        > Player.PLAYER_DAMAGE)
                                    {
                                        roster[currentTurn].Color
                                            = Player.chargeColor;
                                    }
                                }
                            }

                            roster[targetIndex].Color = Color.DarkRed;
                            roster[targetIndex].HealthBar.CurrentColor
                                = new Color(187, 8, 13);
                        }

                        // If no damage is taken
                        else
                        {
                            // If the target is a Friendly & the player blocked
                            // the enemy attack, turn the target blue
                            if (roster[targetIndex] is IFriendly)
                            {
                                roster[targetIndex].Color
                                    = Player.dodgeColor;
                            }
                        }

                        timeCounter = totalTime + 0.5;
                    }

                    if (attackExecuted)
                    {
                        //if (totalTime > timeCounter)
                        //{
                        //    roster[targetIndex].Color = Color.White;
                        //    roster[targetIndex].HealthBar.CurrentColor
                        //        = Color.LawnGreen;
                        //}

                        if (roster[targetIndex] is Enemy)
                        {
                            int sub = EnemySubIndex(targetIndex);
                            enemySelectMenu[sub].Text
                                = enemyRoster[sub].ToString() + " "
                                + enemyRoster[sub].Health + " / "
                                + enemyRoster[sub].MaxHealth;
                        }
                    }
                    break;


                // Return towards the current character's base location
                case CharacterState.b_retreat:
                    roster[currentTurn].MoveTowards(
                        currentBase,
                        800,
                        elapsedSeconds);

                    // Return to normal color
                    //if (totalTime > timeCounter && roster[targetIndex].Color != Color.White)
                    //{
                    //    roster[targetIndex].Color = Color.White;
                    //    roster[targetIndex].HealthBar.CurrentColor
                    //        = Color.LawnGreen;
                    //}

                    if (roster[currentTurn] is IFriendly)
                    {
                        if (roster[currentTurn] is Ally
                            && roster[currentTurn].B_Location
                            == friendlyLocations[subIndex - 1]
                            || roster[currentTurn] is Player
                            && roster[currentTurn].B_Location
                            == friendlyLocations[friendlyLocations.Length - 1])
                        {
                            roster[currentTurn].State = CharacterState.b_idle;
                            roster[currentTurn].Direction = CharDirection.right;
                            pauseCounter = totalTime + 1;
                            if (roster[targetIndex].Health <= 0)
                            {
                                pauseCounter += 0.5;
                            }
                        }
                    }
                    else
                    {
                        if (roster[currentTurn].B_Location
                            == enemyLocations[subIndex])
                        {
                            roster[currentTurn].State = CharacterState.b_idle;
                            roster[currentTurn].Direction = CharDirection.left;
                            pauseCounter = totalTime + 1;
                            if (roster[targetIndex].Health <= 0)
                            {
                                pauseCounter += 0.5;
                            }
                        }
                    }
                    break;


                // Remain at rest at base location until pause between turns is
                // over
                case CharacterState.b_idle:
                    if (totalTime > pauseCounter)
                    {
                        totalTime = 0;

                        turnInProgress = false;
                        attackExecuted = false;

                        // If the target is depleted of health, remove them from
                        // the roster. Update whether the battle should continue
                        // or if the battle is over.
                        if (roster[targetIndex].Health <= 0)
                        {
                            if (targetIndex < currentTurn)
                            {
                                currentTurn--;
                            }

                            RemoveActor(targetIndex);
                        }

                        // If the battle is not over
                        if (!battleWon && !gameOver)
                        {
                            IncrementTurn();

                            if (roster[currentTurn] is IFriendly)
                            {
                                ActivateAllButtons();
                            }
                        }

                        else
                        {
                            // What's supposed to go here?@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@idonotknowletmefigurethisoutsomeothertimetho  Update: 3 Days Later: HELP I STILL DON'T KNOW WHAT GOES HERE
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Draw everything in the BattleManager
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            // Draw the menu
            spriteBatch.Draw(
                enemySelectMenu.Texture,
                enemySelectMenu.MenuPosition,
                enemySelectMenu.MenuColor);

            // Draw each button that correlates to an enemy that's still alive
            for (int i = 0; i < enemySelectMenu.Count; i++)
            {
                if (i < enemyRoster.Count && enemyRoster[i] != null)
                {
                    enemySelectMenu[i].Draw(spriteBatch);
                }
            }

            // Draw the enemies
            for (int i = 0; i < roster.Count; i++)
            {
                if (i != currentTurn)
                {
                    roster[i].Draw(spriteBatch);
                }
            }

            // Draw the current turn Character on top of everyone else
            roster[currentTurn].Draw(spriteBatch);

            spriteBatch.DrawString(
                enemySelectMenu.HeaderFont,
                "Current turn: " + currentTurn + " ("
                    + roster[currentTurn].ToString() + ")",
                Vector2.Zero,
                Color.White);
        }
        #endregion Update/Draw


        /// <summary>
        /// Increment the current turn on the roster & set the subIndex to the
        /// corresponding value
        /// </summary>
        private static void IncrementTurn()
        {
            // Increment turn
            currentTurn = (currentTurn + 1) % roster.Count; // If a Character was just killed that has a faster speed than
                                                            // the currentTurn Character, then the currentTurn needs to be
                                                            // If the current turn is a Friendly            // taken down one before this statement is valid.
            if (roster[currentTurn] is IFriendly)
            {
                ActivateAllButtons();
                subIndex = FriendlySubIndex(currentTurn);

                if (roster[currentTurn] is Ally)
                {
                    currentBase = friendlyLocations[subIndex - 1];
                }
                else if (roster[currentTurn] is Player)
                {
                    currentBase
                        = friendlyLocations[friendlyLocations.Length - 1];
                }
            }

            // If the current turn is an Enemy
            else if (roster[currentTurn] is Enemy)
            {
                DeactivateAllButtons();
                subIndex = EnemySubIndex(currentTurn);
                currentBase = enemyLocations[subIndex];
            }
        }


        #region Indexing Methods
        /// <summary>
        /// Get the sub-roster index of an Character in the Enemy Roster
        /// </summary>
        /// <param name="index">An index in the roster</param>
        /// <returns>EnemyRoster index of Character in the roster at index. -1
        /// if doesn't exist.
        /// </returns>
        private static int EnemySubIndex(int index)
        {
            if (index > -1 && index < roster.Count &&
                enemyRoster.Contains((Enemy)roster[index]))
            {
                return enemyRoster.IndexOf((Enemy)roster[index]);
            }

            return -1;
        }

        /// <summary>
        /// Get the sub-roster index of an Character in the Friendly Roster.
        /// (NOTE - this is NOT an index within the AllyRoster!)
        /// </summary>
        /// <param name="index">An index in the roster</param>
        /// <returns>AllyRoster index of Character in the roster at index + 1. 0
        /// if player. -1 if doesn't exist.
        /// </returns>
        private static int FriendlySubIndex(int index)
        {
            if (index > -1 && index < roster.Count)
            {
                if (roster[index] == player)
                    return 0;

                else if (allyRoster.Contains((Ally)roster[index]))
                    return allyRoster.IndexOf((Ally)roster[index]) + 1;
            }

            return -1;
        }
        #endregion


        #region Turn Selection Logic
        /// <summary>
        /// Select a random roster index of an Ally or Player, and set the
        /// target equal to a location to the right of them, and begin the
        /// next turn.
        /// </summary>
        private static void SelectRandomFriendly()
        {
            // Select a random Friendly index with allies favored 3x over Player
            if (allyRoster.Count > 0)
            {
                int select = rand.Next(allyRoster.Count * 3 + 1);

                // If Player is selected
                if (select == 0)
                {
                    target = new Vector2(player.B_Position.Right - 2
                        - player.B_Position.Width / 4, player.B_Position.Top);
                    targetIndex = roster.IndexOf(player);
                }

                // Select an Ally
                else
                {
                    int index = (select - 1) / 3;
                    target = new Vector2(allyRoster[index].B_Position.Right - 2
                        - allyRoster[index].B_Position.Width / 4,
                        allyRoster[index].B_Position.Top);
                    targetIndex = roster.IndexOf(allyRoster[index]);
                }
            }

            // Default to selecting the Player index
            else
            {
                target = new Vector2(player.B_Position.Right - 2
                    - player.B_Position.Width / 4, player.B_Position.Top);
                targetIndex = roster.IndexOf(player);
            }

            turnInProgress = true;

            roster[currentTurn].State = CharacterState.b_approach;
        }


        /// <summary>
        /// The player selects an Enemy for the Player or Ally to attack, and
        /// the next turn begins.
        /// </summary>
        /// <param name="index">Index of Enemy to attack</param>
        private static void PlayerTurn(int index)
        {
            target = new Vector2(enemyRoster[index].B_LocX
                - enemyRoster[index].B_Position.Width + 2,
                enemyRoster[index].B_LocY);
            targetIndex = roster.IndexOf(enemyRoster[index]);

            subIndex = FriendlySubIndex(currentTurn);

            if (roster[currentTurn] is Player)
            {
                target.X += player.B_Position.Width / 4;
            }
            else if (roster[currentTurn] is Ally)
            {
                target.X += allyRoster[subIndex - 1].B_Position.Width / 4;
            }

            roster[currentTurn].State = CharacterState.b_approach;
            turnInProgress = true;

            enemySelectMenu[index].Selected = false;
            DeactivateAllButtons();
        }

        /// <summary>
        /// Attack the first Enemy in the roster
        /// </summary>
        private static void Select0()
        {
            PlayerTurn(0);
        }

        /// <summary>
        /// Attack the second Enemy in the roster
        /// </summary>
        private static void Select1()
        {
            PlayerTurn(1);
        }

        /// <summary>
        /// Attack the third Enemy in the roster
        /// </summary>
        private static void Select2()
        {
            PlayerTurn(2);
        }
        #endregion


        #region Button Activation
        /// <summary>
        /// Activate all attack buttons on living enemies for the player
        /// </summary>
        private static void ActivateAllButtons()
        {
            for (int i = 0; i < enemyRoster.Count; i++)
            {
                if (enemyRoster[i] == null)
                {

                }
                else if (enemyRoster[i].Health > 0)
                {
                    enemySelectMenu[i].Active = true;
                }
            }
        }

        /// <summary>
        /// Make all the attack buttons inactive for the player
        /// </summary>
        private static void DeactivateAllButtons()
        {
            for (int i = 0; i < enemySelectMenu.Count; i++)
            {
                enemySelectMenu[i].Active = false;
            }
        }
        #endregion Button Activation

        #region Health Bar Activation
        /// <summary>
        /// Turn on all health bar texts
        /// </summary>
        private static void ActivateHealthText()
        {
            for (int i = 0; i < roster.Count; i++)
            {
                roster[i].HealthBar.DrawText = true;
            }
        }

        /// <summary>
        /// Turn off all health bar texts
        /// </summary>
        private static void DeactivateHealthText()
        {
            for (int i = 0; i < roster.Count; i++)
            {
                roster[i].HealthBar.DrawText = false;
            }
        }
        #endregion


        /// <summary>
        /// Check to see if all enemies are dead - a condition required for
        /// winning the battle
        /// </summary>
        /// <returns></returns>
        public static bool AllEnemiesDead()
        {
            return (enemyRoster.Count == 0);
        }


        #region Roster Calculation Methods
        /// <summary>
        /// This method is called to randomly populate the Enemy list with an
        /// Enemy based upon what type of environment the current level is.
        /// </summary>
        /// <param name="environment">Level environment</param>
        private static void PopulateByEnvironment()
        {
            if (enemyRoster.Count < 3)
            {
                if (environment == LevelType.sewer)
                {
                    int index = rand.Next(4);
                    switch (index)
                    {
                        // Insert Slime
                        case 0:
                        case 1:
                        case 2:
                            InsertActor(new Slime(
                                enemyLocations[enemyRoster.Count],
                                healthBackground,
                                healthForeground,
                                Slime.SLIME_HEALTH + (rand.Next(6) - 3),
                                Slime.SLIME_HEALTH + (rand.Next(3) - 2),
                                CharacterState.b_idle));
                            break;

                        case 3:
                            InsertActor(new Zombie(
                                enemyLocations[enemyRoster.Count],
                                healthBackground,
                                healthForeground,
                                Zombie.ZOM_HEALTH + (rand.Next(6) - 3),
                                Zombie.ZOM_SPEED + (rand.Next(3) - 1),
                                CharacterState.b_idle));
                            break;
                    }
                }

                else if (environment == LevelType.street)
                {
                    int index = rand.Next(1);
                    switch (index)
                    {
                        // Insert Zombie
                        case 0:
                            InsertActor(new Zombie(
                                enemyLocations[enemyRoster.Count],
                                healthBackground,
                                healthForeground,
                                Zombie.ZOM_HEALTH + (rand.Next(6) - 3),
                                Zombie.ZOM_SPEED + (rand.Next(3) - 1),
                                CharacterState.b_idle));
                            break;

                    }
                }

                // Insert the native Tower Enemy 3x more likely than any other
                // previously found Enemy
                else if (environment == LevelType.skyscraper)
                {
                    int index = rand.Next(4);
                    switch (index)
                    {
                        // Insert native Zombie
                        case 0:
                        case 1:
                        case 2:
                            InsertActor(new Zombie(
                                enemyLocations[enemyRoster.Count],
                                healthBackground,
                                healthForeground,
                                Zombie.ZOM_HEALTH + (rand.Next(6) - 3),
                                Zombie.ZOM_SPEED + (rand.Next(3) - 1),
                                CharacterState.b_idle));
                            break;

                        // Insert Slime
                        case 3:
                            InsertActor(new Slime(
                                enemyLocations[enemyRoster.Count],
                                healthBackground,
                                healthForeground,
                                Slime.SLIME_HEALTH + (rand.Next(6) - 3),
                                Slime.SLIME_HEALTH + (rand.Next(3) - 2),
                                CharacterState.b_idle));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Recalculate the locations of the base locations of all friendlies.
        /// Requires the Player to be in the roster.
        /// </summary>
        private static void CalculateFriendlyBases()
        {
            if (allyRoster.Count == 0)
            {
                friendlyLocations = new Vector2[1]
                {
                    new Vector2(60, screenHeight - player.B_Position.Height - 213)
                };
            }
            else if (allyRoster.Count == 1)
            {
                friendlyLocations = new Vector2[2]
                {
                    new Vector2(50, screenHeight - player.B_Position.Height - 165),
                    new Vector2(75, screenHeight - player.B_Position.Height - 165 - allyRoster[0].B_Position.Height - 50),
                };
            }
            else
            {
                friendlyLocations = new Vector2[3]
                {
                    new Vector2(50, screenHeight - player.B_Position.Height - 100),
                    new Vector2(65, screenHeight - player.B_Position.Height - 100 - allyRoster[0].B_Position.Height - 50),
                    new Vector2(80, screenHeight - player.B_Position.Height - 100 - allyRoster[0].B_Position.Height - 50 - allyRoster[1].B_Position.Height - 50)
                };
            }
        }

        /// <summary>
        /// Recalculate the locations of the base locations of all enemies.
        /// Requires the Player to be in the roster.
        /// </summary>
        private static void CalculateEnemyBases(int numEnemies)
        {
            if (numEnemies == 1)
            {
                enemyLocations = new Vector2[1]
                {
                    new Vector2(screenWidth - 60 - enemyRoster[0].B_Position.Width, screenHeight - enemyRoster[0].B_Position.Height - 213)
                };
            }
            else if (numEnemies == 2)
            {
                enemyLocations = new Vector2[2]
                {
                    new Vector2(screenWidth - 50 - enemyRoster[0].B_Position.Width, screenHeight - enemyRoster[0].B_Position.Height - 165),
                    new Vector2(screenWidth - 75 - enemyRoster[1].B_Position.Width, screenHeight - enemyRoster[0].B_Position.Height - 165 - enemyRoster[1].B_Position.Height - 50)
                };
            }
            else
            {
                enemyLocations = new Vector2[3]
                {
                    new Vector2(screenWidth - 65 - enemyRoster[1].B_Position.Width, screenHeight - enemyRoster[0].B_Position.Height - 100 - enemyRoster[1].B_Position.Height - 50),
                    new Vector2(screenWidth - 50 - enemyRoster[0].B_Position.Width, screenHeight - enemyRoster[0].B_Position.Height - 100),
                    new Vector2(screenWidth - 80 - enemyRoster[2].B_Position.Width, screenHeight - enemyRoster[0].B_Position.Height - 100 - enemyRoster[1].B_Position.Height - 50 - enemyRoster[2].B_Position.Height - 50)
                };
            }
        }
        #endregion
        #endregion
    }
}