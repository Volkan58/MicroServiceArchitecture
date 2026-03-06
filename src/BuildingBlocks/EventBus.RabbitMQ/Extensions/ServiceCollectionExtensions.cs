using EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.RabbitMQ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQEventBus(
        this IServiceCollection services,
        string hostName,
        string exchangeName,
        string queueName,
        string userName = "guest",
        string password = "guest")
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQEventBus>>();
                return new RabbitMQEventBus(logger, sp, hostName, exchangeName, queueName, userName, password);
            });

            return services;
        }
    }
}
