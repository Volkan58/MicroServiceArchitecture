using MediatR;
using Product.Application.Commands;
using Product.Application.Services;
using Product.Domain.Repositories;

namespace Product.Application.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheInvalidationService _cacheInvalidationService;

        public DeleteProductCommandHandler(
            IProductRepository productRepository,
            ICacheInvalidationService cacheInvalidationService)
        {
            _productRepository = productRepository;
            _cacheInvalidationService = cacheInvalidationService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(request.Id, cancellationToken);

            await _cacheInvalidationService.InvalidateProductCacheAsync(
                request.Id,
                product.Category,
                cancellationToken);

            return true;
        }
    }
}
