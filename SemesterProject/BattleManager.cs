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

        //private static void EnemyTurn()
        //{
        //    for (int i = 0; i < enemyTeam.Count; i++)
        //    {
        //        enemyTeam[i].Attack(playerTeam[rand.Next(playerTeam.Count)]);
        //    }
        //}
    }
}