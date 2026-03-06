using Log.Application.Commands;
using Log.Application.DTOs;
using Log.Domain.Entities;
using Log.Domain.Repositories;
using MediatR;

namespace Log.Application.Handlers
{
    public class CreateLogCommandHandler : IRequestHandler<CreateLogCommand, LogEntryDto>
    {
        private readonly ILogRepository _logRepository;

        public CreateLogCommandHandler(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<LogEntryDto> Handle(CreateLogCommand request, CancellationToken cancellationToken)
        {
            var logEntry = new LogEntry(
                request.ServiceName,
                request.Level,
                request.Message,
                request.Exception,
                request.StackTrace,
                request.Properties
            );

            await _logRepository.AddAsync(logEntry, cancellationToken);

            return new LogEntryDto(
                logEntry.Id,
                logEntry.ServiceName,
                logEntry.Level,
                logEntry.Message,
                logEntry.Exception,
                logEntry.StackTrace,
                logEntry.Properties,
                logEntry.Timestamp
            );
        }
    }
}
