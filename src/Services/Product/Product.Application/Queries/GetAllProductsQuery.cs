using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Queries
{
    public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
}
