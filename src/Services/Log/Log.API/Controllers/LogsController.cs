using Asp.Versioning;
using Log.Application.Commands;
using Log.Application.DTOs;
using Log.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Log.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LogsController> _logger;

        public LogsController(IMediator mediator, ILogger<LogsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LogEntryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLogRequest request)
        {
            try
            {
                var command = new CreateLogCommand(
                    request.ServiceName,
                    request.Level,
                    request.Message,
                    request.Exception,
                    request.StackTrace,
                    request.Properties
                );

                var logEntry = await _mediator.Send(command);

                _logger.LogInformation("Log entry created: {LogId}", logEntry.Id);
                return CreatedAtAction(nameof(Create), new { id = logEntry.Id }, logEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating log entry");
                return StatusCode(500, new { message = "An error occurred while creating the log entry" });
            }
        }

        [HttpGet("service/{serviceName}")]
        [ProducesResponseType(typeof(IEnumerable<LogEntryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByService(string serviceName, [FromQuery] int skip = 0, [FromQuery] int take = 100)
        {
            try
            {
                var query = new GetLogsByServiceQuery(serviceName, skip, take);
                var logs = await _mediator.Send(query);

                _logger.LogInformation("Retrieved {Count} logs for service {ServiceName}", logs.Count(), serviceName);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving logs for service {ServiceName}", serviceName);
                return StatusCode(500, new { message = "An error occurred while retrieving logs" });
            }
        }

        [HttpGet("level/{level}")]
        [ProducesResponseType(typeof(IEnumerable<LogEntryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLevel(string level, [FromQuery] int skip = 0, [FromQuery] int take = 100)
        {
            try
            {
                var query = new GetLogsByLevelQuery(level, skip, take);
                var logs = await _mediator.Send(query);

                _logger.LogInformation("Retrieved {Count} logs for level {Level}", logs.Count(), level);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving logs for level {Level}", level);
                return StatusCode(500, new { message = "An error occurred while retrieving logs" });
            }
        }
    }
}
