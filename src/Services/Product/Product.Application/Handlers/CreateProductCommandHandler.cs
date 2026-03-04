using EventBus.Abstractions;
using MediatR;
using Product.Application.Commands;
using Product.Application.DTOs;
using Product.Application.Events;
using Product.Domain.Repositories;

namespace Product.Application.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventBus _eventBus;

        public CreateProductCommandHandler(IProductRepository productRepository, IEventBus eventBus)
        {
            _productRepository = productRepository;
            _eventBus = eventBus;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Domain.Entities.Product(
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                request.Category,
                request.UserId
            );

            await _productRepository.AddAsync(product, cancellationToken);

            var @event = new ProductCreatedEvent(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock,
                product.Category,
                product.CreatedBy
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
