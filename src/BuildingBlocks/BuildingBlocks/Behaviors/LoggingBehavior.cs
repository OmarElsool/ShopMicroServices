using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) // inject Ilogger in primary ctor
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse> // should not be null and the request must inheirt from IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation($"[Start] Handle Request={typeof(TRequest).Name} - Response={typeof(TResponse).Name} - RequestData={request}");

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timetaken = timer.Elapsed;
        if (timetaken.Seconds > 3) {
            logger.LogWarning($"[Performance] the Resquest: {typeof(TRequest).Name} took {timetaken}");
        }

        logger.LogInformation($"[End] Handle Request={typeof(TRequest).Name} - Response={typeof(TResponse).Name}");
        return response;
    }
}
