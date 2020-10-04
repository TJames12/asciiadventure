using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace asciiadventure {
    public class Game {
        private Random random = new Random();
        private static Boolean Eq(char c1, char c2){
            return c1.ToString().Equals(c2.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static string Menu() {
            return "Your goal is to collect all of the treasure(T)\nWASD to move\nIJKL to collect treasure/defuse mines\nBeware of the mobs(#)\nAnd the mines(X)\nQ to quit\n";
        }

        private static void PrintScreen(Screen screen, string message, string menu) {
            Console.Clear();
            Console.WriteLine(screen);
            Console.WriteLine($"\n{message}");
            Console.WriteLine($"\n{menu}");
        }
        public void Run() {
            Console.ForegroundColor = ConsoleColor.White;

            Screen screen = new Screen(10, 10);
            // add a couple of walls
            for (int i=0; i < 3; i++){
                new Wall(1, 2 + i, screen);
            }
            for (int i=0; i < 4; i++){
                new Wall(3 + i, 4, screen);
            }
            
            // add a player
            Player player = new Player(0, 0, screen, "Zelda");
            
            // add a treasure at a random location
            int tRow = random.Next(1,9);
            int tCol = random.Next(1,9);
            Treasure treasure = new Treasure(tRow, tCol, screen);
            

            // add a mine at a random locations
            int mRow = random.Next(3,9);
            int mCol = random.Next(1,9);
            Mine mine = new Mine(mRow, mCol, screen);

            // add some mobs
            List<Mob> mobs = new List<Mob>();
            mobs.Add(new Mob(9, 9, screen));
            
            
            // initially print the game board
            PrintScreen(screen, "Welcome!", Menu());
            
            Boolean gameOver = false;
            
            while (!gameOver) {
                char input = Console.ReadKey(true).KeyChar;

                String message = "";

                if (Eq(input, 'q')) {
                    break;
                } else if (Eq(input, 'w')) {
                    player.Move(-1, 0);
                }  else if (Eq(input, 's')) {
                    player.Move(1, 0);
                } else if (Eq(input, 'a')) {
                    player.Move(0, -1);
                } else if (Eq(input, 'd')) {
                    player.Move(0, 1);
                } else if (Eq(input, 'i')) {
                    message += player.Action(-1, 0) + "\n";
                } else if (Eq(input, 'k')) {
                    message += player.Action(1, 0) + "\n";
                } else if (Eq(input, 'j')) {
                    message += player.Action(0, -1) + "\n";
                } else if (Eq(input, 'l')) {
                    message += player.Action(0, 1) + "\n";
                } else if (Eq(input, 'v')) {
                    // TODO: handle inventory
                    message = "You have nothing\n";
                } else {
                    message = $"Unknown command: {input}";
                }

                if(!treasure.isPresent && player.numT<5) { //add a new treasure when one is collected also add another mob
                    int newTRow = random.Next(1,9);
                    int newTCol = random.Next(1,9);
                    if ((screen[newTRow, newTCol] is GameObject)) { //Check to make sure treasure isnt being added in place of another object
                         newTRow = random.Next(1,9);
                         newTCol = random.Next(1,9);
                    }
                    treasure = new Treasure(newTRow, newTCol, screen);
                    
                    int newMobRow = random.Next(1,9);
                    int newMobCol = random.Next(4,9);
                    if ((screen[newMobRow, newMobCol] is GameObject)) { //Check to make sure mob isnt being added in place of another object
                         newMobRow = random.Next(1,9);
                         newMobCol = random.Next(4,9);
                    }
                    mobs.Add(new Mob(newMobRow, newMobCol, screen));
                }

                    
                

                if (player.numT>4){ //player wins when 5 treasures are collected
                    message += "YOU WIN!\n ALL TREASURES COLLECTED";
                    gameOver = true;
                }

                if (screen[mRow, mCol] is Player){// the player stepped on a mine
                    if (!mine.isDefused){
                        mine.Token = "*";
                        message += "GAME OVER!\nBOOM! YOU STEPPED ON A MINE!\n";
                        gameOver = true;
                    }
                }

                // OK, now move the mobs
                foreach (Mob mob in mobs){
                    // TODO: Make mobs smarter, so they jump on the player, if it's possible to do so
                    List<Tuple<int, int>> moves = screen.GetLegalMoves(mob.Row, mob.Col);
                    if (moves.Count == 0){
                        continue;
                    }
                    // mobs move randomly
                    var (deltaRow, deltaCol) = moves[random.Next(moves.Count)];

                    if (screen[mob.Row + deltaRow, mob.Col + deltaCol] is Mine){
                        // the mob stepped on a mine!
                        mob.Token = "O";
                        message += "BOOM!\n";
                        mob.Delete();
                        mob.isDead = true;
                        mine.isDefused = true;
                        mine.Delete();
                    }
                    
                    if (screen[mob.Row + deltaRow, mob.Col + deltaCol] is Player && !mob.isDead){
                        // the mob got the player!
                        mob.Token = "*";
                        message += "GAME OVER!\n A MOB GOT YOU!\n";
                        gameOver = true;
                    }
                    if (!mob.isDead)
                        mob.Move(deltaRow, deltaCol);
                }

                PrintScreen(screen, message, Menu());
            }
        }

        public static void Main(string[] args){
            Game game = new Game();
            game.Run();
        }
    }
}