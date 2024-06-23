using EventSourcing.Api.Models;
using EventSourcing.Commands.Concurrent;

namespace EventSourcing.Api.Commands;

public class ConcurrentCommand(int number) : IConcurrentCommand<BaseResult>
{
    public int Number { get; init; } = number;
}

public class ConcurrentCommandHandler(ILogger<ConcurrentCommandHandler> logger)
    : IConcurrentCommandHandler<ConcurrentCommand, BaseResult>
{
    public int ConcurrentCount { get; init; } = 1;

    public async Task<BaseResult> HandleAsync(ConcurrentCommand command, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);

        var message = $"{nameof(ConcurrentCommandHandler)}-{command.Number} handled at {DateTime.Now:HH:mm:ss.fff}";
        logger.LogInformation(message + "\n");
        return new BaseResult(command.Number, message);
    }
}