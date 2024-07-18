﻿using System.Diagnostics;
using System.Reflection;
using EventSourcing.Commands;
using EventSourcing.Commands.Concurrent;
using EventSourcing.Commands.Extensions;
using EventSourcing.Events;
using EventSourcing.Events.Extensions;
using EventSourcing.Queries;
using EventSourcing.Queries.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Extensions;

/// <summary>
///     Represents event-sourcing extensions.
/// </summary>
public static class EventSourcingExtensions
{
    internal static readonly ConcurrentCommandBus ConcurrentCommandBus = new();

    /// <summary>
    ///     Register handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the handlers will be added.
    /// </param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the handlers will be registered.
    /// </param>
    public static IServiceCollection AddEventSourcing(
        this IServiceCollection services,
        params Type[] types)
    {
        return AddEventSourcing(services, ServiceLifetime.Transient, types);
    }

    /// <summary>
    ///     Register handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the handlers will be added.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the handlers will be registered.
    /// </param>
    public static IServiceCollection AddEventSourcing(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        params Type[] types)
    {
        EventSourcingFactory.Provider = services.BuildServiceProvider();

        EventSourcingFactory.Logger = services
            .BuildServiceProvider()
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(EventSourcing));

        var loggerEnabled = EventSourcingFactory.Logger.IsEnabled(LogLevel.Debug);
        var sw = new Stopwatch();
        if (loggerEnabled) EventSourcingFactory.Logger
            .LogDebug("Starting to register handlers.");

        sw.Start();
        var handlersTypes = types
            .Select(t => t.Assembly)
            .Distinct()
            .SelectMany(t => t.GetTypes())
            .Where(t => t is { IsInterface: false, IsAbstract: false, IsPublic: true })
            .SelectMany(t => t.GetInterfaces(), (implementationType, serviceType) =>
                new ServiceDescriptor(serviceType, implementationType, lifetime))
            .Where(t => t.ServiceType.IsGenericType)
            .ToList();

        var counter = services.AddCommandHandlers(handlersTypes);
        counter += services.AddConcurrentCommandHandlers(handlersTypes);
        counter += services.AddEventHandlers(handlersTypes);
        counter += services.AddQueryHandlers(handlersTypes);
        sw.Stop();

        if (loggerEnabled) EventSourcingFactory.Logger
            .LogDebug("Finished to register {Count} handlers in {ElapsedMilliseconds} ms.",
                counter, sw.ElapsedMilliseconds);

        EventSourcingFactory.Provider = services.BuildServiceProvider();

        return services;
    }

    /// <summary>
    ///     Register command handlers to the <see cref="IServiceCollection" />
    ///     from the provided collection of <see cref="ServiceDescriptor" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the command handlers will be added.</param>
    /// <param name="handlerServices">
    ///     A collection of <see cref="ServiceDescriptor" />
    ///     representing the handler services to be added.
    /// </param>
    /// <param name="logger">An optional <see cref="ILogger" /> instance for logging debug information.</param>
    /// <returns>The number of command handlers registered.</returns>
    public static int AddCommandHandlers(
        this IServiceCollection services,
        IEnumerable<ServiceDescriptor> handlerServices,
        ILogger? logger = null)
    {
        var loggerEnabled = logger != null && logger.IsEnabled(LogLevel.Debug);
        var genericTypes = new List<Type>
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>)
        };

        var handlers = handlerServices
            .Where(t => genericTypes.Exists(
                x => x == t.ServiceType.GetGenericTypeDefinition()))
            .ToList();

        for (var index = 0; index < handlers.Count; index++)
        {
            var handler = handlers[index];
            services.Add(handler);
            if (loggerEnabled)
                logger!.LogDebug("Registered command handler[{Count}]: {HandlerName}",
                    index, handler.ImplementationType?.FullName);
        }

        return handlers.Count;
    }

    /// <summary>
    ///     Register concurrent command handlers to the <see cref="IServiceCollection" />
    ///     from the provided collection of <see cref="ServiceDescriptor" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the concurrent command handlers will be added.</param>
    /// <param name="handlerServices">
    ///     A collection of <see cref="ServiceDescriptor" />
    ///     representing the handler services to be added.
    /// </param>
    /// <param name="logger">An optional <see cref="ILogger" /> instance for logging debug information.</param>
    /// <returns>The number of concurrent command handlers registered.</returns>
    public static int AddConcurrentCommandHandlers(
        this IServiceCollection services,
        IEnumerable<ServiceDescriptor> handlerServices,
        ILogger? logger = null)
    {
        var loggerEnabled = logger != null && logger.IsEnabled(LogLevel.Debug);
        var genericTypes = new List<Type>
        {
            typeof(IConcurrentCommandHandler<,>)
        };

        var handlers = handlerServices
            .Where(t => genericTypes.Exists(
                x => x == t.ServiceType.GetGenericTypeDefinition()))
            .ToList();

        for (var index = 0; index < handlers.Count; index++)
        {
            var handler = handlers[index];
            services.Add(handler);
            if (loggerEnabled)
                logger!.LogDebug("Registered concurrent command handler[{Count}]: {HandlerName}",
                    index, handler.ImplementationType?.FullName);
        }

        return handlers.Count;
    }

    /// <summary>
    ///     Register event handlers to the <see cref="IServiceCollection" />
    ///     from the provided collection of <see cref="ServiceDescriptor" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the event handlers will be added.</param>
    /// <param name="handlerServices">
    ///     A collection of <see cref="ServiceDescriptor" />
    ///     representing the handler services to be added.
    /// </param>
    /// <param name="logger">An optional <see cref="ILogger" /> instance for logging debug information.</param>
    /// <returns>The number of event handlers registered.</returns>
    public static int AddEventHandlers(
        this IServiceCollection services,
        IEnumerable<ServiceDescriptor> handlerServices,
        ILogger? logger = null)
    {
        var loggerEnabled = logger != null && logger.IsEnabled(LogLevel.Debug);
        var genericType = typeof(IEventHandler<>);
        var handlers = handlerServices
            .Where(t => genericType == t.ServiceType.GetGenericTypeDefinition())
            .ToList();

        for (var index = 0; index < handlers.Count; index++)
        {
            var handler = handlers[index];
            services.Add(handler);
            if (loggerEnabled)
                logger!.LogDebug("Registered event handler[{Count}]: {HandlerName}",
                    index, handler.ImplementationType?.FullName);
        }

        return handlers.Count;
    }

    /// <summary>
    ///     Register query handlers to the <see cref="IServiceCollection" />
    ///     from the provided collection of <see cref="ServiceDescriptor" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the query handlers will be added.</param>
    /// <param name="handlerServices">
    ///     A collection of <see cref="ServiceDescriptor" />
    ///     representing the handler services to be added.
    /// </param>
    /// <param name="logger">An optional <see cref="ILogger" /> instance for logging debug information.</param>
    /// <returns>The number of query handlers registered.</returns>
    public static int AddQueryHandlers(
        this IServiceCollection services,
        IEnumerable<ServiceDescriptor> handlerServices,
        ILogger? logger = null)
    {
        var loggerEnabled = logger != null && logger.IsEnabled(LogLevel.Debug);
        var genericTypes = new List<Type>
        {
            typeof(IQueryHandler<,>)
        };

        var handlers = handlerServices
            .Where(t => genericTypes.Exists(
                x => x == t.ServiceType.GetGenericTypeDefinition()))
            .ToList();

        for (var index = 0; index < handlers.Count; index++)
        {
            var handler = handlers[index];
            services.Add(handler);
            if (loggerEnabled)
                logger!.LogDebug("Registered query handler[{Count}]: {HandlerName}",
                    index, handler.ImplementationType?.FullName);
        }

        return handlers.Count;
    }

    /// <summary>
    ///     Register all handlers from the assembly of the specified type.
    /// </summary>
    public static IServiceCollection AddGenericTypes(
        this IServiceCollection services,
        IEnumerable<Assembly> assembliesSource,
        ServiceLifetime lifetime,
        params Type[] genericTypes)
    {
        var handlers = assembliesSource
            .Distinct()
            .SelectMany(t => t.GetTypes())
            .Where(t => t is { IsInterface: false, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces(), (implementationType, serviceType)
                => new ServiceDescriptor(serviceType, implementationType, lifetime))
            .Where(t => t.ServiceType.IsGenericType)
            .Where(t => Array.Exists(genericTypes,
                x => x == t.ServiceType.GetGenericTypeDefinition()));

        foreach (var handler in handlers)
            services.Add(handler);

        return services;
    }

    #region [ Commands ]

    /// <summary>
    ///     Tries to register command handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the command handlers will be added.</param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the command handlers will be registered.
    /// </param>
    public static IServiceCollection AddCommandHandlers(
        this IServiceCollection services,
        params Type[] types)
    {
        return AddCommandHandlers(services, ServiceLifetime.Transient, types);
    }

    /// <summary>
    ///     Tries to register command handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the command handlers will be added.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the command handlers will be registered.
    /// </param>
    public static IServiceCollection AddCommandHandlers(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        params Type[] types)
    {
        services.AddGenericTypes(
            types.Select(t => t.Assembly),
            lifetime,
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
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public static async Task ExecuteAsync<TCommand>(
        this TCommand command,
        CancellationToken ct = default) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        await command.ExecuteAsync(EventSourcingFactory.Provider, ct);
    }

    /// <summary>
    ///     Executes a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     The type of the command to be executed, which must implement <see cref="ICommand" />.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <exception cref="ArgumentNullException" />
    public static void Execute<TCommand>(this TCommand command) where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
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
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this ICommand<TResult> command,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await command.ExecuteAsync(EventSourcingFactory.Provider, ct);
    }

    /// <summary>
    ///     Executes a command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <exception cref="ArgumentNullException" />
    public static void Execute<TResult>(this ICommand<TResult> command)
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
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this IConcurrentCommand<TResult> command,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        return await ExecuteAsync(command, EventSourcingFactory.Provider, ct);
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
    /// <exception cref="ArgumentNullException" />
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

        List<dynamic> handlers = serviceProvider.GetServices(handlerType)
            .Where(handler => handler!.GetType().IsPublic)
            .ToList()!;

        if (handlers.Count == 0)
            throw new InvalidOperationException(
                $"Handler for concurrent command type {type.Name} not registered.");

        var tasks = handlers
            .Select(handler => (Task<TResult>)ConcurrentCommandBus
                .ExecuteAsync(type, command, handler, ct))
            .ToList();

        var firstCompletedTask = await Task.WhenAny(tasks);

        await Task.WhenAll(tasks);

        return await firstCompletedTask;
    }

    /// <summary>
    ///     Executes a concurrent command that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the command execution.
    /// </typeparam>
    /// <param name="command">The concurrent command to be executed.</param>
    /// <exception cref="ArgumentNullException" />
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
    /// <exception cref="ArgumentNullException" />
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
    ///     Tries to register event handlers in the dependency injection container.
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
        return AddEventHandlers(services, ServiceLifetime.Transient, types);
    }

    /// <summary>
    ///     Tries to register event handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the command handlers will be added.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the command handlers will be registered.
    /// </param>
    public static IServiceCollection AddEventHandlers(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        params Type[] types)
    {
        return services.AddGenericTypes(
            types.Select(t => t.Assembly),
            lifetime,
            typeof(IEventHandler<>)
        );
    }

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public static async Task PublishAsync<TEvent>(this TEvent eventModel) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(eventModel);
        await eventModel.PublishAsync(EventSourcingFactory.Provider);
    }

    /// <summary>
    ///     Publishes an event to all subscribed handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="eventModel">The event model.</param>
    /// <exception cref="ArgumentNullException" />
    public static void Publish<TEvent>(this TEvent eventModel) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(eventModel);
        Task.Run(async () => { await PublishAsync(eventModel); });
    }

    #endregion

    #region [ Queries ]

    /// <summary>
    ///     Tries to register query handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the query handlers will be added.</param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the query handlers will be registered.
    /// </param>
    public static IServiceCollection AddQueryHandlers(
        this IServiceCollection services,
        params Type[] types)
    {
        return AddQueryHandlers(services, ServiceLifetime.Transient, types);
    }

    /// <summary>
    ///     Tries to register query handlers in the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the query handlers will be added.</param>
    /// <param name="lifetime">The lifetime.</param>
    /// <param name="types">
    ///     An array of <see cref="Type" /> objects representing the types from whose
    ///     assemblies the query handlers will be registered.
    /// </param>
    public static IServiceCollection AddQueryHandlers(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        params Type[] types)
    {
        services.AddGenericTypes(
            types.Select(t => t.Assembly),
            lifetime,
            typeof(IQueryHandler<,>)
        );

        return services;
    }

    /// <summary>
    ///     Executes a query that returns a result.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the result returned by the query execution.
    /// </typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="ct">Optional <see cref="CancellationToken" /> to cancel the execution</param>
    /// <exception cref="ArgumentNullException" />
    /// <exception cref="InvalidOperationException" />
    public static async Task<TResult> ExecuteAsync<TResult>(
        this IQuery<TResult> query,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        return await query.ExecuteAsync(EventSourcingFactory.Provider, ct);
    }

    #endregion
}