using EventSourcing.Commands;
using EventSourcing.Commands.Extensions;
using EventSourcing.Tests.Unit.Commands.Stubs;

namespace EventSourcing.Tests.Unit.Commands;

public class DiCommandWithResultTests
{
    #region [ ExecuteAsync ]

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Registered_Command_And_Return_Result()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<ICommandHandler<CommandWithResult, SampleResult>>();
        var expectedResult = new SampleResult();
        var command = new CommandWithResult();

        mockHandler
            .Setup(e =>
                e.HandleAsync(It.IsAny<CommandWithResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(ICommandHandler<CommandWithResult, SampleResult>)))
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
        CommandWithResult command = default!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            command.ExecuteAsync(mockServiceProvider.Object));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_ServiceProvider_Is_Null()
    {
        // Arrange
        IServiceProvider serviceProvider = default!;
        CommandWithResult command = default!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            command.ExecuteAsync(serviceProvider));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_InvalidOperationException_When_Handler_Is_Not_Registered()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var command = new CommandWithResult();
        ICommandHandler<CommandWithResult, SampleResult> handler = default!;

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(ICommandHandler<CommandWithResult, SampleResult>)))
            .Returns(handler);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            command.ExecuteAsync(mockServiceProvider.Object));
    }

    #endregion

    #region [ Execute ]

    [Fact]
    public async Task Execute_Should_Execute_Registered_Command_And_Return_Result()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockHandler = new Mock<ICommandHandler<CommandWithResult, SampleResult>>();
        var expectedResult = new SampleResult();
        var command = new CommandWithResult();

        mockHandler
            .Setup(e =>
                e.HandleAsync(It.IsAny<CommandWithResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        mockServiceProvider
            .Setup(e =>
                e.GetService(typeof(ICommandHandler<CommandWithResult, SampleResult>)))
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
        CommandWithResult command = default!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            command.Execute(mockServiceProvider.Object));
    }

    [Fact]
    public void Execute_Should_Throw_ArgumentNullException_When_ServiceProvider_Is_Null()
    {
        // Arrange
        IServiceProvider serviceProvider = default!;
        CommandWithResult command = default!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            command.Execute(serviceProvider));
    }

    #endregion
}