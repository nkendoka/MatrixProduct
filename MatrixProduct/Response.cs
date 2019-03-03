using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProduct
{
    public class BaseResponse
    {
        public string Cause { get; set; }
        public bool Success { get; set; }
    }

    public class InitResponse: BaseResponse
    {
        public int Value { get; set; }
    }
    public class DataSetResponse : BaseResponse
    {
        public int[] Value { get; set; }
    }
    public class ValidateResponse : BaseResponse
    {
        public string Value { get; set; }
    }
}
