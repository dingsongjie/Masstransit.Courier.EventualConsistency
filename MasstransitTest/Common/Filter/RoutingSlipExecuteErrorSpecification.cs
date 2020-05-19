using GreenPipes;
using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common.Filter
{
    public class RoutingSlipExecuteErrorSpecification<TActivity, TArgument> : IPipeSpecification<ExecuteActivityContext<TActivity, TArgument>>
        where TActivity : class, IExecuteActivity<TArgument>
        where TArgument : class
    {
        public void Apply(IPipeBuilder<ExecuteActivityContext<TActivity, TArgument>> builder)
        {
            builder.AddFilter(new ActivityExecuteErrorTransportFilter<TActivity, TArgument>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield return this.Success("success");
        }
    }
}
