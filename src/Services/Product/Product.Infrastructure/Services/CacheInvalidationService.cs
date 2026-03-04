using Microsoft.Extensions.Caching.Distributed;
using Product.Application.Services;

namespace Product.Infrastructure.Services
{
    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly IDistributedCache _cache;

        public CacheInvalidationService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task InvalidateProductCacheAsync(Guid productId, string? category = null, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync($"product:{productId}", cancellationToken);
            await _cache.RemoveAsync("products:all", cancellationToken);

            if (!string.IsNullOrEmpty(category))
            {
                await _cache.RemoveAsync($"products:category:{category}", cancellationToken);
            }
        }

        public async Task InvalidateAllProductsCacheAsync(CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync("products:all", cancellationToken);
        }
    }
}
