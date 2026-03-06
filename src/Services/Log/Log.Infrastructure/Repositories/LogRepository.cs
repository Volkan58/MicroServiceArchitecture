using Log.Domain.Entities;
using Log.Domain.Repositories;
using Log.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Log.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly LogDbContext _context;

        public LogRepository(LogDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
        {
            await _context.LogEntries.AddAsync(logEntry, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<LogEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.LogEntries.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<LogEntry>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            return await _context.LogEntries
                .OrderByDescending(l => l.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntry>> GetByServiceAsync(string serviceName, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            return await _context.LogEntries
                .Where(l => l.ServiceName == serviceName)
                .OrderByDescending(l => l.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntry>> GetByLevelAsync(string level, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            return await _context.LogEntries
                .Where(l => l.Level == level)
                .OrderByDescending(l => l.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            return await _context.LogEntries
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .OrderByDescending(l => l.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
