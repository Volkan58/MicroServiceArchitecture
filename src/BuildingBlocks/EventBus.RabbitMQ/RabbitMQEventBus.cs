using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private IConnection? _connection;
        private IModel? _channel;
        private readonly Dictionary<string, Type> _eventTypes;
        private readonly Dictionary<string, Type> _handlerTypes;

        public RabbitMQEventBus(
       ILogger<RabbitMQEventBus> logger,
       IServiceProvider serviceProvider,
       string hostName,
       string exchangeName,
       string queueName,
       string userName = "guest",
       string password = "guest")
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _exchangeName = exchangeName;
            _queueName = queueName;
            _eventTypes = new Dictionary<string, Type>();
            _handlerTypes = new Dictionary<string, Type>();

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

            _logger.LogInformation("RabbitMQ Event Bus initialized successfully");
        }
        public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
        {
            if (_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not initialized");
                throw new InvalidOperationException("RabbitMQ channel is not initialized");
            }

            var eventName = @event.GetType().Name;
            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: eventName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published event {EventName} with ID {EventId}", eventName, @event.Id);

            await Task.CompletedTask;
        }
        public void Subscribe<T, TH>()
       where T : IntegrationEvent
       where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.ContainsKey(eventName))
            {
                _eventTypes.Add(eventName, typeof(T));
                _handlerTypes.Add(eventName, handlerType);

                if (_channel != null)
                {
                    _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: eventName);
                    _logger.LogInformation("Subscribed to event {EventName} with handler {HandlerName}",
                        eventName, handlerType.Name);
                }
            }

            StartBasicConsume();
        }

        private void StartBasicConsume()
        {
            if (_channel == null)
            {
                _logger.LogError("Cannot start consuming: RabbitMQ channel is not initialized");
                return;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    await ProcessEvent(eventName, message);
                    _channel?.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {EventName}", eventName);
                    _channel?.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_eventTypes.ContainsKey(eventName))
            {
                using var scope = _serviceProvider.CreateScope();

                var eventType = _eventTypes[eventName];
                var handlerType = _handlerTypes[eventName];

                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                var handler = scope.ServiceProvider.GetService(handlerType);

                if (handler != null && integrationEvent != null)
                {
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    var method = concreteType.GetMethod("HandleAsync");

                    if (method != null)
                    {
                        await (Task)method.Invoke(handler, new[] { integrationEvent })!;
                        _logger.LogInformation("Successfully processed event {EventName}", eventName);
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for event {EventName}", eventName);
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ Event Bus disposed");
        }
    }
}
