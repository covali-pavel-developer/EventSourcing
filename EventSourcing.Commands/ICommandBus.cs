namespace EventSourcing.Commands;

/// <summary>
///     Defines a command bus pattern for executing commands in a
///     CQRS (Command Query Responsibility Segregation) architecture.
/// </summary>
/// <remarks>
///     It provides mechanisms to subscribe command handlers to specific
///     command types and execute commands, either returning a result or not.
/// </remarks>
public interface ICommandBus
{
    /// <summary>
    ///     Subscribes a command handler to a command type.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="handler">The command handler.</param>
    /// <exception cref="ArgumentNullException" />
    void Subscribe<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand;

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
        ICommandHandler<TCommand, TResult> handler
    ) where TCommand : ICommand<TResult>;

    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    Task ExecuteAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default
    ) where TCommand : ICommand;

    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    void Execute<TCommand>(TCommand command) where TCommand : ICommand;

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
    ) where TCommand : ICommand<TResult>;

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
    ) where TCommand : ICommand<TResult>;
}