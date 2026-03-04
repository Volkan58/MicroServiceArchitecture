using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Product.Application.DTOs;
using Product.Application.Queries;
using Product.Domain.Repositories;

namespace Product.Application.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IDistributedCache _cache;
        private const int CacheExpirationMinutes = 30;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IDistributedCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"product:{request.Id}";
            var cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                return JsonConvert.DeserializeObject<ProductDto>(cachedProduct);
            }

            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product == null)
            {
                return null;
            }

            var productDto = new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.Category,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt
            );

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonConvert.SerializeObject(productDto),
                cacheOptions,
                cancellationToken
            );

            return productDto;
        }
    }
}
