using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Commands
{
    public record UpdateProductCommand(
       Guid Id,
       string Name,
       string Description,
       decimal Price,
       int Stock,
       string Category,
       Guid UserId
   ) : IRequest<ProductDto>;

}
