using GreenPipes;
using MassTransit;
using MassTransit.Context;
using MassTransit.Courier;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common.Filter
{
    public class ActivityExecuteErrorTransportFilter<TActivity, TArgument> : IFilter<ExecuteActivityContext<TActivity, TArgument>>
        where TActivity : class, IExecuteActivity<TArgument>
        where TArgument : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("moveFault");
        }

        public async Task Send(ExecuteActivityContext<TActivity, TArgument> context, IPipe<ExecuteActivityContext<TActivity, TArgument>> next)
        {
            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!context.TryGetPayload(out IErrorTransport transport))
                    throw new TransportException(context.ReceiveContext.InputAddress, $"The {nameof(IErrorTransport)} was not available on the {nameof(ReceiveContext)}.");
                var exceptionReceiveContext = new RescueExceptionReceiveContext(context.ReceiveContext, ex);
                await transport.Send(exceptionReceiveContext);
            }
        }
    }
}
