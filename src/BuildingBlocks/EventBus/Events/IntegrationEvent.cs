namespace EventBus.Events
{
    public abstract class IntegrationEvent
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        protected IntegrationEvent(Guid id, DateTime createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
        }
    }
}
