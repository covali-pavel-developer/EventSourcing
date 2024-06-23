namespace EventSourcing.Commands.Concurrent;

/// <summary>
///     Defines the structure for command handlers that handle commands supporting
///     concurrent execution with a configurable concurrency limit.
/// </summary>
/// <remarks>
///     This interface extends the basic command handler interface to include
///     support for handling concurrent commands, returning a result upon execution.
/// </remarks>
/// <typeparam name="TCommand">
///     The type of the input command, which must implement <see cref="IConcurrentCommand{TResult}" />
/// </typeparam>
/// <typeparam name="TResult">
///     The type of the result returned by the command handler.
/// </typeparam>
public interface IConcurrentCommandHandler<in TCommand, TResult>
    : ICommandHandler where TCommand : IConcurrentCommand<TResult>
{
    /// <summary>
    ///     Gets or sets the maximum number of concurrent
    ///     instances allowed for this command.
    /// </summary>
    int ConcurrentCount { get; init; }

    /// <summary>
    ///     Executes command and returns a result.
    /// </summary>
    /// <param name="command">The input command object.</param>
    /// <param name="ct">Optional cancellation token.</param>
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}