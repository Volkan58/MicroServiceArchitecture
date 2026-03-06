using EventBus.Abstractions;
using EventBus.Events;
using Log.Application.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Log.Application.Events
{
    public class ProductUpdatedEvent : IntegrationEvent
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UpdatedBy { get; set; }
    }
    public class ProductUpdatedEventHandler : IIntegrationEventHandler<ProductUpdatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductUpdatedEventHandler> _logger;

        public ProductUpdatedEventHandler(IMediator mediator, ILogger<ProductUpdatedEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task HandleAsync(ProductUpdatedEvent @event)
        {
            _logger.LogInformation("Handling ProductUpdatedEvent: {ProductId}", @event.ProductId);

            var command = new CreateLogCommand(
                "ProductService",
                "Information",
                $"Product updated: {@event.Name} (ID: {@event.ProductId})",
                null,
                null,
                new Dictionary<string, string>
                {
                { "ProductId", @event.ProductId.ToString() },
                { "ProductName", @event.Name },
                { "UpdatedBy", @event.UpdatedBy.ToString() },
                { "EventId", @event.Id.ToString() }
                }
            );

            await _mediator.Send(command);
        }
    }
}
