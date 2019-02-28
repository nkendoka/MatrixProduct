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
    class Program
    {
        static void Main(string[] args)
        {
            var prd = mProduct(Tests.A2, Tests.B2);
            var prd2 = mProduct(Tests.Ad2, Tests.Bd2);

            var strf = Formated(prd);
            var hs2 = Gmd5H(strf);
            var hs3 = MD5Hash(strf);

            var cService = new InvCloudService();

            cService.Init(2);
            cService.GetRowData("A", 1);
            cService.GetRowData("B", 0);

            cService.Validate(hs2);
        }

        static int[,] mProduct(int[,] A, int[,] B)
        {
            var size = A.GetLength(0);
            var C = new int[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 0; k < size; k++)
                        C[i, j] += A[i, k] * B[k, j];

            return C;
        }
        static double[,] mProduct(double[,] A, double[,] B)
        {
            Matrix<double> Aa = DenseMatrix.OfArray(A);
            Matrix<double> Bb = DenseMatrix.OfArray(B);

            var Cc = Aa.Multiply(Bb);
            return Cc.ToArray();

        }

        //MD5 hash of values. Joined by colulmn then by row into a single string without separators and then hashed.
        static string Formated(int[,] A)
        {
            var retval = string.Empty;
            var size = A.GetLength(0);

            //foreach (var r in A)
            //    retval += r.ToString();

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    retval += A[j, i].ToString();

            return retval;
        }

        static string Gmd5H(string input)
        {
            var sb = new StringBuilder();

            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);


            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

        public static string MD5Hash(string text)
        {
            var strBuilder = new StringBuilder();

            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            for (int i = 0; i < result.Length; i++)
                strBuilder.Append(result[i].ToString("x2"));

            return strBuilder.ToString();
        }
    }
}
