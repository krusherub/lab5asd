using System;

namespace lab5asd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Player player = new Player();
            player.name = "kirill";
            Player player2 = new Player();
            player2.name = "Sasha";
     //       player.dices[3].Roll();;
     Game game = new Game();
     game.AddPlayer(player);
     game.AddPlayer(player2);
     game.Start();
        }
    }
}