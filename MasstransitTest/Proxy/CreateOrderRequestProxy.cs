using MassTransit;
using MassTransit.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasstransitTest.Proxy
{
    public class CreateOrderRequestProxy :
           RoutingSlipRequestProxy<CreateOrderCommand>
    {
        protected override Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<CreateOrderCommand> request)
        {
            builder.AddActivity("DeductStock", new Uri("rabbitmq://192.168.124.63/dev/DeductStock_execute"), new DeductStockModel { ProductId = request.Message.ProductId });

            builder.AddActivity("DeductBalance", new Uri("rabbitmq://192.168.124.63/dev/DeductBalance_execute"), new DeductBalanceModel { CustomerId = request.Message.CustomerId, Price = request.Message.Price });

            builder.AddActivity("CreateOrder", new Uri("rabbitmq://192.168.124.63/dev/CreateOrder_execute"), new CreateOrderModel { Price = request.Message.Price, CustomerId = request.Message.CustomerId, ProductId = request.Message.ProductId });
            return Task.CompletedTask;
        }
    }
}
