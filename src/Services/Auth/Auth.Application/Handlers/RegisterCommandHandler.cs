using Auth.Application.Commands;
using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using MediatR;

namespace Auth.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }
        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var user = new User(request.Email, passwordHash, request.FirstName, request.LastName);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiration = _tokenService.GetRefreshTokenExpiration();
            var refreshTokenEntity = new RefreshToken(refreshToken, refreshTokenExpiration, request.IpAddress);

            user.AddRefreshToken(refreshTokenEntity);

            await _userRepository.AddAsync(user, cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user);

            return new AuthResponse(
                accessToken,
                refreshToken,
                refreshTokenExpiration,
                new UserDto(user.Id, user.Email, user.FirstName, user.LastName, user.Roles)
            );
        }
    }
}
