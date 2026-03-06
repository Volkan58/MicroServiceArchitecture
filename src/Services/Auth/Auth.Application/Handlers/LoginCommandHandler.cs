using Auth.Application.Commands;
using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
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

        await _userRepository.AddRefreshTokenAsync(user.Id, refreshTokenEntity, cancellationToken);

        var accessToken = await _tokenService.GenerateAccessTokenAsync(user, cancellationToken);
        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse(
            accessToken,
            refreshToken,
            refreshTokenExpiration,
            new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, roles.ToList())
        );
    }
}
