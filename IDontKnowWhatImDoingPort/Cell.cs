using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDontKnowWhatImDoingPort
{
    using System.Drawing;

    using OpenTK.Graphics;

    class Cell
    {
        public Color Color;

        public int X;

        public int Y;

        public Cell(Color color, int x, int y)
        {
            Color = color;
            X = x;
            Y = y;
        }
    }
}
