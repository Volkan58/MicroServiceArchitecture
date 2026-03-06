using EventBus.Abstractions;
using EventBus.Events;
using Log.Application.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Log.Application.Events
{
    public class ProductCreatedEvent : IntegrationEvent
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
    }
    public class ProductCreatedEventHandler : IIntegrationEventHandler<ProductCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductCreatedEventHandler> _logger;

        public ProductCreatedEventHandler(IMediator mediator, ILogger<ProductCreatedEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task HandleAsync(ProductCreatedEvent @event)
        {
            _logger.LogInformation("Handling ProductCreatedEvent: {ProductId}", @event.ProductId);

            var command = new CreateLogCommand(
                "ProductService",
                "Information",
                $"Product created: {@event.Name} (ID: {@event.ProductId})",
                null,
                null,
                new Dictionary<string, string>
                {
                { "ProductId", @event.ProductId.ToString() },
                { "ProductName", @event.Name },
                { "CreatedBy", @event.CreatedBy.ToString() },
                { "EventId", @event.Id.ToString() }
                }
            );

            await _mediator.Send(command);
        }

    }
}
