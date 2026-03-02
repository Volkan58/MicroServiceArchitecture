using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent @event);
    }
}
