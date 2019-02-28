using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //private readonly long[,] A;//private readonly long[,] B;
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
            log.Info($"LoadData starting.");
            var rowsA = new List<double[]>();
            var rowsB = new List<double[]>();

            for (int i = 0; i < size; i++)
            {
                rowsA.Add(cService.GetRowData("A", i));
                rowsB.Add(cService.GetRowData("B", i));
            }

            A = DenseMatrix.OfRowArrays(rowsA);
            B = DenseMatrix.OfRowArrays(rowsB);

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
            log.Info($"Validating hash.");
            var strf = Formated(C);
            var hs = MD5Hash(strf);
            var resp = cService.Validate(hs);
            log.Info($"Validation response: {nameof(resp.Success)} : {resp.Success}");
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

        //MD5 hash of values. Joined by colulmn then by row into a single string without separators and then hashed.
        private string Formated(double[,] A)
        {
            var retval = string.Empty;
            var size = A.GetLength(0);

            foreach (var r in A)
                retval += r.ToString();

            //for (int i = 0; i < size; i++)
            //    for (int j = 0; j < size; j++)
            //        retval += A[j, i].ToString();

            return retval;
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
