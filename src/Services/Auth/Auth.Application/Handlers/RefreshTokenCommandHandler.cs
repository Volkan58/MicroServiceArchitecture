using Auth.Application.Commands;
using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using MediatR;

namespace Auth.Application.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var oldRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

            if (oldRefreshToken == null || !oldRefreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiration = _tokenService.GetRefreshTokenExpiration();
            var refreshTokenEntity = new RefreshToken(newRefreshToken, refreshTokenExpiration, request.IpAddress);

            oldRefreshToken.Revoke(request.IpAddress, newRefreshToken);

            await _userRepository.AddRefreshTokenAsync(user.Id, refreshTokenEntity, cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user);

            return new AuthResponse(
                accessToken,
                newRefreshToken,
                refreshTokenExpiration,
                new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Roles)
            );
        }
    }
}
