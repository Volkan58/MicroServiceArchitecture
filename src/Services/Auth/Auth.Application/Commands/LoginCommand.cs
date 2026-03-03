using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Commands
{
    public record LoginCommand(
       string Email,
       string Password,
       string IpAddress
   ) : IRequest<AuthResponse>;
}
