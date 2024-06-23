using EventSourcing.Api.Models;
using EventSourcing.Commands;

namespace EventSourcing.Api.Commands;

public class SampleCommand(int number) : ICommand<BaseResult>
{
    public int Number { get; init; } = number;
}

public class TestCommandHandler(ILogger<TestCommandHandler> logger)
    : ICommandHandler<SampleCommand, BaseResult>
{
    public async Task<BaseResult> HandleAsync(SampleCommand command, CancellationToken ct)
    {
        var message = $"{nameof(ConcurrentCommandHandler)}-{command.Number} handled at {DateTime.Now:HH:mm:ss.fff}";
        logger.LogInformation(message + "\n");
        return new BaseResult(command.Number, message);
    }
}