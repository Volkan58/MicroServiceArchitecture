using Auth.Application.DTOs;
using MediatR;

namespace Auth.Application.Commands
{
    public record RefreshTokenCommand(
      string RefreshToken,
      string IpAddress
  ) : IRequest<AuthResponse>;
}
