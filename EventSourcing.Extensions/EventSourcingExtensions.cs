using System.Reflection;
using EventSourcing.Commands;
using EventSourcing.Commands.Concurrent;
using EventSourcing.Commands.Extensions;
using EventSourcing.Events;
using EventSourcing.Events.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventSourcing.Extensions;

/// <summary>
///     Represents event-sourcing extensions.
/// </summary>
public static class EventSourcingExtensions
{
    internal static IServiceProvider ServiceProvider = default!;
    internal static readonly ConcurrentCommandBus ConcurrentCommandBus = new();

    /// <summary>
    ///     Register event or command handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the event or command handlers will be added.
    /// </param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the event or command handlers will be registered.
    /// </param>
    public static IServiceCollection AddEventSourcing(
        this IServiceCollection services,
        params Type[] types)
    {
        var handlersTypes = types
            .Select(t => t.Assembly)
            .Distinct()
            .SelectMany(t => t.GetTypes())
            .Where(t => t is { IsInterface: false, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces(), (implementationType, serviceType) => new
            {
                implementationType,
                serviceType
            }
            )
            .Where(t => t.serviceType.IsGenericType);

        var commandGenericTypes = new List<Type>
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>)
        };

        var commandsHandlers = handlersTypes
            .Where(t => commandGenericTypes.Exists(
                x => x == t.serviceType.GetGenericTypeDefinition()))
            .ToList();

        var concurrentCommandGenericTypes = new List<Type>
        {
            typeof(IConcurrentCommandHandler<,>)
        };

        var concurrentCommandsHandlers = handlersTypes
            .Where(t => concurrentCommandGenericTypes.Exists(
                x => x == t.serviceType.GetGenericTypeDefinition()))
            .ToList();

        var eventGenericType = typeof(IEventHandler<>);
        var eventHandlers = handlersTypes
            .Where(t => t.serviceType == eventGenericType)
            .ToList();

        var handlers = commandsHandlers
            .Concat(eventHandlers)
            .Concat(concurrentCommandsHandlers);

        foreach (var handler in handlers)
            services.TryAddTransient(handler.serviceType, handler.implementationType);

        ServiceProvider = services.BuildServiceProvider();

        return services;
    }

    /// <summary>
    ///     Registers all event handlers from the assembly of the specified type.
    /// </summary>
    public static IServiceCollection AddGenericTransient(
        this IServiceCollection services,
        IEnumerable<Assembly> assembliesSource,
        params Type[] genericTypes)
    {
        var handlers = assembliesSource
            .Distinct()
            .SelectMany(t => t.GetTypes())
            .Where(t => t is { IsInterface: false, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces(), (implementationType, serviceType) => new
            {
                implementationType,
                serviceType
            }
            )
            .Where(t => t.serviceType.IsGenericType)
            .Where(t => Array.Exists(genericTypes,
                x => x == t.serviceType.GetGenericTypeDefinition()));

        foreach (var handler in handlers)
            services.TryAddTransient(handler.serviceType, handler.implementationType);

        return services;
    }

    #region [ Commands ]

    /// <summary>
    ///     Register command handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the command handlers will be added.
    /// </param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the command handlers will be registered.
    /// </param>
    public static IServiceCollection AddCommandHandlers(
        this IServiceCollection services,
        params Type[] types)
    {
        services.AddGenericTransient(
            types.Select(t => t.Assembly),
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>)
        );

        return services;
    }

    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task ExecuteAsync<TCommand>(
        this TCommand command,
        CancellationToken ct = default) where TCommand : ICommand
    {
        await command.ExecuteAsync(ServiceProvider, ct);
    }

    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Execute<TCommand>(this TCommand command) where TCommand : ICommand
    {
        Task.Run(async () => { await ExecuteAsync(command); });
    }

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this ICommand<TResult> command,
        CancellationToken ct = default)
    {
        return await command.ExecuteAsync(ServiceProvider, ct);
    }

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Execute<TResult>(this ICommand<TResult> command)
    {
        Task.Run(async () => { await ExecuteAsync(command); });
    }

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this IConcurrentCommand<TResult> command,
        CancellationToken ct = default)
    {
        return await ExecuteAsync(command, ServiceProvider, ct);
    }

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this IConcurrentCommand<TResult> command,
        IServiceProvider serviceProvider,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var type = command.GetType();

        var handlerType = typeof(IConcurrentCommandHandler<,>)
            .MakeGenericType(type, typeof(TResult));

        dynamic handler =
            serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"Handler for concurrent command type {type.Name} not registered.");

        return await ConcurrentCommandBus
            .ExecuteAsync<TResult>(type, command, handler, ct);
    }

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Execute<TResult>(this IConcurrentCommand<TResult> command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Task.Run(async () => { await ExecuteAsync(command); });
    }

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Execute<TResult>(
        this IConcurrentCommand<TResult> command,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        Task.Run(async () => { await ExecuteAsync(command, serviceProvider); });
    }

    #endregion

    #region [ Events ]

    /// <summary>
    ///     Register event handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the command handlers will be added.
    /// </param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the command handlers will be registered.
    /// </param>
    public static IServiceCollection AddEventHandlers(
        this IServiceCollection services,
        params Type[] types)
    {
        services.AddGenericTransient(
            types.Select(t => t.Assembly),
            typeof(IEventHandler<>)
        );

        ServiceProvider = services.BuildServiceProvider();

        return services;
    }

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <exception cref="InvalidOperationException" />
    public static async Task PublishAsync<TEvent>(this TEvent eventModel) where TEvent : IEvent
    {
        await eventModel.PublishAsync(ServiceProvider);
    }

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <exception cref="InvalidOperationException" />
    public static void Publish<TEvent>(this TEvent eventModel) where TEvent : IEvent
    {
        Task.Run(async () => { await PublishAsync(eventModel); });
    }

    #endregion
}