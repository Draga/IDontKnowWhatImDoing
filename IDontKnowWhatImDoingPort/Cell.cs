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
        //public Color Color;

        public byte [] Color = new byte[3];

        public readonly int X;

        public readonly int Y;

        public Cell(byte[] color, int x, int y)
        {
            Color = color;
            X = x;
            Y = y;
        }

        public Cell(int x, int y) : this(new byte[3], x, y)
        {
        }
    }
}
