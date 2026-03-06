namespace Log.Application.DTOs
{
    public record LogEntryDto(
       Guid Id,
       string ServiceName,
       string Level,
       string Message,
       string? Exception,
       string? StackTrace,
       Dictionary<string, string> Properties,
       DateTime Timestamp
   );

    public record CreateLogRequest(
        string ServiceName,
        string Level,
        string Message,
        string? Exception,
        string? StackTrace,
        Dictionary<string, string>? Properties
    );

}
