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
            builder.AddActivity("DeductStock", new Uri("loopback://localhost/DeductStock"), new DeductStockModel { ProductId = request.Message.ProductId });

            builder.AddActivity("DeductBalance", new Uri("loopback://localhost/DeductBalance"), new DeductBalanceModel { CustomerId = request.Message.CustomerId, Price = request.Message.Price });

            builder.AddActivity("CreateOrder", new Uri("loopback://localhost/CreateOrder"), new CreateOrderModel { Price = request.Message.Price, CustomerId = request.Message.CustomerId, ProductId = request.Message.ProductId });
            return Task.CompletedTask;
        }
    }
}
