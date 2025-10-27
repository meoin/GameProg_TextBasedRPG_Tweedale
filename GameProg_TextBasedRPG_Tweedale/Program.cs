using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProg_TextBasedRPG_Tweedale
{
    internal class Program
    {
        static char[,] simonMap = new char[,] // dimensions defined by following data:
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
            //PrintMap(testMap, 1);
            //PrintMap(testMap, 2);
            PrintMap(simonMap, 4);
        }

        static void PrintMap(char[,] map, int mult = 1) 
        {
            DrawHorizontalBorder(map.GetLength(1) * mult);

            for (int row = 0; row < map.GetLength(0); row++) 
            {
                for (int dupeRow = 0; dupeRow < mult; dupeRow++) 
                {
                    Console.Write('|');
                    
                    for (int col = 0; col < map.GetLength(1); col++)
                    {
                        for (int dupeCol = 0; dupeCol < mult; dupeCol++)
                        {
                            Console.Write(map[row, col]);
                        }
                    }
                        
                    Console.WriteLine('|');
                }
                
            }
            DrawHorizontalBorder(map.GetLength(1) * mult);
        }

        static void DrawHorizontalBorder(int length) 
        {
            Console.Write('+');

            for (int i = 0; i < length; i++) 
            {
                Console.Write("-");
            }

            Console.WriteLine('+');
        }
    }
}
