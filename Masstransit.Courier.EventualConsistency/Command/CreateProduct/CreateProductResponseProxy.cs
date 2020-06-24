using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Masstransit.Courier.EventualConsistency.Common;
using Masstransit.Courier.EventualConsistency.Common.Proxy;
using Masstransit.Courier.EventualConsistency.CreateProduct.Activity;
using Masstransit.Courier.EventualConsistency.Dto;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.CreateProduct
{
    public class CreateProductResponseProxy :
            RoutingSlipDefaultExecuteActivityResponseProxy<CreateProductCommand, CommonCommandResponse<CreateProductResult>, CommonCommandResponse<CreateProductResult>>
    {
        public override string ActivityName => "CreateProduct";

        protected override Task<CommonCommandResponse<CreateProductResult>> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipActivityFaulted> context, CreateProductCommand request, Guid requestId)
        {
            var hasCommonFaildException = context.Message.ExceptionInfo.ExceptionType == typeof(CommonActivityExecuteFaildException).FullName;
            if (hasCommonFaildException )
            {
                return Task.FromResult(new CommonCommandResponse<CreateProductResult>
                {
                    Status = 2,
                    Message = context.Message.ExceptionInfo.Message
                });
            }
            // system error  log here
            return Task.FromResult(new CommonCommandResponse<CreateProductResult>
            {
                Status = 3,
                Message = "System error"
            });
        }

        protected override Task<CommonCommandResponse<CreateProductResult>> CreateResponseMessage(ConsumeContext<RoutingSlipActivityCompleted> context, CreateProductCommand request)
        {
            return Task.FromResult(new CommonCommandResponse<CreateProductResult>
            {
                Status = 1,
                Result = new CreateProductResult
                {
                    Message = context.Message.Variables.TryGetAndReturn(nameof(CreateProductResult.Message))?.ToString(),
                    ProductId = context.Message.Variables.TryGetAndReturn(nameof(CreateProductResult.ProductId))?.ToString(),
                }
            });
        }
    }
}
