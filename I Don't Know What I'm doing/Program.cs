using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DontKnowWhatImDoing
{
    class Program
    {
        private const string CharsRange = //"1234567890" +
                                          //"qwertyuiopasdfghjklzxcvbnm" +
                                          "QWERTYUIOPASDFGHJKLZXCVBNM";
        private static int XSize = 50;
        private static int YSize = 50;
        private static Cell[,] Cells;
        static readonly Random Random = new Random();
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            // - 2 Prevents horizontal or vertical taskbar
            XSize = Console.LargestWindowWidth - 3;
            YSize = Console.LargestWindowHeight - 3;
            Cells = new Cell[XSize, YSize];
            Console.SetWindowSize(XSize, YSize);
            InitCells(Cells);
            do
            {
                Cells = RefreshCells(Cells);
                PrintToConsolle(Cells);

                Thread.Sleep(300);
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    InitCells(Cells);
                }
            } while (true);
        }

        private static Cell[,] RefreshCells(Cell[,] cells)
        {
            var newCells = new Cell[XSize, YSize];
            Enumerable.Range(0, XSize).ToList().ForEach(x => Enumerable.Range(0, YSize).ToList().ForEach(y =>
            {
                newCells[x, y] = new Cell((char)Neighbourhood(cells, x, y));
            }));
            return newCells;
        }

        private static int Neighbourhood(Cell[,] cells, int x, int y)
        {
            List<int> values = new List<int>();

            int startPosX = (x - 1 < 0) ? x : x - 1;
            int startPosY = (y - 1 < 0) ? y : y - 1;
            int endPosX = (x >= XSize - 1) ? x : x + 1;
            int endPosY = (y >= YSize - 1) ? y : y + 1;


            // See how many are alive
            for (int rowNum = startPosX; rowNum <= endPosX; rowNum++)
            {
                for (int colNum = startPosY; colNum <= endPosY; colNum++)
                {
                    values.Add(cells[rowNum, colNum].C);
                }
            }
            var mostCommon = values.GroupBy(v => v).OrderByDescending(g => g.Count()).FirstOrDefault();
            return mostCommon != null ? (int)Math.Round(mostCommon.First() + (((double)Random.Next(-1, 2)) * 0.6)) : cells[x, y].C;
            //return (int)Math.Round(values.Average() + Random.Next(-1, 2) * 0.3);
        }

        private static void InitCells(Cell[,] cells)
        {
            Enumerable.Range(0, XSize).ToList().ForEach(x => Enumerable.Range(0, YSize).ToList().ForEach(y =>
            {
                cells[x, y] = new Cell(GetRandomCharacterFromRange());
            }));
        }

        private static void PrintToConsolle(Cell[,] cells)
        {
            Console.CursorLeft = Console.CursorTop = 0;
            var output = string.Empty;
            for (int y = 0; y < cells.GetUpperBound(1); y++)
            {
                for (int x = 0; x < cells.GetUpperBound(0); x++)
                {
                    output += cells[x, y].C;
                }
                output += Environment.NewLine;
            }
            Console.Write(output);
            //lines.ForEach(Console.WriteLine);
        }
        public static char GetRandomChar()
        {
            //int num = Random.Next(31, 127);// random letter or symbol
            int num = Random.Next(60, 90);
            char let = (char)(num);
            return let;
        }
        public static char GetRandomCharacterFromRange()
        {
            int index = Random.Next(CharsRange.Length);
            return CharsRange[index];
        }
    }

    internal class Cell
    {
        public Cell(char c)
        {
            C = c;
        }
        public char C = new char();
    }
}