using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.Common.Proxy
{
    public abstract class RoutingSlipDefaultExecuteActivityResponseProxy<TRequest, TResponse, TFaultResponse> :
        IConsumer<RoutingSlipActivityCompleted>,
        IConsumer<RoutingSlipActivityFaulted>
        where TRequest : class
        where TResponse : class
        where TFaultResponse : class
    {
        public abstract string ActivityName { get; }
        public async Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if(context.Message.ActivityName!= ActivityName)
            {
                return;
            }
            var request = context.Message.GetVariable<TRequest>("Request");
            var requestId = context.Message.GetVariable<Guid>("RequestId");

            Uri responseAddress = null;
            if (context.Message.Variables.ContainsKey("ResponseAddress"))
                responseAddress = context.Message.GetVariable<Uri>("ResponseAddress");

            if (responseAddress == null)
                throw new ArgumentException($"The response address could not be found for the faulted routing slip: {context.Message.TrackingNumber}");

            var endpoint = await context.GetResponseEndpoint<TResponse>(responseAddress, requestId).ConfigureAwait(false);

            var response = await CreateResponseMessage(context, request);

            await endpoint.Send(response).ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
        {
            if (context.Message.ActivityName != ActivityName)
            {
                return;
            }
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

            var response = await CreateFaultedResponseMessage(context, request, requestId);

            await endpoint.Send(response).ConfigureAwait(false);
        }
        protected abstract Task<TResponse> CreateResponseMessage(ConsumeContext<RoutingSlipActivityCompleted> context, TRequest request);

        protected abstract Task<TFaultResponse> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipActivityFaulted> context, TRequest request, Guid requestId);

    }
}
