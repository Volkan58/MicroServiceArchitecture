using EventBus.Abstractions;
using MediatR;
using Product.Application.Commands;
using Product.Application.DTOs;
using Product.Application.Events;
using Product.Application.Services;
using Product.Domain.Repositories;

namespace Product.Application.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventBus _eventBus;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public UpdateProductCommandHandler(
            IProductRepository productRepository,
            IEventBus eventBus,
            ICacheInvalidationService cacheInvalidationService)
        {
            _productRepository = productRepository;
            _eventBus = eventBus;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.Id} not found");
            }

            product.Update(
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                request.Category,
                request.UserId
            );

            await _productRepository.UpdateAsync(product, cancellationToken);

            await _cacheInvalidationService.InvalidateProductCacheAsync(
                product.Id,
                product.Category,
                cancellationToken);

            var @event = new ProductUpdatedEvent(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.Category,
                request.UserId
            );

            await _eventBus.PublishAsync(@event);

            return new ProductDto(
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
        }
    }
}
