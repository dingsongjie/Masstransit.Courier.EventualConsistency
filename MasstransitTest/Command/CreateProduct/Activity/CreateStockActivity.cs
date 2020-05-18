using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.CreateProduct.Activity
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
            //throw new CommonActivityExecuteFaildException("当日订单已达到上限");
            return context.Completed();
        }
    }
    public class CreateStockModel
    {
        public string ProductId { get; set; }
    }
}
