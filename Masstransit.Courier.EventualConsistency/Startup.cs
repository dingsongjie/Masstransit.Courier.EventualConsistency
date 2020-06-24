using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Pipeline.Filters;
using MassTransit.RabbitMqTransport;
using Masstransit.Courier.EventualConsistency.Common;
using Masstransit.Courier.EventualConsistency.Common.Filter;
using Masstransit.Courier.EventualConsistency.CreateProduct;
using Masstransit.Courier.EventualConsistency.CreateProduct.Activity;
using Masstransit.Courier.EventualConsistency.Dto;
using Masstransit.Courier.EventualConsistency.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Masstransit.Courier.EventualConsistency
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();

            services.AddMassTransit(x =>
            {
                x.AddActivities(Assembly.GetExecutingAssembly());

                x.AddRequestClient<CreateOrderCommand>();
                x.AddRequestClient<CreateProductCommand>();
                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(Configuration["RabbitmqConfig:HostUri"]), host =>
                    {
                        host.Username(Configuration["RabbitmqConfig:Username"]);
                        host.Password(Configuration["RabbitmqConfig:Password"]);
                    });
                    cfg.UseInMemoryScheduler();

                    cfg.UseHealthCheck(context);

                    AddActivity(cfg, context);
                }));

            });
            services.AddMassTransitHostedService();
        }

        private void AddActivity(IRabbitMqBusFactoryConfigurator cfg, IRegistrationContext<IServiceProvider> context)
        {
            #region CreateOrderRequest

            #region Command

            cfg.ReceiveEndpoint("CreateOrderCommand", ep =>
            {
                ep.PrefetchCount = 100;
                var requestProxy = new CreateOrderRequestProxy(Configuration);
                var responseProxy = new CreateOrderResponseProxy();
                ep.Instance(requestProxy);
                ep.Instance(responseProxy);
            });
            #endregion

            #region DeductStock

            cfg.ReceiveEndpoint("DeductStock_execute", ep =>
            {

                ep.PrefetchCount = 100;
                ep.ExecuteActivityHost<DeductStockActivity, DeductStockModel>(new Uri($"{Configuration["RabbitmqConfig:HostUri"]}/DeductStock_compensate"), context.Container);
            });

            cfg.ReceiveEndpoint("DeductStock_compensate", ep =>
            {
                ep.PrefetchCount = 100;
                ep.CompensateActivityHost<DeductStockActivity, DeductStockLog>(context.Container, conf =>
                 {
                     conf.AddPipeSpecification(new RoutingSlipCompensateErrorSpecification<DeductStockActivity, DeductStockLog>());
                 });
                ep.UseMessageRetry(policy =>
                {
                    policy.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
                });
                ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
            });
            #endregion

            #region DeductBalance
            cfg.ReceiveEndpoint("DeductBalance_execute", ep =>
                {
                    ep.PrefetchCount = 100;
                    ep.ExecuteActivityHost<DeductBalanceActivity, DeductBalanceModel>(new Uri($"{Configuration["RabbitmqConfig:HostUri"]}/DeductBalance_compensate"), context.Container);
                });

            cfg.ReceiveEndpoint("DeductBalance_compensate", ep =>
            {
                ep.PrefetchCount = 100;
                ep.CompensateActivityHost<DeductBalanceActivity, DeductBalanceLog>(context.Container, conf =>
                 {
                     conf.AddPipeSpecification(new RoutingSlipCompensateErrorSpecification<DeductBalanceActivity, DeductBalanceLog>());
                 });
                ep.UseMessageRetry(policy =>
                {
                    policy.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
                });
                ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
            });
            #endregion

            #region CreateOrder
            cfg.ReceiveEndpoint("CreateOrder_execute", ep =>
                {
                    ep.PrefetchCount = 100;
                    ep.ExecuteActivityHost<CreateOrderActivity, CreateOrderModel>(context.Container);
                });
            #endregion
            #endregion

            #region CreateProductRequest

            #region Command

            cfg.ReceiveEndpoint("CreateProductCommand", ep =>
            {
                ep.PrefetchCount = 100;
                var requestProxy = new CreateProductRequestProxy(Configuration);
                var responseProxy = new CreateProductResponseProxy();
                ep.Instance(requestProxy);
                ep.Instance(responseProxy, config =>
                {

                });
            });
            #endregion

            #region CreateProduct

            cfg.ReceiveEndpoint("CreateProduct_execute", ep =>
            {
                ep.PrefetchCount = 100;
                ep.ExecuteActivityHost<CreateProductActivity, CreateProductModel>(context.Container);
            });

            #endregion

            #region CreateStock
            cfg.ReceiveEndpoint("CreateStock_execute", ep =>
            {
                ep.PrefetchCount = 100;
                ep.ExecuteActivityHost<CreateStockActivity, CreateStockModel>(context.Container, conf =>
                {
                    conf.AddPipeSpecification(new RoutingSlipExecuteErrorSpecification<CreateStockActivity, CreateStockModel>());
                });

                //ep.UseMessageRetry(policy =>
                //{
                //    policy.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
                //});
                //ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
            });


            #endregion


            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
