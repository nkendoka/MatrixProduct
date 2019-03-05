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
        private readonly object locker = new object();
        private const int batchSize = 5;
        private readonly int size;
        private readonly int[] C;
        private readonly short[,,] bufferC;

        public MxOperation(int mSize)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info($"Request to Initialize DataSet: {mSize}.");
            cService = new InvCloudService();
            size = mSize;
            var resp = cService.Init(size);

            bufferC = new short[size, size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int m = 0; m < size; m++)
                        bufferC[i, j, m] = 1;

            C = Enumerable.Range(0, size * size).Select(x => size).ToArray();

            log.Info(resp.Success ? $"Initialized Successfully: {resp.Value}." : "Error Initialization.");
        }

        public void LoadData2()
        {
            log.Info("LoadData started.");
            var items = new List<int>(Enumerable.Range(0, size));
            while (items.Any())
            {
                var range = items.Take(batchSize).ToList();
                var result = Parallel.For(range.First(), range.Last() + 1, (i, state) =>
               // for (int i=0;i<size;i++)
                {
                    var rowA = cService.GetRowData(i);
                    var colB = cService.GetColumnData(i);

                    for (int j = 0; j < rowA.Length; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            var cIndx = i * size + k;
                            C[cIndx] -= bufferC[i, k, j];
                            bufferC[i, k, j] *= (short)rowA[j];
                            C[cIndx] += bufferC[i, k, j];

                            var cIndy = k * size + i;
                            C[cIndy] -= bufferC[k, i, j];
                            bufferC[k, i, j] *= (short)colB[j];
                            C[cIndy] += bufferC[k, i, j];
                        }
                    }
                });

                items = items.Skip(batchSize).ToList();
            }
            log.Info("LoadData complete.");
        }

        public void LoadData()
        {
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