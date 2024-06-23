namespace EventSourcing.Events;

/// <summary>
///     Provides a common type that all specific event handler interfaces inherit from.
/// </summary>
/// <remarks>
///     This interface is used to identify event handler types and does not contain any members.
/// </remarks>
public interface IEventHandler;

/// <summary>
///     Defines interface to be implemented by event handler for a given event type.
/// </summary>
/// <typeparam name="TEvent">
///     The type of the event to be handled, which must implement <see cref="IEvent" />.
/// </typeparam>
public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IEvent
{
    /// <summary>
    ///     Handles the specified event.
    /// </summary>
    /// <param name="eventModel">The event model to be handled.</param>
    Task HandleAsync(TEvent eventModel);
}