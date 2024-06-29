using EventSourcing.Commands;
using EventSourcing.Commands.Extensions;
using EventSourcing.Tests.Unit.Commands.Stubs;

namespace EventSourcing.Tests.Unit.Commands;

public class DiCommandsTests
{
    #region [ ExecuteAsync ]

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Registered_Command()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<ICommandHandler<SampleCommand>>();
        var command = new SampleCommand();

        mockHandler
            .Setup(e =>
                e.HandleAsync(It.IsAny<SampleCommand>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.CompletedTask);

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(ICommandHandler<SampleCommand>)))
            .Returns(mockHandler.Object);

        // Act
        await command.ExecuteAsync(mockServiceProvider.Object);

        // Assert
        mockHandler.Verify(handler =>
            handler.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_Command_Is_Null()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        SampleCommand command = default!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            command.ExecuteAsync(mockServiceProvider.Object));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_ServiceProvider_Is_Null()
    {
        // Arrange
        IServiceProvider serviceProvider = default!;
        SampleCommand command = default!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            command.ExecuteAsync(serviceProvider));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_InvalidOperationException_When_Handler_Is_Not_Registered()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var command = new SampleCommand();
        ICommandHandler<SampleCommand> handler = default!;

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(ICommandHandler<SampleCommand>)))
            .Returns(handler);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            command.ExecuteAsync(mockServiceProvider.Object));
    }

    #endregion

    #region [ Execute ]

    [Fact]
    public async Task Execute_Should_Execute_Registered_Command()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<ICommandHandler<SampleCommand>>();
        var command = new SampleCommand();

        mockHandler
            .Setup(e =>
                e.HandleAsync(It.IsAny<SampleCommand>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.CompletedTask);

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(ICommandHandler<SampleCommand>)))
            .Returns(mockHandler.Object);

        // Act
        command.Execute(mockServiceProvider.Object);

        await Task.Delay(200);

        // Assert
        mockHandler.Verify(handler =>
            handler.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_Throw_ArgumentNullException_When_Command_Is_Null()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        SampleCommand command = default!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            command.Execute(mockServiceProvider.Object));
    }

    [Fact]
    public void Execute_Should_Throw_ArgumentNullException_When_ServiceProvider_Is_Null()
    {
        // Arrange
        IServiceProvider serviceProvider = default!;
        SampleCommand command = default!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            command.Execute(serviceProvider));
    }

    #endregion
}