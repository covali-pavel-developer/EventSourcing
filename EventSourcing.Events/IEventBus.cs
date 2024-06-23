namespace EventSourcing.Events;

/// <summary>
///     Defines an event bus pattern, facilitating the publish-subscribe model for
///     event handling in a CQRS (Command Query Responsibility Segregation) architecture.
/// </summary>
/// <remarks>
///     It allows event handlers to be subscribed to specific event types
///     and publishes events to all subscribed handlers.
/// </remarks>
public interface IEventBus
{
    /// <summary>
    ///     Subscribes an event handler to an event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="handler">The event handler.</param>
    void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent;

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <exception cref="InvalidOperationException" />
    Task PublishAsync<TEvent>(TEvent eventModel) where TEvent : IEvent;

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <exception cref="InvalidOperationException" />
    void Publish<TEvent>(TEvent eventModel) where TEvent : IEvent;
}