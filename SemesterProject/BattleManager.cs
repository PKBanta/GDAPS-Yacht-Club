using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemesterProject
{
    class BattleManager
    {
        private List<Character> characters;
        private List<Enemy> enemies;
        private List<Character> allies;
        private Random rand;

        public BattleManager(List<Character> chars)
        {
            rand = new Random();
            characters = chars;
            for(int i = 0; i < characters.Count; i++)
            {
                if (characters[i] is Enemy)
                {
                    enemies.Add((Enemy)characters[i]);
                }

                if(characters[i] is Player || characters[i] is Ally)
                {
                    allies.Add(characters[i]);
                }
            }
        }

        public void enemyTurn()
        {
            for(int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Attack(allies[rand.Next(allies.Count)]);
            }

        }

    }
}
