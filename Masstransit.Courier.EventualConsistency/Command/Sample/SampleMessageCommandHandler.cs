using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.Command.Sample
{
    public class SampleMessageCommandHandler : IConsumer<SampleMessageCommand>
    {
        public async Task Consume(ConsumeContext<SampleMessageCommand> context)
        {
            await context.RespondAsync(new SampleMessageCommandResult() { Data = "Sample" });
        }
    }
}
