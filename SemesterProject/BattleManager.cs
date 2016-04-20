using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemesterProject
{
    static class BattleManager
    {
        // Fields
        private static List<Character> roster;
        //private static List<Enemy> enemyTeam;
        //private static List<Character> playerTeam;
        private static Random rand = new Random();
        
        // Properties
        public static List<Character> Roster
        {
            get { return roster; }
        }

        //static BattleManager()
        //{
        //    rand = new Random();
            

        //}

        
        // Methods
        /// <summary>
        /// Set up a new battle scene.
        /// </summary>
        /// <param name="initBaddie">The enemy that incited the battle.
        /// Used to determine how to populate the enemy team, and the difficulty
        /// of the battle.</param>
        /// <param name="environment">The level type that the battle is taking
        /// place in. Used to determine what types of enemies to add to the
        /// battle, and what background should be drawn.</param>
        public static void StartBattle(Player player, Enemy initBaddie,
            LevelType environment)
        {

        }

        public static void AddActor(Character actor)
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

        private static void InsertActor(Character actor)
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
        /// Checks through the current roster of characters and removes ones that are dead
        /// </summary>
        public static void checkAlive()
        {
            for(int i = 0; i < roster.Count; i++)
            {
                if(roster[i].Health <= 0)
                {
                    roster.Remove(roster[i]); //remove dead character
                    i--; //subtract from i
                }
            }
        }


        public static void runBattle()
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

        //private static void EnemyTurn()
        //{
        //    for (int i = 0; i < enemyTeam.Count; i++)
        //    {
        //        enemyTeam[i].Attack(playerTeam[rand.Next(playerTeam.Count)]);
        //    }
        //}
    }
}