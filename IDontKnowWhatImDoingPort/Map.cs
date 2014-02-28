using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDontKnowWhatImDoingPort
{
    using System.Drawing;

    class Map
    {
        public int XSize { get; private set; }
        public int YSize { get; private set; }

        public readonly Cell[] Cells;
        public Map(int xSize, int ySize)
        {
            XSize = xSize;
            YSize = ySize;
            Cells = new Cell[XSize * YSize];
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    int i = (y * YSize) + x;
                    Cells[i] = new Cell(null, x, y);
                    //Cells[i].Color = (x + y) % 2 == 0 ? Color.Red : Color.Blue;
                }
            }
        }

        public Map(Map original)
        {
            XSize = original.XSize;
            YSize = original.YSize;
            Cells = new Cell[XSize * YSize];
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new Cell(new[]
                    {
                        original.Cells[i].Color[0],
                        original.Cells[i].Color[1],
                        original.Cells[i].Color[2],
                    },
                    original.Cells[i].X,
                    original.Cells[i].Y);
            }
        }

        public Map Clone()
        {
            return new Map(this);
        }
    }
}
