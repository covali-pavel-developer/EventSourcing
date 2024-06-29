# EventSourcing
[![NuGet](https://img.shields.io/nuget/v/EventSourcing.Extensions)](https://www.nuget.org/packages/EventSourcing.Extensions)

## Overview
The `EventSourcing` library for .NET provides a robust, flexible, and efficient framework for implementing event sourcing in your .NET applications. This library facilitates capturing changes to an application's state as a series of events, allowing for easy auditing, debugging, and replaying of events to restore state.

## Features
- **Event Bus**: Efficiently publish and subscribe to events using an in-memory event bus with support for dependency injection (DI).
- **Command Bus**: Simplify command handling with a powerful command bus that supports both synchronous and asynchronous command execution.
- **Concurrency Control**: Manage concurrent command executions with built-in concurrency handling mechanisms to ensure data integrity.
- **Extensibility**: Easily extend and customize the framework to fit your specific needs, with interfaces and abstractions that promote clean architecture principles.
- **Dependency Injection**: Seamlessly integrate with the .NET dependency injection (DI) container to resolve and inject event and command handlers.

### Dependency Injection
The `EventSourcingExtensions` class provides extension methods designed for configuring event sourcing within a .NET application. It facilitates the registration of event and command handlers into the dependency injection (DI) container. This is achieved by specifying an array of Type objects representing the types from which event or command handlers should be registered. This ensures that handlers are correctly resolved and utilized throughout the application.

Registration of handlers are categorized into:
- Command handlers: `ICommandHandler<>` and `ICommandHandler<,>`
- Concurrent command handlers: `IConcurrentCommandHandler<,>`
- Event handlers: `IEventHandler<>`

All identified handlers are registered as transient services in the DI container using the `TryAddTransient` method, which prevents duplicate registrations.

### EventBus
The `EventBus` class supports both manual subscription of handlers for fine-grained control and automated registration via dependency injection (DI) for convenience and flexibility. This makes it suitable for implementing event-driven architectures and facilitating communication between different components of your application.

**Features**
- **Manual Subscription**:
You can manually subscribe event handlers `IEventHandler<TEvent>` to specific generic event types `TEvent` using the `Subscribe` method. This allows precise control over which handlers receive which events.

- **Dependency Injection (DI) Support**:
Alternatively, handlers can be automatically registered using the `AddEventSourcing` method from `EventSourcingExtensions`.

**Key Points**
- **Concurrency**: Utilizes `ConcurrentDictionary` for thread-safe handling of event handlers.
- **Error Handling**: Throws exceptions when attempting to publish an event without any registered handlers.
- **Synchronously publishes**: An event by invoking `Publish` on a background task.
- **Asynchronous Handling**: Supports asynchronous event handling via `PublishAsync`.

### CommandBus
The `CommandBus` class facilitates the execution of commands within your application, supporting both simple and result-returning commands. It is designed to be flexible and extendable, allowing developers to easily integrate and manage command handling logic. Class supports both manual subscription of handlers for fine-grained control and automated registration via dependency injection (DI) for convenience and flexibility. 

**Features**
- **Manual Subscription**:
You can manually subscribe command handlers `ICommandHandler<TCommand>` to specific generic command types `TCommand` using the `Subscribe` method. This allows precise control over which handlers receive which commands.

- **Dependency Injection (DI) Support**:
Alternatively, handlers can be automatically registered using the `AddEventSourcing` method from `EventSourcingExtensions`.

**Key Points**
- **Concurrency**: Utilizes `ConcurrentDictionary` for thread-safe handling of command handlers.
- **Error Handling**: Throws exceptions when attempting to execute a command without a registered handler or if the handler type does not match.
- **Synchronously executes**: An command by invoking `Execute` on a background task.
- **Asynchronousle executes**: Supports asynchronous command handling via `ExecuteAsync`.

## Getting Started
### Installation
Add the EventSourcing library to your project via NuGet:
```bash
dotnet add package EventSourcing
```

### Usage
#### EventBus Setup
To set up the EventBus for managing events, you can use the following approach:

Define your custom events by implementing the respective interfaces:
```csharp
public class UserRegisteredEvent : IEvent 
{
    public string UserId { get; set; }
    public DateTime RegisteredAt { get; set; }
}
```

Implement handlers for your events:
```csharp
public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    public async Task HandleAsync(UserRegisteredEvent eventModel, CancellationToken ct = default)
    {
        // Handle the event
    }
}
```

Manually subscribe event handlers:
```csharp
var eventBus = new EventBus();
eventBus.Subscribe(new UserRegisteredEvent());
await eventBus.PublishAsync(new UserRegisteredEvent());
```

Alternatively, use DI for automatic registration in your `Program.cs`:
```csharp
services.AddEventSourcing(typeof(UserRegisteredEvent));
await new UserRegisteredEvent().PublishAsync();
```

Alternatively, register IEventBus to yor DI container:
```csharp
services.AddSingleton<IEventBus, EventBus>();
```

Now publish events using the event bus:
```csharp
var eventBus = serviceProvider.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new UserRegisteredEvent 
{
    UserId = "12345",
    RegisteredAt = DateTime.UtcNow
});
```

#### CommandBus Setup
To configure the CommandBus for executing commands, follow these steps:

Define your custom commands by implementing the respective interfaces:
```csharp
public class RegisterUserCommand : ICommand 
{
    public string UserName { get; set; }
    public string Email { get; set; }
}

public class DeleteUserCommand : ICommand<bool>
{
    public string Id { get; set; }
}
```

Implement handlers for your commands:
```csharp
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    public async Task HandleAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        // Handle the command
    }
}

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteUserCommand command, CancellationToken ct = default)
    {
        // Handle the command
    }
}
```

Manually subscribe commands handlers:
```csharp
var commandBus = new CommandBus();
commandBus.Subscribe(new RegisterUserCommandHandler());
commandBus.Subscribe(new DeleteUserCommandHandler());
await commandBus.ExecuteAsync(new RegisterUserCommand());
```

Alternatively, use DI for automatic registration in your `Program.cs`:
```csharp
services.AddEventSourcing(typeof(RegisterUserCommand));
await new RegisterUserCommand().ExecuteAsync();
```

Alternatively, register ICommandBus to yor DI container:
```csharp
services.AddSingleton<ICommandBus, CommandBus>();
```

Now publish events using the event bus:
```csharp
var commandBus = serviceProvider.GetRequiredService<ICommandBus>();
await commandBus.ExecuteAsync(new RegisterUserCommand 
{
    UserName = "JohnDoe",
    Email = "john.doe@example.com"
});
```

### Contributing
I welcome contributions from the community! Please read my contributing guidelines on GitHub to get started.

### License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/covali-pavel-developer/EventSourcing/blob/main/LICENSE.txt) file for more information.
