using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Commands
{
    public record RegisterCommand(
       string Email,
       string Password,
       string FirstName,
       string LastName,
       string IpAddress
   ) : IRequest<AuthResponse>;
}
