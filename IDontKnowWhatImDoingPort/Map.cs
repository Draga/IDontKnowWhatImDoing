﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDontKnowWhatImDoingPort
{
    using System.Drawing;

    class Map
    {
        public const int XSize = 500;
        public const int YSize = 500;

        public Cell[] Cells = new Cell[XSize * YSize];
        public Map()
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    int i = (y * YSize) + x;
                    Cells[i] = new Cell(new Color(), x, y);
                    //Cells[i].Color = (x + y) % 2 == 0 ? Color.Red : Color.Blue;
                }
            }
        }
    }
}
