using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProduct
{
    public class HMatrix
    {
        private readonly int[,,] bufferC;
        private readonly int sX;
        private readonly int sY;
        private readonly int sizeX;
        private readonly int sizeY;

        public HMatrix(int x, int y, int w, int h, int mSize)
        {
            sX = x;sY = y;
            sizeX = w;sizeY = h;

            bufferC = new int[sX, sY, mSize];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int y = 0; y < size; y++)
                        bufferC[i, j, y] = 1;
        }

    }

    //bufferMatrixC = new ConcurrentDictionary<int, ConcurrentDictionary<int, int>>(Enumerable.Range(0, size * size).
    //       ToDictionary(x => x, x => new ConcurrentDictionary<int, int>(Enumerable.Range(0, size).ToDictionary(y => y, y => 1))));
    //bufferMatrixC = new ConcurrentDictionary<int, long>(Enumerable.Range(0, size * size).ToDictionary(x => x, x => (long)1));
    //C = Enumerable.Range(0, size * size).Select(x => 1).ToArray();
}
