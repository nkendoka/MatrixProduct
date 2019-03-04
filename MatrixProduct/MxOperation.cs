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
        private readonly int[] C;
        private readonly ConcurrentDictionary<int, int> rC;
        private HMatrix nw, ne, sw, se;

        public MxOperation(int mSize)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info($"Request to Initialize DataSet: {mSize}.");
            cService = new InvCloudService();
            size = mSize;
            //var resp = cService.Init(size);

            C = Enumerable.Range(0, size * size).Select(x => 1).ToArray();
            rC = new ConcurrentDictionary<int, int>(Enumerable.Range(0, size * size).ToDictionary(x => x, x => 1));

            var hmSizeLT = size > 10 ? (size / 2) : size;
            var hmSizeRB = size - hmSizeLT;
            nw = new HMatrix(0, 0, hmSizeLT, hmSizeLT, size);
            se = new HMatrix(hmSizeLT + 1, hmSizeLT + 1, hmSizeRB, hmSizeRB, size);

            var hmSizeRTx = size - hmSizeLT;
            var hmSizeRTy = hmSizeLT;

            var hmSizeLBx = hmSizeLT;
            var hmSizeLBy = size - hmSizeLT;

            ne = new HMatrix(hmSizeLT + 1, 0, hmSizeRTx, hmSizeRTy, size);
            sw = new HMatrix(0, hmSizeLT + 1, hmSizeLBx, hmSizeLBy, size);

            //log.Info(resp.Success ? $"Initialized Successfully: {resp.Value}." : "Error Initialization.");
        }

        public void LoadData2()
        {
            log.Info("LoadData started.");

            for (int i = 0; i < size / 2; i++)
            {
                var t1 = HalfMatrix(true, i);
                var t2 = HalfMatrix(true, size / 2 + i);
                var result = Task.WhenAll(t1, t2).Result;
            }

            for (int i = 0; i < size / 2; i++)
            {
                var t1 = HalfMatrix(false, i);
                var t2 = HalfMatrix(false, size / 2 + (i));
                var result = Task.WhenAll(t1, t2).Result;
            }

            log.Info("LoadData complete.");
        }
        private async Task<int> HalfMatrix(bool rcol, int index)
        {
            var rowA = cService.GetRowData(index);
            var colB = cService.GetColumnData(index);

            //var t1 = Compute(true, i);
            //var t2 = Compute(true, size / 2 + (i));
            //var result = Task.WhenAll(t1, t2).Result;

            return 1;
        }

        private static async Task<int> Compute(bool rcol, int index)
        {
            //var t1 = HalfMatrix(true, i);
            //var t2 = HalfMatrix(true, size / 2 + (i));
            //var result = Task.WhenAll(t1, t2).Result;

            return 1;
        }

        public void LoadData()
        {
            var batchSize = 5;
            log.Info("LoadData started.");
            var items = new List<int>(Enumerable.Range(0, size));
            var A = new int[size, size];
            var B = new int[size, size];
            while (items.Any())
            {
                var range = items.Take(batchSize).ToList();

                var result = Parallel.For(range.First(), range.Last() + 1, (i, state) =>
                {
                    var rowA = cService.GetRowData(i);
                    var colB = cService.GetColumnData(i);

                    for (int j = 0; j < rowA.Length; j++)
                    {
                        //var cIndx = i * size + j;
                        //C.TryGetValue(cIndx, out int cVal);

                        //bufferMatrixC.TryGetValue(cIndx, out ConcurrentDictionary<int, int> bufferRow);
                        //bufferRow.TryGetValue()

                        A[i, j] = rowA[j];
                        B[i, j] = colB[j];
                    }
                });

                items = items.Skip(batchSize).ToList();
            }
            log.Info("LoadData complete.");
            mProduct(A, B);
        }

        public void Validate()
        {
            //mProduct();
            var hs = MD5Hash(C);
            log.Info($"Validating hash. {hs}");
            var resp = cService.Validate(hs);
            log.Info($"Validation response: {resp.Value}.");
        }

        //Joined by colulmn then by row into a single string without separators and then hashed.
        public string MD5Hash(int[] source)
        {
            using (var md5 = MD5.Create())
            {
                var input = source.Aggregate(new StringBuilder(), (s, i) => s.Append(i)).ToString();
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return data.Aggregate(new StringBuilder(), (s, i) => s.Append(i.ToString())).ToString();
            }
        }

        private void mProduct(int[,] A, int[,] B)
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 0; k < size; k++)
                        C[i * size + j] += A[i, k] * B[k, j];
        }
    }
}