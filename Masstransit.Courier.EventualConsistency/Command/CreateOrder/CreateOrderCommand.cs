using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency
{
    /// <summary>
    /// 长流程 分布式事务
    /// </summary>
    public class CreateOrderCommand
    {
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public int Price { get; set; }
    }
}
