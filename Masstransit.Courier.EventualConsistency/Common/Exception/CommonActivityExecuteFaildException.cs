using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.Common
{
    public class CommonActivityExecuteFaildException : Exception
    {
        public CommonActivityExecuteFaildException(string message) : base(message)
        {

        }
    }
}
