using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest
{
    public class DeductStockActivity : IActivity<DeductStockModel, DeductStockLog>
    {
        public async Task<CompensationResult> Compensate(CompensateContext<DeductStockLog> context)
        {
            var log = context.Log;
            Console.WriteLine("还原库存");
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<DeductStockModel> context)
        {
            var argument = context.Arguments;
            Console.WriteLine("扣减库存");
            return context.Completed(new DeductStockLog() { ProductId = argument.ProductId, Amount = 1 });
        }
    }
    public class DeductStockModel
    {
        public string ProductId { get; set; }
    }
    public class DeductStockLog
    {
        public string ProductId { get; set; }
        public int Amount { get; set; }
    }
}
