using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameProg_TextBasedRPG_Tweedale
{
    internal class Player 
    {
        private int maxHP;
        private int health;
        private int attack;
        public int xPos;
        public int yPos;

        public (int x, int y) lastMovement = (0, 0);

        public Player(int maxhp, int hp, int att, int x, int y) 
        {
            maxHP = maxhp;
            health = hp; 
            attack = att; 
            xPos = x; 
            yPos = y;
        }

        public int GetHealth() { return health; }
        public int GetAttack() { return attack; }
        public int XPos() { return xPos; }
        public int YPos() { return yPos; }

        public void Heal(int heal) 
        {
            health = Math.Min(health + heal, maxHP);
        }

        public void TakeDamage(int dmg) 
        {
            health = Math.Max(health - dmg, 0);
        }

        public void MoveHorizontal(int movement) 
        {
            xPos = xPos + movement;
            lastMovement = (movement, 0);
        }
        public void MoveVertical(int movement) 
        {
            yPos = yPos + movement;
            lastMovement = (0, movement);
        }

        public void SetPosition((int, int) pos) 
        {
            yPos = pos.Item1;
            xPos = pos.Item2;
        }
    }

    internal class Enemy 
    {
        private int maxHP;
        private int health;
        private int attack;
        public int xPos;
        public int yPos;

        public (int x, int y) lastMovement = (0, 0);

        public Enemy(int hp, int att, int x, int y)
        {
            health = hp;
            attack = att;
            xPos = x;
            yPos = y;

            maxHP = hp;
        }
        public int GetHealth() { return health; }
        public int GetAttack() { return attack; }
        public int XPos() { return xPos; }
        public int YPos() { return yPos; }

        public void Heal(int heal)
        {
            health = Math.Min(health + heal, maxHP);
        }

        public void TakeDamage(int dmg)
        {
            health = Math.Max(health - dmg, 0);
        }

        public bool CheckIfDead() 
        {
            return health <= 0;
        }

        public void MoveHorizontal(int movement)
        {
            xPos = xPos + movement;
            lastMovement = (movement, 0);
        }
        public void MoveVertical(int movement)
        {
            yPos = yPos + movement;
            lastMovement = (0, movement);
        }

        public void MoveTowardsPlayer(string[] map, int playerX, int playerY)
        {
            /*int xDistance = playerX - xPos;
            int yDistance = playerY - yPos;

            if (Math.Abs(xDistance) > Math.Abs(yDistance) && xDistance > 0 && map[yPos][xPos+1] == '`') 
            {
                MoveHorizontal(1);
            } 
            else if (Math.Abs(xDistance) > Math.Abs(yDistance) && xDistance < 0 && map[yPos][xPos-1] == '`')
            {
                MoveHorizontal(-1);
            }
            else if (Math.Abs(xDistance) <= Math.Abs(yDistance) && yDistance > 0 && map[yPos+1][xPos] == '`')
            {
                MoveVertical(1);
            }
            else if (Math.Abs(xDistance) <= Math.Abs(yDistance) && yDistance < 0 && map[yPos-1][xPos] == '`')
            {
                MoveVertical(-1);
            }*/

            (int xPos, int yPos)[] possiblePositions = { (xPos + 1, yPos), (xPos - 1, yPos), (xPos, yPos + 1), (xPos, yPos - 1) };
            (int index, int distance)[] sortedPositions = new (int, int)[4];

            for (int i = 0; i < 4; i++) 
            {
                int xDistance = playerX - possiblePositions[i].xPos;
                int yDistance = playerY - possiblePositions[i].yPos;

                int totalDistance = Math.Abs(xDistance) + Math.Abs(yDistance);

                sortedPositions[i] = (i, totalDistance);
            }

            Array.Sort(sortedPositions, (x, y) => x.distance.CompareTo(y.distance));

            if (!(xPos == playerX && yPos == playerY)) 
            {
                for (int i = 0; i < 4; i++)
                {
                    int newXPos = possiblePositions[sortedPositions[i].index].xPos;
                    int newYPos = possiblePositions[sortedPositions[i].index].yPos;

                    if (map[newYPos][newXPos] == '`')
                    {
                        lastMovement = (newXPos - xPos, newYPos - yPos);
                        SetPosition((newYPos, newXPos));
                        break;
                    }
                }
            }
        }

        public void SetPosition((int, int) pos)
        {
            yPos = pos.Item1;
            xPos = pos.Item2;
        }
    }

    internal class Program
    {
        static int scale = 1;
        static string[] map;

        static string[] testMap = new string[] // dimensions defined by following data:
        {
            "^^", 
            "$@"
        };

        static Player player;

        static List<Enemy> enemies = new List<Enemy>();

        // usage: map[y, x]

        // map legend:
        // ^ = mountain
        // ` = grass
        // ~ = water
        // * = trees

        


        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            bool looping = true;
            ReadMap();

            player = new Player(5, 5, 1, 0, 0);
            player.SetPosition(GetStartPosition());

            enemies.Add(new Enemy(1, 1, 5, 0));
            
            DisplayMap();
            DrawPlayer();
            DrawEnemies();

            while (looping) 
            {
                Thread.Sleep(500);
                
                ReadPlayerInput();
                

                foreach (Enemy enemy in enemies) 
                {
                    {
                        if (enemy.XPos() == player.XPos() && enemy.YPos() == player.YPos())
                        {
                            enemy.TakeDamage(player.GetAttack());

                            (int y, int x) pushedPosition = (enemy.YPos() + (player.lastMovement.y * 2), enemy.XPos() + (player.lastMovement.x * 2));

                            if (map[pushedPosition.y][pushedPosition.y] == '`') 
                            {
                                enemy.SetPosition(pushedPosition);
                            }

                            if (enemy.CheckIfDead()) 
                            {
                                enemies.Remove(enemy);
                            }
                        }
                    }
                }

                DrawEnemies(true);
                MoveEnemies();

                foreach (Enemy enemy in enemies)
                {
                    {
                        if (enemy.XPos() == player.XPos() && enemy.YPos() == player.YPos())
                        {
                            Console.SetCursorPosition(0, map.GetLength(0) + 3);
                            player.TakeDamage(enemy.GetAttack());

                            (int y, int x) pushedPosition = (player.YPos() + (enemy.lastMovement.y * 2), player.XPos() + (enemy.lastMovement.x * 2));

                            Console.WriteLine($"Player X{player.XPos()} Y{player.YPos()}");
                            Console.WriteLine($"Pushed Position: X{pushedPosition.x} Y{pushedPosition.y}");

                            if (map[pushedPosition.y][pushedPosition.x] == '`')
                            {
                                Console.WriteLine("Pushed!");

                                player.SetPosition(pushedPosition);
                            }

                            Console.SetCursorPosition(0, map.GetLength(0));
                            Console.Write("Player hurt, HP is at " + player.GetHealth());

                            /*if (enemy.CheckIfDead())
                            {
                                enemies.Remove(enemy);
                            }*/
                        }
                    }
                }

                DrawPlayer();
                DrawEnemies();
            }
        }

        static void ReadPlayerInput() 
        {
            if (Console.KeyAvailable)
            {
                int px = player.XPos();
                int py = player.YPos();

                ConsoleKey input = Console.ReadKey(true).Key;

                DrawPlayer(true);

                if (input == ConsoleKey.W || input == ConsoleKey.UpArrow) 
                {
                    if (py > 0) 
                    {
                        if (map[py - 1][px] == '`' || map[py - 1][px] == '_') 
                        {
                            player.MoveVertical(-1);
                        }
                    }
                }
                else if (input == ConsoleKey.S || input == ConsoleKey.DownArrow)
                {
                    if (py < map.GetLength(0)-1)
                    {
                        if (map[py + 1][px] == '`' || map[py + 1][px] == '_')
                        {
                            player.MoveVertical(1);
                        }
                    }
                }
                else if (input == ConsoleKey.A || input == ConsoleKey.LeftArrow)
                {
                    if (px > 0)
                    {
                        if (map[py][px-1] == '`' || map[py][px - 1] == '_')
                        {
                            player.MoveHorizontal(-1);
                        }
                    }
                }
                else if (input == ConsoleKey.D || input == ConsoleKey.RightArrow)
                {
                    if (px < map[0].Length-1)
                    {
                        if (map[py][px + 1] == '`' || map[py][px + 1] == '_')
                        {
                            player.MoveHorizontal(1);
                        }
                    }
                }

                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
        }

        static bool OldMapScalingCode() 
        {
            //DisplayLegend();
            Console.WriteLine();
            if (scale > 1) Console.Write($"{scale - 1} <--");
            else Console.Write("     ");
            Console.Write($"    {scale}    ");
            Console.WriteLine($"--> {scale + 1}");
            Console.WriteLine("\n     Esc = Quit");

            bool readinginput = true;

            while (readinginput)
            {
                ConsoleKey input = Console.ReadKey(true).Key;

                if (input == ConsoleKey.LeftArrow && scale > 1)
                {
                    scale--;
                    readinginput = false;
                }
                else if (input == ConsoleKey.RightArrow)
                {
                    scale++;
                    readinginput = false;
                }
                else if (input == ConsoleKey.Escape)
                {
                    readinginput = false;
                    Console.WriteLine("Goodbye!");
                    return false;
                }
            }

            Console.Clear();
            DisplayMap(scale);
            DisplayLegend();
            return true;
        }

        static void DrawPlayer(bool invisible = false) 
        {
            Console.SetCursorPosition(player.XPos()+1, player.YPos()+1);

            GetColorForChar(map[player.YPos()][player.XPos()]);
            Console.ForegroundColor = ConsoleColor.Red;

            if (!invisible) Console.Write('Ö');
            else Console.Write(' ');

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void DrawEnemies(bool invisible = false) 
        {
            foreach (Enemy enemy in enemies) 
            {
                DrawEnemy(enemy, invisible);
            }
        }

        static void DrawEnemy(Enemy enemy, bool invisible = false)
        {
            Console.SetCursorPosition(enemy.XPos() + 1, enemy.YPos() + 1);

            GetColorForChar(map[enemy.YPos()][enemy.XPos()]);
            Console.ForegroundColor = ConsoleColor.Red;

            if (!invisible) Console.Write('X');
            else Console.Write(' ');

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void MoveEnemies() 
        {
            foreach (Enemy enemy in enemies) 
            {
                enemy.MoveTowardsPlayer(map, player.XPos(), player.YPos());
            }
        }

        static void ReadMap() 
        {
            string path = @"map.txt"; // Get the path of the file as a string

            map = File.ReadAllLines(path); // Read the data from that file path and save it as a string array (where each line is a separate string in the array)
        }

        static void DisplayMap(int scale) 
        {
            DrawHorizontalBorder(map[0].Length * scale, true);

            for (int row = 0; row < map.GetLength(0); row++) 
            {
                for (int dupeRow = 0; dupeRow < scale; dupeRow++) 
                {
                    Console.Write('║');
                    
                    for (int col = 0; col < map[0].Length; col++)
                    {
                        for (int dupeCol = 0; dupeCol < scale; dupeCol++)
                        {
                            char c = '#';

                            if (map[row].Length > col) 
                            {
                                c = map[row][col];
                            }

                            GetColorForChar(c);

                            Console.Write(' ');

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                    }
                        
                    Console.WriteLine('║');
                }
            }
            DrawHorizontalBorder(map[0].Length * scale, false);
            
        }

        static void DisplayMap()
        {
            DisplayMap(1);
        }

        static void DrawHorizontalBorder(int length, bool top) 
        {
            if (top) Console.Write('╔');
            else Console.Write('╚');

                for (int i = 0; i < length; i++)
                {
                    Console.Write("═");
                }

            if (top) Console.WriteLine('╗');
            else Console.WriteLine('╝');
        }

        static void DisplayLegend()
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("` = Grass");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("~ = Water");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("^ = Mountain");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("* = Trees");

            Console.ForegroundColor = ConsoleColor.White;
        }

        static void GetColorForChar(char c) 
        {
            Console.ForegroundColor = ConsoleColor.Black;

            if (c == '`') Console.BackgroundColor = ConsoleColor.Green;
            else if (c == '~') Console.BackgroundColor = ConsoleColor.Blue;
            else if (c == '^') Console.BackgroundColor = ConsoleColor.DarkGray;
            else if (c == '*') Console.BackgroundColor = ConsoleColor.DarkGreen;
            else if (c == '#') Console.BackgroundColor = ConsoleColor.Magenta;
            else if (c == '_') Console.BackgroundColor = ConsoleColor.Green;
            else Console.BackgroundColor = ConsoleColor.White;
        }

        static (int, int) GetStartPosition() 
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map[0].Length; col++)
                {  
                    char c = '#';

                    if (map[row].Length > col)
                    {
                        c = map[row][col];
                    }

                    if (c == '_') 
                    {
                        return (row, col);
                    }
                      
                }
            }

            return (0, 0);
        }
    }
}
