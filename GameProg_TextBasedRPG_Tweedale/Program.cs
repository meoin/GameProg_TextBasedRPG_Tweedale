using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProg_TextBasedRPG_Tweedale
{
    internal class Program
    {
        static char[,] map = new char[,] // dimensions defined by following data:
        {
            {'^','^','^','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
            {'^','^','`','`','`','`','*','*','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','~','~','~','`','`','`'},
            {'^','^','`','`','`','*','*','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','~','~','~','`','`','`','`','`'},
            {'^','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
            {'`','`','`','`','~','~','~','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
            {'`','`','`','`','~','~','~','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
            {'`','`','`','~','~','~','~','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','^','^','`','`','`','`','`','`'},
            {'`','`','`','`','`','~','~','~','`','`','`','`','`','`','`','`','`','`','`','`','`','^','^','^','^','`','`','`','`','`'},
            {'`','`','`','`','`','~','~','~','~','`','`','`','`','`','`','`','`','`','`','`','`','`','`','^','^','^','^','`','`','`'},
            {'`','`','`','`','`','`','`','~','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
            {'`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
            {'`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`','`'},
        };

        static char[,] testMap = new char[,] // dimensions defined by following data:
        {
            {'^', '^' },
            {'$', '@' }
        };

        

        // usage: map[y, x]

        // map legend:
        // ^ = mountain
        // ` = grass
        // ~ = water
        // * = trees

        


        static void Main(string[] args)
        {
            int scale = 1;
            bool looping = true;

            //PrintMap(testMap, 1);
            //PrintMap(testMap, 4);
            DisplayMap();

            while (looping) 
            {
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
                        looping = false;
                        Console.WriteLine("Goodbye!");
                    }
                }

                if (looping) 
                {
                    Console.Clear();
                    DisplayMap(scale);
                }

            }
        }

        static void DisplayMap(int scale) 
        {
            DrawHorizontalBorder(map.GetLength(1) * scale, true);

            for (int row = 0; row < map.GetLength(0); row++) 
            {
                for (int dupeRow = 0; dupeRow < scale; dupeRow++) 
                {
                    Console.Write('║');
                    
                    for (int col = 0; col < map.GetLength(1); col++)
                    {
                        for (int dupeCol = 0; dupeCol < scale; dupeCol++)
                        {
                            char c = map[row, col];

                            GetColorForChar(c);

                            Console.Write(map[row, col]);

                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                        
                    Console.WriteLine('║');
                }
                
            }
            DrawHorizontalBorder(map.GetLength(1) * scale, false);
            DisplayLegend();
        }

        static void DisplayMap()
        {
            DrawHorizontalBorder(map.GetLength(1), true);

            for (int row = 0; row < map.GetLength(0); row++)
            {
                Console.Write('║');

                for (int col = 0; col < map.GetLength(1); col++)
                {
                    char c = map[row, col];

                    GetColorForChar(c);

                    Console.Write(map[row, col]);

                    Console.ForegroundColor = ConsoleColor.White;
                        
                }

                Console.WriteLine('║');
                

            }
            DrawHorizontalBorder(map.GetLength(1), false);
            DisplayLegend();
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
            if (c == '`') Console.ForegroundColor = ConsoleColor.Green;
            else if (c == '~') Console.ForegroundColor = ConsoleColor.Blue;
            else if (c == '^') Console.ForegroundColor = ConsoleColor.DarkGray;
            else if (c == '*') Console.ForegroundColor = ConsoleColor.DarkGreen;
            else Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
