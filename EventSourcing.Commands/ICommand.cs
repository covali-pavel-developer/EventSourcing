namespace EventSourcing.Commands;

/// <summary>
///     Represents a command that does not return any result upon execution.
/// </summary>
/// <remarks>
///     Typically used for actions that modify the state of the application
///     without producing a return value.
/// </remarks>
public interface ICommand;

/// <summary>
///     Represents a command that returns a <typeparamref name="TResult" /> upon execution.
/// </summary>
/// <remarks>
///     Used for actions that need to return data or a status after execution.
/// </remarks>
/// <typeparam name="TResult">
///     Type of the result that will be returned from the handler of this command
///     (i.e. <see cref="ICommandHandler{TCommand, TResult}" />)
/// </typeparam>
public interface ICommand<out TResult>;