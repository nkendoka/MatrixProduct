using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace MatrixProduct
{
    public class MxOperation
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly InvCloudService cService;
        private readonly int size;
        private Matrix<double> A;
        private Matrix<double> B;
        private double[,] C;
        public MxOperation(int mSize)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info($"Request to Initialize DataSet: {mSize}.");
            cService = new InvCloudService();
            size = mSize;
            var resp = cService.Init(size);
            log.Info(resp.Success?$"Initialized Successfully: {resp.Value}.":"Error Initialization.");
        }

        public void LoadData()
        {
            log.Info($"LoadData started.");
            var rowsA = new ConcurrentDictionary<int, double[]>();
            var rowsB = new ConcurrentDictionary<int, double[]>();

            var result = Parallel.For(0, size, (i, state) => {

                if (state.ShouldExitCurrentIteration)
                {
                    if (state.LowestBreakIteration < i)
                        return;
                }

                rowsA[i] = cService.GetRowData("A", i);
                rowsB[i] = cService.GetRowData("B", i);
            });

            var da = rowsA.OrderBy(x => x.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value).Values.ToArray();
            var db  = rowsB.OrderBy(x => x.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value).Values.ToArray();

            A = DenseMatrix.OfRowArrays(da);
            B = DenseMatrix.OfRowArrays(db);

            log.Info($"LoadData complete.");
        }

        public void Calculate()
        {
            log.Info($"Matrix multiplying started.");
            mProduct();
            log.Info($"Matrix multiplying complete.");
        }

        public void Validate()
        {
            log.Info($"Compute hash started.");
            var strf = FormatM(C);
            var hs = MD5Hash(strf);
            log.Info($"Hash compute complete.");

            log.Info($"Validating hash.");
            var resp = cService.Validate(hs);
            log.Info($"Validation response: {resp.Value}. {nameof(resp.Success)}: {resp.Success}");
        }

        private long[,] mProduct(int[,] A, int[,] B)
        {
            var size = A.GetLength(0);
            var C = new long[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 0; k < size; k++)
                        C[i, j] += A[i, k] * B[k, j];

            return C;
        }

        private double[,] mProduct()
        {
            var mC = A.Multiply(B);
            C = mC.ToArray();

            return C;
        }

        //Joined by colulmn then by row into a single string without separators and then hashed.
        private string FormatM(double[,] A)
        {
            var retval = string.Empty;
            var matrixFormat = new ConcurrentDictionary<int, string>();
            var result = Parallel.For(0, size, (i, state) => {

                if (state.ShouldExitCurrentIteration)
                {
                    if (state.LowestBreakIteration < i)
                        return;
                }

                var row = GetRow(i);
                var newSet = string.Join(null, row.ToList());
                var res = matrixFormat.TryAdd(i, newSet);
            });

            retval = string.Join(null, matrixFormat.OrderBy(x => x.Key).Select(y => y.Value));
            return retval;
        }

        private double[] GetRow(int row)
        {
            int cols = C.GetUpperBound(1) + 1;
            double[] result = new double[cols];

            int size = Marshal.SizeOf<double>();
            Buffer.BlockCopy(C, row * cols * size, result, 0, cols * size);
            return result;
        }

        private string MD5Hash(string text)
        {
            var strBuilder = new StringBuilder();

            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            for (int i = 0; i < result.Length; i++)
                strBuilder.Append(result[i].ToString("X2"));

            return strBuilder.ToString();
        }
    }
}
