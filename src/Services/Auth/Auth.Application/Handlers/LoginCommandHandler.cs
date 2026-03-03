using Auth.Application.Commands;
using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using MediatR;

namespace Auth.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }
        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is deactivated");
            }

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiration = _tokenService.GetRefreshTokenExpiration();
            var refreshTokenEntity = new RefreshToken(refreshToken, refreshTokenExpiration, request.IpAddress);

            user.AddRefreshToken(refreshTokenEntity);

            await _userRepository.UpdateAsync(user, cancellationToken);

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
