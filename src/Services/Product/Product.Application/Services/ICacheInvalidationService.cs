namespace Product.Application.Services
{
    public interface ICacheInvalidationService
    {
        Task InvalidateProductCacheAsync(Guid productId, string? category = null, CancellationToken cancellationToken = default);
        Task InvalidateAllProductsCacheAsync(CancellationToken cancellationToken = default);
    }
}
