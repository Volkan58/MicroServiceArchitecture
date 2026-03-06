using Log.Application.DTOs;
using MediatR;

namespace Log.Application.Commands
{
    public record CreateLogCommand(
      string ServiceName,
      string Level,
      string Message,
      string? Exception,
      string? StackTrace,
      Dictionary<string, string>? Properties
  ) : IRequest<LogEntryDto>;
}
