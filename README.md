# EventSourcing

[![NuGet](https://img.shields.io/nuget/v/EventSourcing.svg)](https://www.nuget.org/packages/EventSourcing/)

## Overview
The EventSourcing library for .NET provides a robust, flexible, and efficient framework for implementing event sourcing in your .NET applications. This library facilitates capturing changes to an application's state as a series of events, allowing for easy auditing, debugging, and replaying of events to restore state.

## Features
- **Event Bus**: Efficiently publish and subscribe to events using an in-memory event bus with support for dependency injection (DI).
- **Command Bus**: Simplify command handling with a powerful command bus that supports both synchronous and asynchronous command execution.
- **Concurrency Control**: Manage concurrent command executions with built-in concurrency handling mechanisms to ensure data integrity.
- **Extensibility**: Easily extend and customize the framework to fit your specific needs, with interfaces and abstractions that promote clean architecture principles.
- **Dependency Injection**: Seamlessly integrate with the .NET DI container to resolve and inject event and command handlers.

## Getting Started

### Installation
Add the EventSourcing library to your project via NuGet:
```bash
dotnet add package EventSourcing
```

### Configuration
Configure your services to use the EventSourcing library. This typically involves setting up the dependency injection (DI) container in your `Startup.cs` or `Program.cs`:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add the EventSourcing services
    services.AddEventSourcing();
}
```

### Defining Events and Commands
Define your custom events and commands by implementing the respective interfaces:
```csharp
public class UserRegisteredEvent : IEvent 
{
    public string UserId { get; set; }
    public DateTime RegisteredAt { get; set; }
}

public class RegisterUserCommand : ICommand 
{
    public string UserName { get; set; }
    public string Email { get; set; }
}
```

### Handling Events and Commands
Implement handlers for your events and commands:
```csharp
public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
{
    public async Task HandleAsync(UserRegisteredEvent eventModel, CancellationToken ct = default)
    {
        // Handle the event
    }
}

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    public async Task HandleAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        // Handle the command
    }
}
```

### Publishing Events
Publish events using the event bus:
```csharp
var eventBus = serviceProvider.GetRequiredService<IEventBus>();
await eventBus.PublishAsync(new UserRegisteredEvent 
{
    UserId = "12345",
    RegisteredAt = DateTime.UtcNow
});
```

### Executing Commands
Execute commands using the command bus:
```csharp
var commandBus = serviceProvider.GetRequiredService<ICommandBus>();
await commandBus.ExecuteAsync(new RegisterUserCommand 
{
    UserName = "JohnDoe",
    Email = "john.doe@example.com"
});
```

### Documentation
For detailed documentation, examples, and API references, please visit my [GitHub repository](https://github.com/covali-pavel-developer/EventSourcing).

### Contributing
I welcome contributions from the community! Please read my contributing guidelines on GitHub to get started.

### License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/covali-pavel-developer/EventSourcing/blob/main/LICENSE.txt) file for more information.
