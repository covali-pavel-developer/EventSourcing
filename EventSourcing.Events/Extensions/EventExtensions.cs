using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Events.Extensions;

/// <summary>
///     Extension methods for <see cref="IEvent" />.
/// </summary>
public static class EventExtensions
{
    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task PublishAsync<TEvent>(
        this TEvent eventModel,
        IServiceProvider serviceProvider) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(eventModel);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var eventType = typeof(TEvent);
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

        IEnumerable<dynamic?> handlers =
            serviceProvider.GetServices(handlerType)
            ?? throw new InvalidOperationException(
                $"Handler for event type {eventType.Name} not registered.");

        foreach (var handler in handlers)
            if (handler != null)
                await handler.HandleAsync((dynamic)eventModel);
    }

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Publish<TEvent>(
        this TEvent eventModel,
        IServiceProvider serviceProvider) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(eventModel);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        Task.Run(async () => { await PublishAsync(eventModel, serviceProvider); });
    }
}