namespace EventSourcing.Commands.Concurrent;

public interface IConcurrentCommandBus
{
    /// <summary>
    ///     Subscribes a command handler to a command type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="handler">The command handler.</param>
    /// <exception cref="ArgumentNullException" />
    void Subscribe<TCommand, TResult>(
        IConcurrentCommandHandler<TCommand, TResult> handler
    ) where TCommand : IConcurrentCommand<TResult>;

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    Task<TResult> ExecuteAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken ct = default
    ) where TCommand : IConcurrentCommand<TResult>;

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    void Execute<TCommand, TResult>(
        TCommand command
    ) where TCommand : IConcurrentCommand<TResult>;
}