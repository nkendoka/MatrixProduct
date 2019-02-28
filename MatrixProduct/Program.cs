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
            if (args.Any())
            {
                var matrixOp = new MxOperation(int.Parse(args[0]));
            }else

            while (true)
            {
                    Console.Write(">>");
                    var rd = Console.ReadLine();
                    if ("QEX".Contains(rd.ToUpper().First())) break;
                    if (!int.TryParse(rd, out int size)) continue;

                    var matrixOp = new MxOperation(size);
                    matrixOp.LoadData();
                    matrixOp.Calculate();
                    matrixOp.Validate();
            }


            Console.ReadLine();
        }
    }
}
