using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProduct
{
    public class HMatrix
    {
        private readonly short[,,] bufferC;
        private readonly int sX;
        private readonly int sY;
        private readonly int sizeX;
        private readonly int sizeY;
        private readonly int sizeM;

        public HMatrix(int x, int y, int w, int h, int mSize)
        {
            sX = x;sY = y;
            sizeX = w;sizeY = h; sizeM = mSize;

             bufferC = new short[sizeX, sizeY, sizeM];

            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                    for (int m = 0; m < sizeM;m++)
                        bufferC[i, j, m] = 1;
        }

    }


}
