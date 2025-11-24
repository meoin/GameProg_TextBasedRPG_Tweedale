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
        public int maxHP;
        private int health;
        private int attack;
        public int xPos;
        public int yPos;
        public int coins;

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

        public void PickupCoins(int value) 
        {
            coins += value;
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
        public int maxHP;
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

        private bool AdjacentToPlayer(int playerX, int playerY) 
        {
            (int xPos, int yPos)[] possiblePositions = { (xPos + 1, yPos), (xPos - 1, yPos), (xPos, yPos + 1), (xPos, yPos - 1) };

            bool adjacent = false;

            for (int i = 0; i < 4; i++) 
            {
                if (possiblePositions[i].xPos == playerX && possiblePositions[i].yPos == playerY)
                {
                    adjacent = true; break;
                }
            }

            return adjacent;
        }

        public void MoveTowardsPlayer(string[] map, Player player, List<Enemy> enemies)
        {
            int playerX = player.XPos();
            int playerY = player.YPos();

            if (AdjacentToPlayer(playerX, playerY))
            {
                int pushX = xPos + ((playerX - xPos) * 2);
                int pushY = yPos + ((playerY - yPos) * 2);

                //Console.SetCursorPosition(0, map.GetLength(0) + 3);
                player.TakeDamage(GetAttack());

                //Console.WriteLine($"Player X{player.XPos()} Y{player.YPos()}");
                //Console.WriteLine($"Pushed Position: X{pushX} Y{pushY}");

                bool otherEnemyInTargetPosition = false;

                for (int e = 0; e < enemies.Count; e++)
                {
                    if (enemies[e].XPos() == pushX && enemies[e].YPos() == pushY)
                    {
                        otherEnemyInTargetPosition = true;
                    }
                }

                if (map[pushY][pushX] == '`' && !otherEnemyInTargetPosition)
                {
                    player.SetPosition((pushY, pushX));
                }
            }
            else 
            {
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
                            bool otherEnemyInTargetPosition = false;

                            for (int e = 0; e < enemies.Count; e++) 
                            {
                                if (enemies[e].XPos() == newXPos && enemies[e].YPos() == newYPos) 
                                {
                                    otherEnemyInTargetPosition = true;
                                }
                            }

                            if (!otherEnemyInTargetPosition)
                            {
                                lastMovement = (newXPos - xPos, newYPos - yPos);
                                SetPosition((newYPos, newXPos));
                                break;
                            }
                        }
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

        static int xOffset = 1;
        static int yOffset = 3;

        static Player player;

        static List<Enemy> enemies = new List<Enemy>();
        static List<(int x, int y, int timer)> enemySpawners = new List<(int x, int y, int timer)>();
        static List<(int x, int y, int value)> coins = new List<(int x, int y, int value)>();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            bool looping = true;
            ReadMap();

            player = new Player(5, 5, 1, 0, 0);
            player.SetPosition(GetStartPosition());

            enemySpawners.Add((4, 0, 8));
            enemySpawners.Add((3, 10, 12));
            enemySpawners.Add((32, 1, 3));
            enemySpawners.Add((30, 6, 5));
            enemies.Add(new Enemy(3, 1, 5, 0));
            coins.Add((12, 12, 1));
            

            DisplayMap();
            DrawSpawners();
            DrawCoins();
            DrawEnemies();
            DrawPlayer();
            DrawHUD();

            while (looping) 
            {
                //Thread.Sleep(500);
                
                ReadPlayerInput();

                DrawCoins();
                DrawEnemies();
                DrawPlayer();
                DrawHUD();

                Thread.Sleep(300);

                DrawPlayer(true);
                DrawEnemies(true);

                MoveEnemies();
                EnemySpawnerCountdown();

                DrawSpawners();
                DrawCoins();
                DrawEnemies();
                DrawPlayer();
                DrawHUD();

                if (player.GetHealth() <= 0) 
                {
                    looping = false;
                    DrawPlayer(true);
                }
            }

            EndGame();
        }

        static void DrawHUD() 
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("                                                                                                               ");
            Console.SetCursorPosition(0, 0);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Health: {player.GetHealth()}/{player.maxHP}   ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Coins: ${player.coins}");
        }

        static void EndGame() 
        {
            Console.SetCursorPosition(0, map.GetLength(0) + 2 + yOffset);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("You died!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Final coins: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"${player.coins}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        static Enemy EnemyAtPosition(int yPos, int xPos) 
        {
            for (int i = 0; i < enemies.Count; i++) 
            {
                Enemy enemy = enemies[i];
                if (enemy.XPos() == xPos && enemy.YPos() == yPos) 
                {
                    return enemy;
                }
            }

            return null;
        }

        static void AttackEnemy(Enemy enemy, int yPush, int xPush) 
        {
            int newEnemyX = enemy.XPos() + xPush;
            int newEnemyY = enemy.YPos() + yPush;

            DrawEnemy(enemy, true);

            //Console.SetCursorPosition(0, map.GetLength(0) + 3);

            enemy.TakeDamage(player.GetAttack());

            //Console.WriteLine($"Enemy X{enemy.XPos()} Y{enemy.YPos()}");
            //Console.WriteLine($"Pushed Position: X{newEnemyX} Y{newEnemyY}");

            if (map[newEnemyY+yPush][newEnemyX+xPush] == '`')
            {
                enemy.SetPosition((newEnemyY + yPush, newEnemyX + xPush));
            }
            else if (map[newEnemyY + xPush][newEnemyX + yPush] == '`')
            {
                enemy.SetPosition((newEnemyY + xPush, newEnemyX + yPush));
            }
            else if (map[newEnemyY][newEnemyX] == '`')
            {
                enemy.SetPosition((newEnemyY, newEnemyX));
            } 

            if (enemy.CheckIfDead())
            {
                coins.Add((enemy.XPos(), enemy.YPos(), enemy.maxHP));
                enemies.Remove(enemy);
            }
        }

        static void ReadPlayerInput() 
        {
            //if (Console.KeyAvailable)
            //{
            int px = player.XPos();
            int py = player.YPos();

            ConsoleKey input = Console.ReadKey(true).Key;

            DrawPlayer(true);

            if (input == ConsoleKey.W || input == ConsoleKey.UpArrow)
            {
                if (py > 0)
                {
                    if (EnemyAtPosition(py - 1, px) != null) 
                    {
                        AttackEnemy(EnemyAtPosition(py - 1, px), -1, 0);
                    }
                    else if (map[py - 1][px] == '`' || map[py - 1][px] == '_')
                    {
                        player.MoveVertical(-1);
                    }
                }
            }
            else if (input == ConsoleKey.S || input == ConsoleKey.DownArrow)
            {
                if (py < map.GetLength(0) - 1)
                {
                    if (EnemyAtPosition(py + 1, px) != null)
                    {
                        AttackEnemy(EnemyAtPosition(py + 1, px), 1, 0);
                    }
                    else if (map[py + 1][px] == '`' || map[py + 1][px] == '_')
                    {
                        player.MoveVertical(1);
                    }
                }
            }
            else if (input == ConsoleKey.A || input == ConsoleKey.LeftArrow)
            {
                if (px > 0)
                {
                    if (EnemyAtPosition(py, px-1) != null)
                    {
                        AttackEnemy(EnemyAtPosition(py, px - 1), 0, -1);
                    }
                    else if (map[py][px - 1] == '`' || map[py][px - 1] == '_')
                    {
                        player.MoveHorizontal(-1);
                    }
                }
            }
            else if (input == ConsoleKey.D || input == ConsoleKey.RightArrow)
            {
                if (px < map[0].Length - 1)
                {
                    if (EnemyAtPosition(py, px + 1) != null)
                    {
                        AttackEnemy(EnemyAtPosition(py, px + 1), 0, 1);
                    }
                    else if (map[py][px + 1] == '`' || map[py][px + 1] == '_')
                    {
                        player.MoveHorizontal(1);
                    }
                }
            }
            else if (input == ConsoleKey.Spacebar) 
            {
                player.MoveHorizontal(0);
            }

            PickupCoins();
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
            Console.SetCursorPosition(player.XPos()+1, player.YPos()+yOffset);

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
            Console.SetCursorPosition(enemy.XPos() + 1, enemy.YPos() + yOffset);

            GetColorForChar(map[enemy.YPos()][enemy.XPos()]);
            Console.ForegroundColor = ConsoleColor.Red;

            if (!invisible) Console.Write($"{enemy.GetHealth()}");
            else Console.Write(' ');

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void DrawCoins() 
        {
            for (int i = 0; i < coins.Count; i++) 
            {
                Console.SetCursorPosition(coins[i].x + 1, coins[i].y + yOffset);

                GetColorForChar(map[coins[i].y][coins[i].x]);
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write('$');

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void PickupCoins() 
        {
            for (int i = 0; i < coins.Count; i++) 
            {
                if (player.XPos() == coins[i].x && player.YPos() == coins[i].y) 
                {
                    player.PickupCoins(coins[i].value);
                    coins.Remove(coins[i]);
                }
            }
        }

        static void MoveEnemies() 
        {
            foreach (Enemy enemy in enemies) 
            {
                enemy.MoveTowardsPlayer(map, player, enemies);
            }
        }

        static void EnemySpawnerCountdown() 
        {
            for (int i = 0; i < enemySpawners.Count; i++) 
            {
                (int x, int y, int timer) spawner = enemySpawners[i];
                enemySpawners[i] = (spawner.x, spawner.y, spawner.timer-1);

                if (enemySpawners[i].timer <= 0) 
                {
                    Random rand = new Random();
                    int hp = rand.Next(1, 6);

                    enemies.Add(new Enemy(hp, 1, spawner.x, spawner.y+1));

                    enemySpawners[i] = (spawner.x, spawner.y, rand.Next(8, 19));
                }
            }
        }

        static void DrawSpawners()
        {
            for (int i = 0; i < enemySpawners.Count; i++)
            {
                Console.SetCursorPosition(enemySpawners[i].x + 1, enemySpawners[i].y + yOffset);

                GetColorForChar(map[enemySpawners[i].y][enemySpawners[i].x]);
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Write('∩');

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void ReadMap() 
        {
            string path = @"map.txt"; // Get the path of the file as a string

            map = File.ReadAllLines(path); // Read the data from that file path and save it as a string array (where each line is a separate string in the array)
        }

        static void DisplayMap(int scale) 
        {
            Console.SetCursorPosition(xOffset - 1, yOffset - 1);
            
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
