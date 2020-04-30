using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MasstransitTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IBus bus;
        IRequestClient<CreateOrderCommand> client;

        public OrderController(ILogger<OrderController> logger, IBus bus, IRequestClient<CreateOrderCommand> client)
        {
            _logger = logger;
            this.bus = bus;
            this.client = client;
        }
        [HttpGet("Get")]
        public async Task<CommonCommandResponse<CreateOrderResult>> Get()
        {
            
            var result = await client.GetResponse<CommonCommandResponse<CreateOrderResult>>(new MasstransitTest.CreateOrderCommand()
            {
                ProductId = "11",
                Price = 100,
                CustomerId = "22"
            });
            
            return result.Message;
        }
    }

}
