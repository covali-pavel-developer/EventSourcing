using System.Collections.Concurrent;
using System.Reflection;
using EventSourcing.Commands.Concurrent;
using EventSourcing.Commands.Concurrent.Internal;
using EventSourcing.Tests.Unit.Commands.Stubs;

namespace EventSourcing.Tests.Unit.Commands.Concurrent;

public class DiConcurrentCommandTests
{
    #region [ ExecuteAsync ]

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Registered_Command_And_Return_Result()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>>();
        var expectedResult = new SampleResult();
        var command = new ConcurrentSampleCommand();

        mockHandler
            .Setup(e =>
                e.HandleAsync(It.IsAny<ConcurrentSampleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>)))
            .Returns(mockHandler.Object);

        // Act
        var result = await command.ExecuteAsync(mockServiceProvider.Object);

        // Assert
        mockHandler.Verify(e =>
            e.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_Command_Is_Null()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        ConcurrentSampleCommand command = default!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            command.ExecuteAsync(mockServiceProvider.Object));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_ServiceProvider_Is_Null()
    {
        // Arrange
        IServiceProvider serviceProvider = default!;
        ConcurrentSampleCommand command = default!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            command.ExecuteAsync(serviceProvider));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_InvalidOperationException_When_Handler_Is_Not_Registered()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var command = new ConcurrentSampleCommand();
        IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult> handler = default!;

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>)))
            .Returns(handler);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            command.ExecuteAsync(mockServiceProvider.Object));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Enforce_Concurrency_Limit()
    {
        // Arrange
        const int expectedConcurrentCount = 2;

        var command = new ConcurrentSampleCommand();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>>();

        mockHandler
            .Setup(e => e.ConcurrentCount)
            .Returns(expectedConcurrentCount);

        mockHandler.Setup(e =>
                e.HandleAsync(It.IsAny<ConcurrentSampleCommand>(),
                    It.IsAny<CancellationToken>()))
            .Returns(async (ConcurrentSampleCommand _, CancellationToken ct) =>
            {
                await Task.Delay(1000, ct);
                return new SampleResult();
            });

        mockServiceProvider
            .Setup(e => e.GetService(
                typeof(IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>)))
            .Returns(mockHandler.Object);

        // Act
        var tasks = new[]
        {
            command.ExecuteAsync(mockServiceProvider.Object),
            command.ExecuteAsync(mockServiceProvider.Object),
            command.ExecuteAsync(mockServiceProvider.Object)
        };

        await Task.Delay(100);

        // Assert
        var concurrentHandlerField = typeof(ConcurrentCommandBus)
            .GetField("_handlers",
                BindingFlags.NonPublic
                | BindingFlags.Instance);

        Assert.NotNull(concurrentHandlerField);

        var handlers = (ConcurrentDictionary<string, ConcurrentHandler>?)concurrentHandlerField
            .GetValue(EventSourcingExtensions.ConcurrentCommandBus);

        Assert.NotNull(handlers);

        var semaphore = handlers[nameof(ConcurrentSampleCommand)].Semaphore;

        Assert.Equal(0, semaphore.CurrentCount);
        await Task.WhenAll(tasks);
    }

    #endregion

    #region [ Execute ]

    [Fact]
    public async Task Execute_Should_Execute_Registered_Command_And_Return_Result()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>>();
        var expectedResult = new SampleResult();
        var command = new ConcurrentSampleCommand();

        mockHandler
            .Setup(e =>
                e.HandleAsync(It.IsAny<ConcurrentSampleCommand>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(IConcurrentCommandHandler<ConcurrentSampleCommand, SampleResult>)))
            .Returns(mockHandler.Object);

        // Act
        command.Execute(mockServiceProvider.Object);

        await Task.Delay(100);

        // Assert
        mockHandler.Verify(e =>
            e.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_Throw_ArgumentNullException_When_Command_Is_Null()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        ConcurrentSampleCommand command = default!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            command.Execute(mockServiceProvider.Object));
    }

    [Fact]
    public void Execute_Should_Throw_ArgumentNullException_When_ServiceProvider_Is_Null()
    {
        // Arrange
        IServiceProvider serviceProvider = default!;
        ConcurrentSampleCommand command = default!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            command.Execute(serviceProvider));
    }

    #endregion
}