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
        // Fields
        private static List<Character> roster = new List<Character>();
        private static Random rand = new Random();
        private static int currentTurn;
        private static ListMenu enemySelectMenu;
        private static List<Enemy> enemyRoster;

        // Properties
        /// <summary>
        /// The roster of all actors currently alive in the battle
        /// </summary>
        public static List<Character> Roster
        {
            get { return roster; }
        }


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
        public static void StageBattle(Player player, List<Ally> allies, List<Enemy> enemies, Menu menu, Texture2D button, GraphicsDevice g/*,
            LevelType environment*/)
        {
            roster.Add(player);
            enemyRoster = new List<Enemy>();
            
            // Insert the Player into the roster
            roster.Add(player as Player);

            // Insert the Allies into the roster
            for (int i = 0; i < allies.Count; i++)
            {
                allies[i].X = 200 - 30 * i;
                allies[i].Y = 230 - 10 * i;

                InsertActor(allies[i] as Ally);
            }

            // Insert the baddies into the roster
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].X = g.Viewport.Width - enemies[i].Width - 200 + 30 * i;
                enemies[i].Y = g.Viewport.Height - enemies[i].Height - 230 + 10 * i;

                enemyRoster.Add(enemies[i]);
                InsertActor(enemies[i]);

                if (i == 2)
                {
                    i = enemies.Count;
                }
            }

            // Select 3 enemies at most
            Button button0 = new Button(
                button,
                new Rectangle(0, g.Viewport.Height, 300, 100),
                Select0,
                menu.BodyFont,
                "Enemy0" + enemies[0].Health + " / " + enemies[0].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                false,
                true,
                true,
                false);

            Button button1 = new Button(
                button,
                new Rectangle(0, g.Viewport.Height + 101, 300, 100),
                Select1,
                menu.BodyFont,
                "Enemy1" + enemies[1].Health + " / " + enemies[1].MaxHealth,
                new Vector2(10, 10),
                Color.White,
                false,
                true,
                true,
                false);

            Button button2 =
                new Button(button,
                new Rectangle(0, g.Viewport.Height + 202, 300, 100),
                Select2,
                menu.BodyFont,
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
        }

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
                }
            }

            for (int i = roster.Count - 1; i >= 0; i--) 
            {
                if(roster[i].Speed > actor.Speed)
                {
                    roster.Insert(i++, actor);
                }
                if(roster[i].Speed == actor.Speed)
                {
                    if(actor is Ally || actor is Player)
                    {
                        roster.Insert(i, actor);
                    }
                    else
                    {
                        roster.Insert(i++, actor);
                    }
                }
            }
        }
        
        /// <summary>
        /// Update everything contained within the BattleManager
        /// </summary>
        /// <param name="mouse">The current mouse state</param>
        /// <param name="prevMouse">The previous mouse state</param>
        public static void Update(MouseState mouse, MouseState prevMouse)
        {
            for (int i = 0; i < enemySelectMenu.Count; i++)
            {
                if (enemyRoster.Count < i)
                {
                    enemySelectMenu[i].Text = "Enemy" + i.ToString() + " "
                        + enemyRoster[i].Health + " / "
                        + enemyRoster[i].MaxHealth;
                }

                if (!(roster[currentTurn] is Enemy))
                    enemySelectMenu[i].Update(mouse, prevMouse);
            }

            for (int i = 0; i < roster.Count; i++)
            {
                roster[i].Update();
            }
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

            enemySelectMenu.Draw(spriteBatch);
        }
        
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
                        enemyRoster.Remove((Enemy)(roster[i]));
                    }

                    roster.RemoveAt(i); //remove dead character
                    i--; //subtract from i
                }
            }
        }

        public static void RunBattle()
        {
            List<Character> battleRoster = new List<Character>();

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


            for (int i = 0; i < roster.Count; i++)
            {
                Character target = null;
                if (roster[i] is Ally || roster[i] is Player) //if the 
                {
                    /* If we keep the character method, this chooses a target for allies/player randomly 
                    while(!(target is Enemy))
                    {
                        target = battleRoster[rand.Next(battleRoster.Count)];
                    }*/

                    //roster[i].Attack(); //runs the attack method for ally/player THIS NEEDS TO BE IMPLEMENTED
                }
                else
                {
                    while (!(target is Ally) || !(target is Player)) //look for an ally or a player to attack
                    {
                        target = battleRoster[rand.Next(battleRoster.Count)];
                    }
                    roster[i].Attack(target); //run the enemy attack method
                }
            }
        }

        /// <summary>
        /// The player selects an Enemy for the Player or Ally to attack
        /// </summary>
        /// <param name="index">Index of Enemy to attack</param>
        private static void PlayerTurn(int index)
        {
            if (index < enemyRoster.Count)
                roster[currentTurn].Attack(enemyRoster[index]);
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

        //private static void EnemyTurn()
        //{
        //    for (int i = 0; i < enemyTeam.Count; i++)
        //    {
        //        enemyTeam[i].Attack(playerTeam[rand.Next(playerTeam.Count)]);
        //    }
        //}
    }
}