using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Dto
{
    /// <summary>
    /// 短流程 分布式事务
    /// </summary>
    public class CreateProductCommand
    {
        public string ProductId { get; set; }
        public int Price { get; set; }
    }
}
