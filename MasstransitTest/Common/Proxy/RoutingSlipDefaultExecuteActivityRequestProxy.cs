﻿using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common.Proxy
{
    public abstract class RoutingSlipDefaultExecuteActivityRequestProxy<TRequest> :
        IConsumer<TRequest>
        where TRequest : class
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddSubscription(context.ReceiveContext.InputAddress, RoutingSlipEvents.ActivityCompleted | RoutingSlipEvents.ActivityFaulted);

            builder.AddVariable("RequestId", context.RequestId);
            builder.AddVariable("ResponseAddress", context.ResponseAddress);
            builder.AddVariable("FaultAddress", context.FaultAddress);
            builder.AddVariable("Request", context.Message);

            await BuildRoutingSlip(builder, context);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip).ConfigureAwait(false);
        }

        protected abstract Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<TRequest> request);
    }
}
