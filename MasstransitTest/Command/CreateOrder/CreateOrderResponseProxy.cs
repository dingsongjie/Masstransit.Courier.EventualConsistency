using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MasstransitTest.Common;
using MasstransitTest.Common.Proxy;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Proxy
{
    public class CreateOrderResponseProxy :
            RoutingSlipDefaultResponseProxy<CreateOrderCommand, CommonCommandResponse<CreateOrderResult>, CommonCommandResponse<CreateOrderResult>>
    {

        protected override Task<CommonCommandResponse<CreateOrderResult>> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, CreateOrderCommand request)
        {

            return Task.FromResult(new CommonCommandResponse<CreateOrderResult>
            {
                Status = 1,
                Result = new CreateOrderResult
                {
                    Message = context.Message.Variables.TryGetAndReturn(nameof(CreateOrderResult.Message))?.ToString(),
                    OrderId = context.Message.Variables.TryGetAndReturn(nameof(CreateOrderResult.OrderId))?.ToString(),
                }
            });
        }
        protected override Task<CommonCommandResponse<CreateOrderResult>> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, CreateOrderCommand request, Guid requestId)
        {
            var commonActivityExecuteFaildException = context.Message.ActivityExceptions.FirstOrDefault(m => m.ExceptionInfo.ExceptionType == typeof(CommonActivityExecuteFaildException).FullName);
            if (commonActivityExecuteFaildException != null)
            {
                return Task.FromResult(new CommonCommandResponse<CreateOrderResult>
                {
                    Status = 2,
                    Message = commonActivityExecuteFaildException.ExceptionInfo.Message
                });
            }
            // system error  log here
            return Task.FromResult(new CommonCommandResponse<CreateOrderResult>
            {
                Status = 3,
                Message = "System error"
            });
        }

        protected override Task<CommonCommandResponse<CreateOrderResult>> CreateCompensationFaultedResponseMessage(ConsumeContext<RoutingSlipCompensationFailed> context, CreateOrderCommand request, Guid requestId)
        {
            var exception = context.Message.ExceptionInfo;
            // lg here context.Message.ExceptionInfo
            return Task.FromResult(new CommonCommandResponse<CreateOrderResult>
            {
                Status = 3,
                Message = "System error"
            });           
        }
    }
}
