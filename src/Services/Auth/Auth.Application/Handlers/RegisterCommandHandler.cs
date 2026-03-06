using Auth.Application.Commands;
using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Handlers;


public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsAsync(request.Email, cancellationToken))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new ApplicationUser(request.Email, request.Email, request.FirstName, request.LastName);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, "User");

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
