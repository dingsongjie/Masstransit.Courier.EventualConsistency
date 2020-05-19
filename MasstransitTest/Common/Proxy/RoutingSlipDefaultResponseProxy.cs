using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common.Proxy
{
    public abstract class RoutingSlipDefaultResponseProxy<TRequest, TResponse, TFaultResponse> : RoutingSlipResponseProxy<TRequest, TResponse, TFaultResponse>, IConsumer<RoutingSlipCompensationFailed>
        where TRequest : class
        where TResponse : class
        where TFaultResponse : class
    {
        public async Task Consume(ConsumeContext<RoutingSlipCompensationFailed> context)
        {
            var request = context.Message.GetVariable<TRequest>("Request");
            var requestId = context.Message.GetVariable<Guid>("RequestId");

            Uri faultAddress = null;
            if (context.Message.Variables.ContainsKey("FaultAddress"))
                faultAddress = context.Message.GetVariable<Uri>("FaultAddress");
            if (faultAddress == null && context.Message.Variables.ContainsKey("ResponseAddress"))
                faultAddress = context.Message.GetVariable<Uri>("ResponseAddress");

            if (faultAddress == null)
                throw new ArgumentException($"The fault/response address could not be found for the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetFaultEndpoint<TResponse>(faultAddress, requestId).ConfigureAwait(false);

            var response = await CreateCompensationFaultedResponseMessage(context, request, requestId);

            await endpoint.Send(response).ConfigureAwait(false);
        }
        protected abstract Task<TFaultResponse> CreateCompensationFaultedResponseMessage(ConsumeContext<RoutingSlipCompensationFailed> context, TRequest request, Guid requestId);
    }
}
