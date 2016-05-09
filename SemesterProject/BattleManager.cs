using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SemesterProject
{
    static class BattleManager
    {
        #region Fields/Properties
        // Fields
        private static List<Character> battleRoster;
        private static List<Character> roster = new List<Character>();
        private static Random rand = new Random();
        private static int currentTurn;
        private static ListMenu enemySelectMenu;
        private static List<Enemy> enemyRoster;
        private static Rectangle playerHp;
        private static Player player;
        private static List<Ally> allyList;

        // Properties
        /// <summary>
        /// The roster of all actors currently alive in the battle
        /// </summary>
        public static List<Character> Roster
        {
            get { return roster; }
        }

        public static List<Enemy> EnemyRoster
        {
            get { return enemyRoster; }
        }
        #endregion Fields/Properties

        #region Constructor
        // Methods
        /// <summary>
        /// Stage a new battle scene.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="allies"></param>
        /// <param name="enemies"></param>
        /// <param name="menu"></param>
        /// <param name="button"></param>
        /// <param name="g"></param>
        /// <param name="environment">The level type that the battle is taking
        /// place in. Used to determine what types of enemies to add to the
        /// battle, and what background should be drawn.</param>
        public static void StageBattle(Player player1, List<Enemy> enemies, Menu menu, Button button, GraphicsDevice g/*,
            LevelType environment*/)
        {
            player = player1;
            currentTurn = 0;            
            //roster.Add(player);
            enemyRoster = new List<Enemy>();
            allyList = new List<Ally>();
            // Insert the Player into the roster
            roster.Add(player as Player);

            // Insert the Allies into the roster
            

            // Insert the baddies into the roster
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].X = g.Viewport.Width - enemies[i].Width - 200 + 30 * i;
                enemies[i].Y = g.Viewport.Height - enemies[i].Height - 430 + 150 * i;

                enemyRoster.Add(enemies[i]);
                InsertActor(enemies[i]);

                if (i == 2)
                {
                    i = enemies.Count;
                }
            }

            // Select 3 enemies at most
            Button button0 = new Button(
                button.Texture,
                new Rectangle(0, g.Viewport.Height - button.Texture.Height, button.Texture.Width + 30, button.Texture.Height),
                Select0,
                button.Font,
                "Enemy0" + enemies[0].Health + " / " + enemies[0].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                false,
                true,
                true,
                false);

            Button button1 = new Button(
                button.Texture,
                new Rectangle(0, g.Viewport.Height - 101 - button.Texture.Height, button.Texture.Width + 30, button.Texture.Height),
                Select1,
                button.Font,
                "Enemy1" + enemies[1].Health + " / " + enemies[1].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                false,
                true,
                true,
                false);

            Button button2 = new Button(
                button.Texture,
                new Rectangle(0, g.Viewport.Height - 202 - button.Texture.Height, button.Texture.Width + 30, button.Texture.Height),
                Select2,
                button.Font,
                "Enemy2" + enemies[2].Health + " / " + enemies[2].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                false,
                true,
                true,
                false);

            List<Button> selectButtons = new List<Button>()
            {
                button0,
                button1,
                button2,
            };

            enemySelectMenu = new ListMenu(menu,
                selectButtons,
                Keys.Up,
                Keys.Down,
                Keys.Enter,
                -1,
                true);

            battleRoster = new List<Character>();

            //Populate the battleRoster with 3x the allies and 1x the player (favors allies over players 3:1)
            for (int j = 0; j < roster.Count; j++)
            {
                if (roster[j] is Ally) //adds 3 allies to the battleRoster
                {
                    battleRoster.Add(roster[j]);
                    battleRoster.Add(roster[j]);
                    battleRoster.Add(roster[j]);
                }
                else if (roster[j] is Player || roster[j] is Enemy)
                {
                    battleRoster.Add(roster[j]);
                }
            }

            playerHp = new Rectangle(0, 20, 100, 50);
            
        }
        #endregion Constructor

        #region InsertActor
        /// <summary>
        /// Inset an actor into the proper location within the roster
        /// based upon their speed stat
        /// </summary>
        /// <param name="actor">Character to add to fight</param>
        public static void InsertActor(Character actor)
        {
            if (roster.Count == 0)
            {
                if (actor is Ally)
                {
                    //playerTeam.Add(actor as Ally);
                    roster.Add(actor as Ally);
                }

                else if (actor is Enemy)
                {
                    //enemyTeam.Add(actor as Enemy);
                    roster.Add(actor as Enemy);
                    enemyRoster.Add(actor as Enemy);
                    enemySelectMenu[0].Active = true;
                }
            }

            for (int i = roster.Count - 1; i >= 0; i--) 
            {
                if (roster[i].Speed > actor.Speed)
                {
                    roster.Insert(i + 1, actor);
                }

                if (roster[i].Speed == actor.Speed)
                {
                    if(actor is Ally || actor is Player)
                    {
                        roster.Insert(i, actor);
                        break;
                    }
                    else
                    {
                        roster.Insert(i + 1, actor);
                        break;
                    }
                }
            }
        }
        #endregion InsertActor

        #region Update/Draw
        /// <summary>
        /// Update everything contained within the BattleManager
        /// </summary>
        /// <param name="mouse">The current mouse state</param>
        /// <param name="prevMouse">The previous mouse state</param>
        public static void Update(MouseState mouse, MouseState prevMouse)
        {
            for (int i = 0; i < enemySelectMenu.Count; i++)
            {
                if (i < enemyRoster.Count && enemyRoster[i] != null)
                {
                    enemySelectMenu[i].Text = "Enemy " + i.ToString() + " "
                        + enemyRoster[i].Health + " / "
                        + enemyRoster[i].MaxHealth;
                }

                // If it's not the enemy's turn, update the menu select buttons to check to see if they've been activated.
                if (!(roster[currentTurn] is Enemy))
                {
                    
                        enemySelectMenu[i].Update(mouse, prevMouse);
                    
                         
                    
                }



                    
            }

            for (int i = 0; i < roster.Count; i++)
            {
                roster[i].Update();
            }
            
            RunBattle();
        }

        /// <summary>
        /// Draw everything in the BattleManager
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Character character in roster)
            {
                character.Draw(spriteBatch);
            }
            /*
            enemySelectMenu.Draw(spriteBatch);
            */
            for(int i = 0; i < enemySelectMenu.Count; i++)
            {
                if(enemyRoster[i] != null)
                {
                    enemySelectMenu[i].Draw(spriteBatch);
                }
            }
            
            foreach(Enemy enemy in enemyRoster)
            {
                if(enemy != null)
                {
                    enemy.Draw(spriteBatch);
                }

                
            }

            foreach(Ally ally in allyList)
            {
                if(ally != null)
                {
                    ally.Draw(spriteBatch);
                }
            }
            
            spriteBatch.Draw(enemySelectMenu[0].Texture, playerHp, Color.White);
            spriteBatch.DrawString(enemySelectMenu[0].Font, player.Health + " / " + player.MaxHealth, new Vector2(playerHp.X + 5, playerHp.Y + playerHp.Height / 2), Color.White);
            spriteBatch.DrawString(enemySelectMenu[0].Font, "Player HP:" , new Vector2(playerHp.X + 5, playerHp.Y +1), Color.White);

        }
        #endregion Update/Draw

        #region CheckAlive
        /// <summary>
        /// Checks through the current roster of characters and removes ones that are dead
        /// </summary>
        public static void CheckAlive()
        {
            for(int i = 0; i < roster.Count; i++)
            {
                if(roster[i].Health <= 0)
                {
                    if (roster[i] is Enemy)
                    {
                        enemySelectMenu[enemyRoster.IndexOf((Enemy)roster[i])].Active = false;
                        enemySelectMenu.Remove(enemyRoster.IndexOf((Enemy)roster[i]));
                        //enemyRoster.Remove((Enemy)(roster[i]));
                    }

                    roster.RemoveAt(i); //remove dead character
                    i--; //subtract from i
                }
            }
            
            for(int j = 0; j < enemyRoster.Count; j++)
            {
                if(enemyRoster[j] == null)
                {

                }
                else if(enemyRoster[j].Health <= 0)
                {
                    enemyRoster[j] = null;
                    
                    j--;
                }
            }
        }
        #endregion CheckAlive

        #region RunBattle
        public static void RunBattle()
        {
            CheckAlive();
            if (roster[currentTurn] is Enemy)
            {
                Character target = null;
                while (!(target is Ally) || !(target is Player)) //look for an ally or a player to attack
                {
                    target = battleRoster[rand.Next(battleRoster.Count)];
                }
                
                roster[currentTurn].Attack(target); //run the enemy attack method
                currentTurn = (currentTurn + 1) % roster.Count;

                
            }

            if (roster[currentTurn] is Enemy && !(roster[(roster.Count + -1) % roster.Count] is Enemy))
            {
               DeactivateAllButtons();
            }

            else if (roster[currentTurn] is Ally || roster[currentTurn] is Player)
            {
                ActivateAllButtons();
            }
            /*
            for (int i = 0; i < roster.Count; i++)
            {
                
                Character target = null;
                if (roster[i] is Ally || roster[i] is Player) //if the 
                {
                    int turn = currentTurn;
                    for (int l = 0; l < enemyRoster.Count; l++)
                    {
                        if (enemyRoster[l].Health > 0)
                        {
                            enemySelectMenu[l].Active = true;
                        }
                    }        
                }

                else
                {
                    for(int t = 0; t < enemyRoster.Count; t++)
                    {
                        if(enemyRoster[t].Health > 0)
                        {
                            enemySelectMenu[t].Active = false;
                        }
                    }

                    while (!(target is Ally) || !(target is Player)) //look for an ally or a player to attack
                    {
                        target = battleRoster[rand.Next(battleRoster.Count)];                        
                    }
                    roster[i].Attack(target); //run the enemy attack method
                }
            }*/
            CheckAlive();
        }
        #endregion RunBattle
        /// <summary>
        /// The player selects an Enemy for the Player or Ally to attack
        /// </summary>
        /// <param name="index">Index of Enemy to attack</param>
        private static void PlayerTurn(int index)
        {
            if (index < enemyRoster.Count)
                roster[currentTurn].Attack(enemyRoster[index]);

            currentTurn = (currentTurn + 1) % roster.Count;
        }

        private static void Select0()
        {
            PlayerTurn(0);
        }

        private static void Select1()
        {
            PlayerTurn(1);
        }

        private static void Select2()
        {
            PlayerTurn(2);
        }

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
        //private static void EnemyTurn()
        //{
        //    for (int i = 0; i < enemyTeam.Count; i++)
        //    {
        //        enemyTeam[i].Attack(playerTeam[rand.Next(playerTeam.Count)]);
        //    }
        //}

        public static bool AllDead()
        {
            bool allDead = false;
            int numDead = 0;
            for(int i = 0; i < enemyRoster.Count; i++)
            {
                if(enemyRoster[i] == null)
                {
                    numDead++;
                }
            }
            

            if(numDead == enemyRoster.Count)
            {
                allDead = true;
                
            }

            return allDead;
        }
    }
}