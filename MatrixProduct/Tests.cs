using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProduct
{
    public static class Tests
    {
        public static int[,] A2 = new int[,] { { 1, 0 }, { 0, -1 } };
        public static int[,] B2 = new int[,] { { 0, -1 }, { -1, -1 } };

        public static double[,] Ad2 = new double[,] { { 1, 0 }, { 0, -1 } };
        public static double[,] Bd2 = new double[,] { { 0, -1 }, { -1, -1 } };


        public static int[,] A3 = new int[,] { { 0, -2, -2 }, { -2, -2, 0 }, { -2, 0, 1 } };
        public static int[,] B3 = new int[,] { { -2, -1, 0 }, { -1, 0, 2 }, { 0, 2, 2 } };

        public static double[,] Ad3 = new double[,] { { 0, -2, -2 }, { -2, -2, 0 }, { -2, 0, 1 } };
        public static double[,] Bd3 = new double[,] { { -2, -1, 0 }, { -1, 0, 2 }, { 0, 2, 2 } };

        public static int[,] A4 = new int[,] { { -3, -3, -1, 2 }, { -3, -1, 2, 3 }, { -1, 2, 3, 1 }, { 2, 3, 1, -2 } };
        public static int[,] B4 = new int[,] { { -2, 1, 3, 3 }, { 1, 3, 3, 0 }, { 3, 3, 0, 3 }, { 3, 0, -3, -3 } };

        public static double[,] Ad4 = new double[,] { { -3, -3, -1, 2 }, { -3, -1, 2, 3 }, { -1, 2, 3, 1 }, { 2, 3, 1, -2 } };
        public static double[,] Bd4 = new double[,] { { -2, 1, 3, 3 }, { 1, 3, 3, 0 }, { 3, 3, 0, 3 }, { 3, 0, -3, -3 } };
    }
}
