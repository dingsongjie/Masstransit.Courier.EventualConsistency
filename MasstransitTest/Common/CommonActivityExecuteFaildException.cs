using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common
{
    public class CommonActivityExecuteFaildException : Exception
    {
        public CommonActivityExecuteFaildException(string message) : base(message)
        {

        }
    }
}
