using Auth.Domain.Entities;

namespace Auth.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiration();
    }
}
