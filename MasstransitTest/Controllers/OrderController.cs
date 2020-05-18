using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MasstransitTest.CreateProduct.Activity;
using MasstransitTest.Dto;
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

            var result = await createOrderClient.GetResponse<CommonCommandResponse<CreateOrderResult>>(new MasstransitTest.CreateOrderCommand()
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
