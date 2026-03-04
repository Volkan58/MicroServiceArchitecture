using EventBus.Events;

namespace Product.Application.Events
{
    public class ProductUpdatedEvent : IntegrationEvent
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Category { get; set; }
        public Guid UpdatedBy { get; set; }

        public ProductUpdatedEvent(Guid productId, string name, string description, decimal price, int stock, string category, Guid updatedBy)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Category = category;
            UpdatedBy = updatedBy;
        }
    }
}
