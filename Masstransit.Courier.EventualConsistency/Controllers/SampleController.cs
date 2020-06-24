using Masstransit.Courier.EventualConsistency.Command.Sample;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.Controllers
{
    [Route("[controller]")]
    public class SampleController
    {
        private IRequestClient<SampleMessageCommand> client;
        public SampleController(IRequestClient<SampleMessageCommand> client)
        {
            this.client = client;
        }
        [HttpGet("Request")]
        public async Task<string> CreateProduct()
        {
            var result = await client.GetResponse<SampleMessageCommandResult>(new SampleMessageCommand());
            return result.Message.Data;
        }
    }
}
