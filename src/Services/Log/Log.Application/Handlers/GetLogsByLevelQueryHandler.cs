using Log.Application.DTOs;
using Log.Application.Queries;
using Log.Domain.Repositories;
using MediatR;

namespace Log.Application.Handlers
{
    public class GetLogsByLevelQueryHandler : IRequestHandler<GetLogsByLevelQuery, IEnumerable<LogEntryDto>>
    {
        private readonly ILogRepository _logRepository;

        public GetLogsByLevelQueryHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<LogEntryDto>> Handle(GetLogsByLevelQuery request, CancellationToken cancellationToken)
        {
            var logs = await _logRepository.GetByLevelAsync(request.Level, request.Skip, request.Take, cancellationToken);

            return logs.Select(log => new LogEntryDto(
                log.Id,
                log.ServiceName,
                log.Level,
                log.Message,
                log.Exception,
                log.StackTrace,
                log.Properties,
                log.Timestamp
            ));
        }

    }
}
