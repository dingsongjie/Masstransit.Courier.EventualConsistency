using MassTransit.Courier;
using Masstransit.Courier.EventualConsistency.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.CreateProduct.Activity
{
    public class CreateStockActivity : IExecuteActivity<CreateStockModel>
    {
        private readonly ILogger<CreateStockActivity> logger;
        public CreateStockActivity(ILogger<CreateStockActivity> logger)
        {
            this.logger = logger;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<CreateStockModel> context)
        {
            logger.LogInformation($"产品:{context.Arguments.ProductId} 新建库存成功");
            await Task.Delay(100);
            //throw new CommonActivityExecuteFaildException("库存出错");
            return context.Completed();
        }
    }
    public class CreateStockModel
    {
        public string ProductId { get; set; }
    }
}
