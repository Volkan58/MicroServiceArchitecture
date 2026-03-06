using Log.Application.DTOs;
using Log.Application.Queries;
using Log.Domain.Repositories;
using MediatR;

namespace Log.Application.Handlers
{
    public class GetLogsByServiceQueryHandler : IRequestHandler<GetLogsByServiceQuery, IEnumerable<LogEntryDto>>
    {
        private readonly ILogRepository _logRepository;

        public GetLogsByServiceQueryHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<LogEntryDto>> Handle(GetLogsByServiceQuery request, CancellationToken cancellationToken)
        {
            var logs = await _logRepository.GetByServiceAsync(request.ServiceName, request.Skip, request.Take, cancellationToken);

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
