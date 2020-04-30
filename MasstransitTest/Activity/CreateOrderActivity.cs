using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest
{
    public class CreateOrderActivity : IExecuteActivity<CreateOrderModel>
    {
        private readonly ILogger<CreateOrderActivity> logger;
        public CreateOrderActivity(ILogger<CreateOrderActivity> logger)
        {
            this.logger = logger;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<CreateOrderModel> context)
        {
            logger.LogInformation("创建订单");
            await Task.Delay(100);
            //throw new CommonActivityExecuteFaildException("当日订单已达到上限");
            return context.CompletedWithVariables(new CreateOrderResult { OrderId="111122",Message="创建订单成功" });
        }
    }
    public class CreateOrderModel
    {
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public int Price { get; set; }
    }
    public class CreateOrderResult
    {
        public string OrderId { get; set; }
        public string Message { get; set; }
    }
    public class CommonActivityExecuteFaildException: Exception
    {
        public CommonActivityExecuteFaildException(string message):base(message)
        {

        }
    }
}
