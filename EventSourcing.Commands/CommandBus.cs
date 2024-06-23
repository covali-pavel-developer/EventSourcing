using System.Collections.Concurrent;

namespace EventSourcing.Commands;

/// <inheritdoc cref="ICommandBus" />
public class CommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<string, object> _handlers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandBus" /> class.
    /// </summary>
    public CommandBus()
    {
        _handlers = new ConcurrentDictionary<string, object>();
    }

    /// <inheritdoc />
    public void Subscribe<TCommand>(
        ICommandHandler<TCommand> handler) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(handler);
        _handlers[typeof(TCommand).Name] = handler;
    }

    /// <inheritdoc />
    public void Subscribe<TCommand, TResult>(
        ICommandHandler<TCommand, TResult> handler)
        where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(handler);
        _handlers[typeof(TCommand).Name] = handler;
    }

    /// <inheritdoc />
    public async Task ExecuteAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        var type = command.GetType().Name;

        if (!_handlers.TryGetValue(type, out var handler)
            || handler is not ICommandHandler<TCommand> commandHandler)
            throw new InvalidOperationException(
                $"Handler for command type {type} not registered.");

        await commandHandler.HandleAsync(command, ct);
    }

    /// <inheritdoc />
    public void Execute<TCommand>(TCommand command) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        Task.Run(async () => { await ExecuteAsync(command); });
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken ct = default) where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);
        var type = command.GetType().Name;

        if (!_handlers.TryGetValue(type, out var handler)
            || handler is not ICommandHandler<TCommand, TResult> commandHandler)
            throw new InvalidOperationException(
                $"Handler for command type {type} not registered.");

        return await commandHandler.HandleAsync(command, ct);
    }

    /// <inheritdoc />
    public void Execute<TCommand, TResult>(
        TCommand command) where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);
        Task.Run(async () => { await ExecuteAsync<TCommand, TResult>(command); });
    }
}