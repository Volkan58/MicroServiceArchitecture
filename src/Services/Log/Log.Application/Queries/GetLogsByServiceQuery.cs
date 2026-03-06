using Log.Application.DTOs;
using MediatR;

namespace Log.Application.Queries
{
    public record GetLogsByServiceQuery(string ServiceName, int Skip = 0, int Take = 100) : IRequest<IEnumerable<LogEntryDto>>;

}
