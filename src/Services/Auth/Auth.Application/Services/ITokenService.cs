using Auth.Domain.Entities;

namespace Auth.Application.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiration();
    }
}
