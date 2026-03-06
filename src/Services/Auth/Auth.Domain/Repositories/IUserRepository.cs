using Auth.Domain.Entities;

namespace Auth.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task AddRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    }
}
