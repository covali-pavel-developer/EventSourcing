namespace EventSourcing.Commands.Concurrent;

/// <summary>
///     Defines the structure for commands that support concurrent execution
///     with a configurable concurrency limit.
/// </summary>
/// <remarks>
///     This interface is particularly useful in scenarios where multiple instances
///     of a command can be executed simultaneously up to a specified limit,
///     enhancing performance while controlling resource usage.
/// </remarks>
/// <typeparam name="TResult">
///     The type of the result that will be returned from the handler of this command.
/// </typeparam>
public interface IConcurrentCommand<out TResult>;