using GreenPipes;
using MassTransit;
using MassTransit.Context;
using MassTransit.Courier;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common
{
    public class ActivityCompensateErrorTransportFilter<TActivity, TLog> : IFilter<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("moveFault");
        }

        public async Task Send(CompensateActivityContext<TActivity, TLog> context, IPipe<CompensateActivityContext<TActivity, TLog>> next)
        {
            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                if (!context.TryGetPayload(out IErrorTransport transport))
                    throw new TransportException(context.ReceiveContext.InputAddress, $"The {nameof(IErrorTransport)} was not available on the {nameof(ReceiveContext)}.");
                var exceptionReceiveContext = new RescueExceptionReceiveContext(context.ReceiveContext, ex);
                await transport.Send(exceptionReceiveContext);
            }
        }
    }
}
