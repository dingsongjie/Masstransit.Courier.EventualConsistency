using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest
{
    public class DeductBalanceActivity : IActivity<DeductBalanceModel, DeductBalanceLog>
    {
        public async Task<CompensationResult> Compensate(CompensateContext<DeductBalanceLog> context)
        {
            Console.WriteLine("还原余额");
            //throw new Exception("11");
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<DeductBalanceModel> context)
        {
            Console.WriteLine("扣减余额");
            return context.Completed(new DeductBalanceLog());
        }
    }
    public class DeductBalanceModel
    {
        public string CustomerId { get; set; }
        public int Price { get; set; }
    }
    public class DeductBalanceLog
    {

    }
}
