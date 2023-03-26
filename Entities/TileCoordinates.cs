using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Entities
{
    public class TileCoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileCoordinates()
        {
            X = 0;
            Y = 0;
        }

        public TileCoordinates(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
