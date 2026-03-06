using Log.Domain.Entities;

namespace Log.Domain.Repositories
{
    public interface ILogRepository
    {
        Task AddAsync(LogEntry logEntry, CancellationToken cancellationToken = default);
        Task<LogEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntry>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntry>> GetByServiceAsync(string serviceName, int skip = 0, int take = 100, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntry>> GetByLevelAsync(string level, int skip = 0, int take = 100, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    }
}
