namespace EventSourcing.Queries;

/// <summary>
///     Provides a query type that all specific query handler interfaces inherit from.
/// </summary>
public interface IQueryHandler;

/// <summary>
///     Defines interface to be implemented by a query handler
///     for a given query type that returns a result.
/// </summary>
/// <typeparam name="TQuery">
///     The type of the input query, which must implement <see cref="IQuery{TResult}" />.
/// </typeparam>
/// <typeparam name="TResult">
///     The type of the result returned by the query handler.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResult> : IQueryHandler where TQuery : IQuery<TResult>
{
    /// <summary>
    ///     Executes query and returns a result.
    /// </summary>
    /// <param name="query">The input query object.</param>
    /// <param name="ct">Optional cancellation token.</param>
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}