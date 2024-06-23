using System.Collections.Concurrent;
using EventSourcing.Commands.Concurrent.Internal;

namespace EventSourcing.Commands.Concurrent;

/// <inheritdoc cref="IConcurrentCommandBus" />
public class ConcurrentCommandBus : IConcurrentCommandBus
{
    private readonly ConcurrentDictionary<string, ConcurrentHandler> _handlers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CommandBus" /> class.
    /// </summary>
    public ConcurrentCommandBus()
    {
        _handlers = new ConcurrentDictionary<string, ConcurrentHandler>();
    }

    /// <inheritdoc />
    public void Subscribe<TCommand, TResult>(
        IConcurrentCommandHandler<TCommand, TResult> handler
    )
        where TCommand : IConcurrentCommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(handler);

        var concurrentCount = handler.ConcurrentCount;
        if (concurrentCount <= 0) concurrentCount = 1;

        _handlers[typeof(TCommand).Name] = new ConcurrentHandler(
            handler,
            new SemaphoreSlim(concurrentCount, concurrentCount)
        );
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken ct = default
    )
        where TCommand : IConcurrentCommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);
        var type = command.GetType().Name;

        if (!_handlers.TryGetValue(type, out var handler)
            || handler.Handler is not IConcurrentCommandHandler<TCommand, TResult> commandHandler)
            throw new InvalidOperationException(
                $"Handler for command type {type} not registered.");

        try
        {
            await handler.Semaphore.WaitAsync(ct);
            return await commandHandler.HandleAsync(command, ct);
        }
        finally
        {
            handler.Semaphore.Release();
        }
    }

    public void Execute<TCommand, TResult>(TCommand command)
        where TCommand : IConcurrentCommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);
        Task.Run(async () => { await ExecuteAsync<TCommand, TResult>(command); });
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteAsync<TResult>(
        Type type,
        IConcurrentCommand<TResult> command,
        dynamic handler,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(handler);

        var concurrentCount = handler.ConcurrentCount;
        if (concurrentCount <= 0) concurrentCount = 1;

        var concurrentHandler = _handlers
            .GetOrAdd(type.Name, _ => new ConcurrentHandler(
                handler,
                new SemaphoreSlim(concurrentCount, concurrentCount)
            ));

        try
        {
            await concurrentHandler.Semaphore.WaitAsync(ct);
            return (TResult)await handler.HandleAsync((dynamic)command, ct);
        }
        finally
        {
            concurrentHandler.Semaphore.Release();
        }
    }
}