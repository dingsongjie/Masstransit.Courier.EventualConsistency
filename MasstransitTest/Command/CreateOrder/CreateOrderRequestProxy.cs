using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Proxy
{
    public class CreateOrderRequestProxy : RoutingSlipRequestProxy<CreateOrderCommand>

    {
        private readonly IConfiguration configuration;
        public CreateOrderRequestProxy(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected override Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<CreateOrderCommand> request)
        {
            builder.AddActivity("DeductStock", new Uri($"{configuration["RabbitmqConfig:HostUri"]}/DeductStock_execute"), new DeductStockModel { ProductId = request.Message.ProductId });

            builder.AddActivity("DeductBalance", new Uri($"{configuration["RabbitmqConfig:HostUri"]}/DeductBalance_execute"), new DeductBalanceModel { CustomerId = request.Message.CustomerId, Price = request.Message.Price });

            builder.AddActivity("CreateOrder", new Uri($"{configuration["RabbitmqConfig:HostUri"]}/CreateOrder_execute"), new CreateOrderModel { Price = request.Message.Price, CustomerId = request.Message.CustomerId, ProductId = request.Message.ProductId });

            //builder.AddSubscription(new Uri($"{configuration["RabbitmqConfig:HostUri"]}/CreateOrderCommand_events"),RoutingSlipEvents.All);
            return Task.CompletedTask;
        }
    }
}
