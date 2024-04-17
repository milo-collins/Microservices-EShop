using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Order;
using EShop.Infrastructure.Query.Product;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.Infrastructure.EventBus
{
    public static class Extension
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMq = new RabbitMqOption();
            // Binds to rabbit mq option class object
            configuration.GetSection("rabbitmq").Bind(rabbitMq);

            // establish connection with RabbitMQ...
            services.AddMassTransit(x => {
                // Returns Bus Control obj using RabbitMQ
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg => 
                {
                    // Connection details of rabbitMQ
                    cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg => {
                        hostcfg.Username(rabbitMq.Username);
                        hostcfg.Password(rabbitMq.Password);
                    });
                    cfg.ConfigureEndpoints(provider);
                }));
                x.AddRequestClient<GetProductById>();
                x.AddRequestClient<LoginUser>();
                x.AddRequestClient<Order.Order>();
            });

            return services;


        }
    }
}
