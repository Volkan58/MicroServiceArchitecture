using Microsoft.EntityFrameworkCore;
using Product.Domain.Repositories;
using Product.Infrastructure.Persistence;

namespace Product.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await GetByIdAsync(id, cancellationToken);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
        }
    }
}
