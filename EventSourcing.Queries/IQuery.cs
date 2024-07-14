namespace EventSourcing.Queries;

/// <summary>
///     Represents a query that returns a <typeparamref name="TResult" /> upon execution.
/// </summary>
/// <remarks>
///     Used for actions that need to return data.
/// </remarks>
/// <typeparam name="TResult">
///     Type of the result that will be returned from the handler of this query
///     (i.e. <see cref="IQueryHandler{TQuery,TResult}" />)
/// </typeparam>
public interface IQuery<out TResult>;