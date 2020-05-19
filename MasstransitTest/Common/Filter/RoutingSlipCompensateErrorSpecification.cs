using GreenPipes;
using MassTransit.Courier;
using MassTransit.Pipeline.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Common
{
    public class RoutingSlipCompensateErrorSpecification<TActivity, TLog> : IPipeSpecification<CompensateActivityContext<TActivity, TLog>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        public void Apply(IPipeBuilder<CompensateActivityContext<TActivity, TLog>> builder)
        {
            builder.AddFilter(new ActivityCompensateErrorTransportFilter<TActivity, TLog>());
        }

        public IEnumerable<ValidationResult> Validate()
        {
           yield return this.Success("success");
        }
    }
}
