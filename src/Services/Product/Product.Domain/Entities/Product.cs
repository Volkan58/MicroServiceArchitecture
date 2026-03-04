namespace Product.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string Category { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public Guid? UpdatedBy { get; private set; }

        private Product() { }

        public Product(string name, string description, decimal price, int stock, string category, Guid createdBy)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Price = price >= 0 ? price : throw new ArgumentException("Price must be non-negative", nameof(price));
            Stock = stock >= 0 ? stock : throw new ArgumentException("Stock must be non-negative", nameof(stock));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public void Update(string name, string description, decimal price, int stock, string category, Guid updatedBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Price = price >= 0 ? price : throw new ArgumentException("Price must be non-negative", nameof(price));
            Stock = stock >= 0 ? stock : throw new ArgumentException("Stock must be non-negative", nameof(stock));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public void UpdateStock(int quantity)
        {
            if (Stock + quantity < 0)
            {
                throw new InvalidOperationException("Insufficient stock");
            }

            Stock += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
