using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using MassTransit.RabbitMqTransport;
using MasstransitTest.Proxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MasstransitTest
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
            services.AddTransient<DeductStockActivity>();
            services.AddTransient<DeductBalanceActivity>();
            services.AddTransient<CreateOrderActivity>();
            services.AddMassTransit(x =>
            {
                x.AddRequestClient<CreateOrderCommand>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri("rabbitmq://192.168.124.63/dev"), host =>
                    {
                        host.Username("mqadmin");
                        host.Password("mq62TEST");

                    });
                    cfg.UseInMemoryScheduler();

                    cfg.UseHealthCheck(provider);

                    AddActivity(cfg, provider);
                    //cfg.ConfigureEndpoints(provider);

                }));
            });
            services.AddMassTransitHostedService();
        }

        private static void AddActivity(IRabbitMqBusFactoryConfigurator cfg, IServiceProvider serviceProvider)
        {
            #region CreateOrderRequest
            #region DeductStock
            
            cfg.ReceiveEndpoint("DeductStock_execute", ep =>
            {
                
                ep.PrefetchCount = 100;
                ep.ExecuteActivityHost<DeductStockActivity, DeductStockModel>(new Uri("rabbitmq://192.168.124.63/dev/DeductStock_compensate"), serviceProvider);
            });

            cfg.ReceiveEndpoint("DeductStock_compensate", ep =>
            {
                
                ep.PrefetchCount = 100;
                
                ep.CompensateActivityHost<DeductStockActivity, DeductStockLog>(serviceProvider, conf =>
                 {
                     conf.UseRetry(policy =>
                     {
                         policy.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
                     });
                 });
                ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
            });
            #endregion

            #region DeductBalance
            cfg.ConfigureEndpoints(serviceProvider);
            cfg.ReceiveEndpoint("DeductBalance_execute", ep =>
                {
                    ep.PrefetchCount = 100;
                    ep.ExecuteActivityHost<DeductBalanceActivity, DeductBalanceModel>(new Uri("rabbitmq://192.168.124.63/dev/DeductBalance_compensate"), serviceProvider);
                });

            cfg.ReceiveEndpoint("DeductBalance_compensate", ep =>
            {
                ep.PrefetchCount = 100;
                ep.CompensateActivityHost<DeductBalanceActivity, DeductBalanceLog>(serviceProvider, conf =>
                 {
                     conf.UseRetry(policy =>
                     {
                         policy.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2));
                     });
                 });
                ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10)));
            });
            #endregion

            #region CreateOrder
            cfg.ReceiveEndpoint("CreateOrder_execute", ep =>
                {
                    ep.PrefetchCount = 100;
                    ep.ExecuteActivityHost<CreateOrderActivity, CreateOrderModel>(serviceProvider);
                });

            cfg.ReceiveEndpoint("CreateOrderCommand", ep =>
            {
                ep.PrefetchCount = 100;
                var requestProxy = new CreateOrderRequestProxy();
                var responseProxy = new CreateOrderResponseProxy();
                ep.Instance(requestProxy);
                ep.Instance(responseProxy);
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
