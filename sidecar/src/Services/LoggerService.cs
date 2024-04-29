using Grpc.Core;
using UtilitySidecar;

namespace UtilitySidecar.Services;

public class LoggerService : Logger.LoggerBase
{
    private readonly ILogger<LoggerService> _logger;
    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger;
    }

    public override Task<LogEventReply> SendLogEvent(LogEventRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LogEventReply
        {
            Message = "Received " + request.Message
        });
    }
}
