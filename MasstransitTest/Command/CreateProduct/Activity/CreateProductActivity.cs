using MassTransit.Courier;
using MasstransitTest.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.CreateProduct.Activity
{
    public class CreateProductActivity : IExecuteActivity<CreateProductModel>
    {
        private readonly ILogger<CreateProductActivity> logger;
        public CreateProductActivity(ILogger<CreateProductActivity> logger)
        {
            this.logger = logger;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<CreateProductModel> context)
        {
            logger.LogInformation("创建产品成功");
            await Task.Delay(100);
            throw new CommonActivityExecuteFaildException("当日订单已达到上限");
            return context.CompletedWithVariables(new CreateProductResult { ProductId = "110", Message = "创建产品成功" });
        }
    }
    public class CreateProductModel
    {
        public string ProductId { get; set; }
        public int Price { get; set; }
    }
    public class CreateProductResult
    {
        public string ProductId { get; set; }
        public string Message { get; set; }
    }
}
