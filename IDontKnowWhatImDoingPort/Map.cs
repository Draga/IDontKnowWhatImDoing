using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDontKnowWhatImDoingPort
{
    using System.Drawing;

    [Serializable]
    class Map
    {
        public int XSize { get; private set; }
        public int YSize { get; private set; }

        public readonly Cell[][] Cells;
        public Map(int xSize, int ySize)
        {
            XSize = xSize;
            YSize = ySize;
            Cells = new Cell[XSize][];
            for (int x = 0; x < XSize; x++)
            {
                Cells[x] = new Cell[YSize];
                for (int y = 0; y < YSize; y++)
                {
                    Cells[x][y] = new Cell();
                    //Cells[i].Color = (x + y) % 2 == 0 ? Color.Red : Color.Blue;
                }
            }
        }

        public Map(Map original)
        {
            XSize = original.XSize;
            YSize = original.YSize;
            Cells = new Cell[XSize][];
            for (int x = 0; x < XSize; x++)
            {
                Cells[x] = new Cell[YSize];
                for (int y = 0; y < YSize; y++)
                {
                    Cells[x][y] = original.Cells[x][y];
                    //Cells[i].Color = (x + y) % 2 == 0 ? Color.Red : Color.Blue;
                }
            }
        }

        public Map Clone()
        {
            return new Map(this);
        }
    }
}
