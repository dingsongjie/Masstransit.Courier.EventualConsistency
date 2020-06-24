using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Masstransit.Courier.EventualConsistency.Common.Proxy;
using Masstransit.Courier.EventualConsistency.CreateProduct.Activity;
using Masstransit.Courier.EventualConsistency.Dto;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masstransit.Courier.EventualConsistency.CreateProduct
{
    public class CreateProductRequestProxy : RoutingSlipDefaultExecuteActivityRequestProxy<CreateProductCommand>

    {
        private readonly IConfiguration configuration;
        public CreateProductRequestProxy(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected override Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<CreateProductCommand> request)
        {
            builder.AddActivity("CreateProduct", new Uri($"{configuration["RabbitmqConfig:HostUri"]}/CreateProduct_execute"), new CreateProductModel { ProductId = request.Message.ProductId, Price = request.Message.Price });

            builder.AddActivity("CreateStock", new Uri($"{configuration["RabbitmqConfig:HostUri"]}/CreateStock_execute"), new CreateStockModel { ProductId = request.Message.ProductId});

            return Task.CompletedTask;
        }
    }
}
