using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Product.Application.DTOs;
using Product.Application.Queries;
using Product.Domain.Repositories;

namespace Product.Application.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IDistributedCache _cache;
        private const int CacheExpirationMinutes = 15;

        public GetAllProductsQueryHandler(IProductRepository productRepository, IDistributedCache cache)
        {
            _productRepository = productRepository;
            _cache = cache;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "products:all";
            var cachedProducts = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(cachedProducts) ?? Enumerable.Empty<ProductDto>();
            }

            var products = await _productRepository.GetAllAsync(cancellationToken);

            var productDtos = products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock,
                p.Category,
                p.IsActive,
                p.CreatedAt,
                p.UpdatedAt
            )).ToList();

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonConvert.SerializeObject(productDtos),
                cacheOptions,
                cancellationToken
            );

            return productDtos;
        }
    }
}
