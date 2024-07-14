using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Queries.Extensions;

/// <summary>
///     Extension methods for <see cref="IQuery{TResult}" />.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    ///     Executes a query that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the query execution.
    /// </typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this IQuery<TResult> query,
        IServiceProvider serviceProvider,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var type = query.GetType();

        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(type, typeof(TResult));

        List<dynamic?> handlers = serviceProvider
            .GetServices(handlerType)
            .ToList();

        switch (handlers.Count)
        {
            case 0:
                throw new InvalidOperationException(
                    $"Handler for query type {type.Name} not registered.");
            case > 1:
                throw new InvalidOperationException(
                    $"Query has {handlers.Count} handlers, please register only one handler or make it internal!");
        }

        var handler = handlers[0]!;
        if (!handler.GetType().IsPublic)
            throw new InvalidOperationException(
                $"Handler for query type {type.Name} is not public.");

        return (TResult)await handler.HandleAsync((dynamic)query, ct);
    }
}