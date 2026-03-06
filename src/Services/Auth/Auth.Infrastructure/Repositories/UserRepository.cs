using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;


public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (tokenEntity == null) return null;

        var userId = _context.Entry(tokenEntity).Property<Guid>("UserId").CurrentValue;
        return await GetByIdAsync(userId, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(Guid userId, RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Add(refreshToken);
        _context.Entry(refreshToken).Property("UserId").CurrentValue = userId;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(user).State == EntityState.Detached)
        {
            _context.Users.Update(user);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email!, cancellationToken);
    }
}
