using GreenPipes;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest
{
    public class DeductBalanceActivity : IActivity<DeductBalanceModel, DeductBalanceLog>
    {
        private readonly ILogger<DeductBalanceActivity> logger;
        public DeductBalanceActivity(ILogger<DeductBalanceActivity> logger)
        {
            this.logger = logger;
        }
        public async Task<CompensationResult> Compensate(CompensateContext<DeductBalanceLog> context)
        {
            logger.LogInformation("还原余额");
            //throw new Exception("11");
            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<DeductBalanceModel> context)
        {
            
            logger.LogInformation("扣减余额");
            await Task.Delay(100);
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
