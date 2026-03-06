using MediatR;

namespace Product.Application.Commands
{
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;
}
