namespace EventSourcing.Commands;

/// <summary>
///     Provides a common type that all specific command handler interfaces inherit from.
/// </summary>
public interface ICommandHandler;

/// <summary>
///     Defines interface to be implemented by a command handler
///     for a given command type that does not return a result.
/// </summary>
/// <typeparam name="TCommand">
///     The type of the command to be handled, which must implement <see cref="ICommand" />.
/// </typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
{
    /// <summary>
    ///     Executes command and does not return a result.
    /// </summary>
    /// <param name="command">The input command object.</param>
    /// <param name="ct">Optional cancellation token.</param>
    Task HandleAsync(TCommand command, CancellationToken ct = default);
}

/// <summary>
///     Defines interface to be implemented by a command handler
///     for a given command type that returns a result.
/// </summary>
/// <typeparam name="TCommand">
///     The type of the input command, which must implement <see cref="ICommand{TResult}" />.
/// </typeparam>
/// <typeparam name="TResult">
///     The type of the result returned by the command handler.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
{
    /// <summary>
    ///     Executes command and returns a result.
    /// </summary>
    /// <param name="command">The input command object.</param>
    /// <param name="ct">Optional cancellation token.</param>
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}