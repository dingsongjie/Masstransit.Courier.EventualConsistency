using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency
{
    public class CommonCommandResponse
    {
        /// <summary>
        /// 1 成功返回 2 错误并返回错误信息 3 系统出错
        /// </summary>
        public int Status { get; set; }
        public string Message { get; set; }
    }
    public class CommonCommandResponse<TResult>: CommonCommandResponse
    {
        public TResult Result { get; set; }
    }
}
