﻿namespace EventSourcing.Commands.Extensions;

/// <summary>
///     Extension methods for <see cref="ICommand" />.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task ExecuteAsync<TCommand>(
        this TCommand command,
        IServiceProvider serviceProvider,
        CancellationToken ct = default) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var type = typeof(TCommand);

        var handlerType = typeof(ICommandHandler<>)
            .MakeGenericType(type);

        dynamic handler =
            serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"Handler for command type {type.Name} not registered.");

        await handler.HandleAsync((dynamic)command, ct);
    }

    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Execute<TCommand>(
        this TCommand command,
        IServiceProvider serviceProvider) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        Task.Run(async () => { await ExecuteAsync(command, serviceProvider); });
    }

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this ICommand<TResult> command,
        IServiceProvider serviceProvider,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var type = command.GetType();

        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(type, typeof(TResult));

        dynamic handler =
            serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"Handler for command type {type.Name} not registered.");

        return (TResult)await handler.HandleAsync((dynamic)command, ct);
    }

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Execute<TResult>(
        this ICommand<TResult> command,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        Task.Run(async () => { await ExecuteAsync(command, serviceProvider); });
    }
}