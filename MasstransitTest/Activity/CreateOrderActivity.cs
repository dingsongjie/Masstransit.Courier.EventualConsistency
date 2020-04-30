using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest
{
    public class CreateOrderActivity : IExecuteActivity<CreateOrderModel>
    {

        public async Task<ExecutionResult> Execute(ExecuteContext<CreateOrderModel> context)
        {
            Console.WriteLine("创建订单");
            
            throw new CommonActivityExecuteFaildException("当日订单已达到上限");
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
