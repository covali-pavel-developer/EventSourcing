namespace EventSourcing.Commands.Concurrent;

public interface IConcurrentCommandBus
{
    /// <summary>
    ///     Subscribes a concurrent command handler to a command type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the concurrent command.</typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the concurrent command execution.
    /// </typeparam>
    /// <param name="handler">The concurrent command handler.</param>
    /// <exception cref="ArgumentNullException" />
    void Subscribe<TCommand, TResult>(
        IConcurrentCommandHandler<TCommand, TResult> handler
    ) where TCommand : IConcurrentCommand<TResult>;

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the concurrent command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the concurrent command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    Task<TResult> ExecuteAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken ct = default
    ) where TCommand : IConcurrentCommand<TResult>;

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the concurrent command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the concurrent command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    void Execute<TCommand, TResult>(
        TCommand command
    ) where TCommand : IConcurrentCommand<TResult>;

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the concurrent command execution.
    /// </typeparam>
    /// <param name="type">The type of concurrent command handler.</param>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <param name="handler">The concurrent command handler.</param>
    /// <exception cref="ArgumentNullException" />
    Task<TResult> ExecuteAsync<TResult>(
        Type type,
        IConcurrentCommand<TResult> command,
        dynamic handler,
        CancellationToken ct = default
    );
}