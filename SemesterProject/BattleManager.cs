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
        private static List<Character> roster;
        private static Random rand = new Random();
        
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
        /// <param name="initBaddie">The enemy that incited the battle.
        /// Used to determine how to populate the enemy team, and the difficulty
        /// of the battle.</param>
        /// <param name="environment">The level type that the battle is taking
        /// place in. Used to determine what types of enemies to add to the
        /// battle, and what background should be drawn.</param>
        public static void StageBattle(Player player, Enemy initBaddie/*,
            LevelType environment*/)
        {
            roster.Add(player);
        }

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

        public static void Update()
        {

        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Character character in roster)
            {
                character.Draw(spriteBatch);
            }
        }

        //private static void RandomAttack()
        //{
        //    for (int i = 0; i < enemyTeam.Count; i++)
        //    {
        //        enemyTeam[i].Attack(playerTeam[rand.Next(playerTeam.Count)]);
        //    }
        //}
    }
}