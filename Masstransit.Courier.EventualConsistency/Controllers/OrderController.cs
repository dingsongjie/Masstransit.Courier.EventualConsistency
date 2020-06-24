using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Masstransit.Courier.EventualConsistency.CreateProduct.Activity;
using Masstransit.Courier.EventualConsistency.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Masstransit.Courier.EventualConsistency.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IBus bus;
        IRequestClient<CreateOrderCommand> createOrderClient;
        IRequestClient<CreateProductCommand> createProductClient;

        public OrderController(ILogger<OrderController> logger, IBus bus, IRequestClient<CreateOrderCommand> createOrderClient, IRequestClient<CreateProductCommand> createProductClient)
        {
            _logger = logger;
            this.bus = bus;
            this.createProductClient = createProductClient;
            this.createOrderClient = createOrderClient;
        }
        [HttpGet("CreateOrder")]
        public async Task<CommonCommandResponse<CreateOrderResult>> CreateOrder()
        {

            var result = await createOrderClient.GetResponse<CommonCommandResponse<CreateOrderResult>>(new Masstransit.Courier.EventualConsistency.CreateOrderCommand()
            {
                ProductId = "11",
                Price = 100,
                CustomerId = "22"
            });

            return result.Message;
        }
        [HttpGet("CreateProduct")]
        public async Task<CommonCommandResponse<CreateProductResult>> CreateProduct()
        {

            var result = await createProductClient.GetResponse<CommonCommandResponse<CreateProductResult>>(new CreateProductCommand()
            {
                ProductId = "11",
                Price = 100,
            });

            return result.Message;
        }
    }

}
