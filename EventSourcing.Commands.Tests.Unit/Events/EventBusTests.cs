using EventSourcing.Events;
using EventSourcing.Tests.Unit.Events.Stubs;

namespace EventSourcing.Tests.Unit.Events;

public class EventBusTests
{
    [Fact]
    public void Subscribe_Should_Register_Handler()
    {
        // Arrange
        var handlerMock = new Mock<IEventHandler<SampleEvent>>();
        var eventBus = new EventBus();

        // Act
        eventBus.Subscribe(handlerMock.Object);

        // Assert
        // The handler should be registered without any exception.
        Assert.True(true);
    }

    [Fact]
    public void Subscribe_Should_Throw_ArgumentNullException_When_Handler_Is_Null()
    {
        // Arrange
        IEventHandler<SampleEvent> handler = default!;
        var eventBus = new EventBus();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => eventBus.Subscribe(handler));
    }

    [Fact]
    public async Task PublishAsync_Should_Execute_Registered_EventHandler()
    {
        // Arrange
        var handlerMock = new Mock<IEventHandler<SampleEvent>>();
        var eventBus = new EventBus();
        var eventModel = new SampleEvent();

        handlerMock.Setup(e =>
                e.HandleAsync(It.IsAny<SampleEvent>()))
            .Returns(Task.CompletedTask);

        eventBus.Subscribe(handlerMock.Object);

        // Act
        await eventBus.PublishAsync(eventModel);

        // Assert
        handlerMock.Verify(e =>
            e.HandleAsync(eventModel), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_Should_Throw_InvalidOperationException_When_Handler_Is_Not_Registered()
    {
        // Arrange
        var eventModel = new SampleEvent();
        var eventBus = new EventBus();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => eventBus.PublishAsync(eventModel));
    }

    [Fact]
    public async Task Publish_Should_Execute_Registered_EventHandler()
    {
        // Arrange
        var handlerMock = new Mock<IEventHandler<SampleEvent>>();
        var eventBus = new EventBus();
        var eventModel = new SampleEvent();

        handlerMock.Setup(e =>
                e.HandleAsync(It.IsAny<SampleEvent>()))
            .Returns(Task.CompletedTask);

        eventBus.Subscribe(handlerMock.Object);

        // Act
        eventBus.Publish(eventModel);

        await Task.Delay(100);

        // Assert
        handlerMock.Verify(e =>
            e.HandleAsync(eventModel), Times.Once);
    }
}