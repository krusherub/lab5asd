using System;
using System.Collections.Generic;
using System.Linq;
namespace lab5asd
{
    public class Game
    {
        private List<Player> _players;
        public Player currentTurnPlayer; 
        public Game()
        {
            _players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public Player WhosTurnFirst()
        {
            var random = new Random();
            var index = random.Next(_players.Count);
            return _players[index];
        }

        // передает ход след. игроку, меняет currentTurnPlayer
        private void PassTheMoveToNext()
        {
            int index = -1;
            int i = 0;
            foreach (var player in _players)
            {
                if (player.Equals(currentTurnPlayer))
                {
                    index = i;
                    break;
                }
                i++;
            }

            if (index == _players.Count-1)
            {
                index = 0;
                rounds++;
            }
            else
            {
                index++;
            }

            currentTurnPlayer = _players[index];
        }

        public enum WinCombinations
        {
            General = 60,
            Four = 40,
            FullHouse = 30,
            Streat = 20, 
            None = 0
        }


        public WinCombinations CheckCombination(Player player)
        {
            var A = player.dices.GroupBy(x => x.value).Select(x => x).ToArray();
            var dump = new List<(int key, int count)>();
            foreach (var group in A)
            {
                var a = group.Key;
                var b = group.Count();
                dump.Add((a,b));
            }
            // ничего
            if (dump.Count > 2 && dump.Count < 5)
            {
                return WinCombinations.None;
            }

            if (dump.Count == 1)
            {
                return WinCombinations.General;
            }

            if (dump.Count == 2)
            {
                if (dump.Select(x=>x.count).Any(x => x == 4) )
                {
                    return WinCombinations.Four;
                }
                else
                {
                    return WinCombinations.FullHouse;
                }
            }

            if (dump.Count ==5)
            {
                var a = dump.Select(x => x.key).OrderBy(x => x).ToArray();
                for (int i = 0; i < a.Length-1; i++)
                {
                    if (a[i]-a[i+1]!=-1)
                    {
                        return WinCombinations.None;
                    }
                }

                return WinCombinations.Streat;
            }

            return WinCombinations.None;
        }

        private int rounds;
        public void Start()
        {
            if (_players.Count < 2)
            {
                throw new Exception("Not enogh players");
            }

            currentTurnPlayer = WhosTurnFirst();
            bool playing = true;
            rounds = 1;
            while (playing && rounds < 10 )
            {
                Console.WriteLine("it`s turn: " + currentTurnPlayer.name);
                currentTurnPlayer.RollAllDice();
                if (CheckCombination(currentTurnPlayer) == WinCombinations.None)
                {
                    currentTurnPlayer.WhatToReroll();
                    PassTheMoveToNext();
                    continue;
                }

                if (CheckCombination(currentTurnPlayer) == WinCombinations.General && rounds == 1 )
                {
                    GameFinishGeneral(currentTurnPlayer);
                    return;
                }
                
                currentTurnPlayer.AddScore(CheckCombination(currentTurnPlayer));
                PassTheMoveToNext();
            }

            var bestPlayerResult = _players[0];
            foreach (var player in _players)
            {
                if (player.score > bestPlayerResult.score)
                {
                    bestPlayerResult = player;
                }
            }
            GameFinish(bestPlayerResult);
        }

        private void GameFinish(Player player)
        {
            rounds = int.MaxValue;
            Console.WriteLine("Game finished.");
            Console.WriteLine("Best score: "+ player.score +" Player name: "+ player.name);
        }
        private void GameFinishGeneral(Player player)
        {
            rounds = int.MaxValue;
            Console.WriteLine("Game finished.");
            Console.WriteLine("You have rolled General");
            Console.WriteLine("Win: "+ player.name);
        }
    }


    public class Dice
    {
        public int value = 1;

        public int Roll()
        {
            var random = new Random();
            int value = random.Next(7);
            this.value = value;
            return value;
        }
    }

    public class Player
    {
        public Dice[] dices;
        public int score;
        public string name;
        public Player()
        {
            score = 0;
            dices = new[] {new Dice(), new Dice(), new Dice(), new Dice(), new Dice()};
        }

         /*
         public virtual void MakeTurn()
        {
            
        }
        */

        public void RollAllDice()
        {
            foreach (var dice in dices)
            {
                dice.Roll();
            }

            Console.WriteLine("You have rolled");
            ShowAllDices();
        }

        private void ShowAllDices()
        {
            //Console.WriteLine("Your dices:");
            foreach (var dice in dices)
            {
                Console.Write(dice.value+ " ");
            }
        }

        public void AddScore(Game.WinCombinations combinations)
        {
            score += (int) combinations;
        }

        public virtual void WhatToReroll()
        {
            Console.WriteLine("Your current situation");
            ShowAllDices();
            Console.WriteLine("What dices or dice you want to reroll?");
            foreach (var index in AskForIndexesOfDices())
            {
                dices[index].Roll();
            }
            Console.WriteLine("You rolled: ");
            ShowAllDices();
            ShowScore();
            WhatToScore();
        }

        public virtual void WhatToScore()
        {
            Console.WriteLine("what dice do you want to add to score?");
            var a = int.Parse(Console.ReadLine());
            score += dices.Count(x => x.value == a) * a;
            ShowScore();
        }
        private void ShowScore()
        {
            Console.WriteLine("Your current score: " + score);
        }
         
        private IEnumerable<int> AskForIndexesOfDices() {
            Console.WriteLine("pls write index or indexes of dices");
            var a = Console.ReadLine();
            var indexes = a.Split(',');
            for (int i = 0; i < indexes.Length; i++)
            {
                yield return int.Parse(indexes[i]);
            }
        }
    }



    public class ComputerPlayer : Player
    {
            
    }
    
}