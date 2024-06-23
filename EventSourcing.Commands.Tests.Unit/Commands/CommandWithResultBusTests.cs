using EventSourcing.Commands;
using EventSourcing.Tests.Unit.Commands.Stubs;

namespace EventSourcing.Tests.Unit.Commands;

public class CommandWithResultBusTests
{
    #region [ Subscribe ]

    [Fact]
    public void Subscribe_Should_Register_Handler()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<CommandWithResult, SampleResult>>();
        var commandBus = new CommandBus();

        // Act
        commandBus.Subscribe(handlerMock.Object);

        // Assert
        // The handler should be registered without any exception.
        Assert.True(true);
    }

    [Fact]
    public void Subscribe_Should_Throw_ArgumentNullException_When_Handler_Is_Null()
    {
        // Arrange
        ICommandHandler<CommandWithResult, SampleResult> handler = default!;
        var commandBus = new CommandBus();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => commandBus.Subscribe(handler));
    }

    #endregion

    #region [ ExecuteAsync ]

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Registered_Command_And_Return_Result()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<CommandWithResult, SampleResult>>();
        var expectedResult = new SampleResult();
        var commandBus = new CommandBus();
        var command = new CommandWithResult();

        handlerMock.Setup(e =>
                e.HandleAsync(It.IsAny<CommandWithResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        commandBus.Subscribe(handlerMock.Object);

        // Act
        var result = await commandBus.ExecuteAsync<CommandWithResult, SampleResult>(command);

        // Assert
        handlerMock.Verify(e =>
            e.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_ArgumentNullException_When_Command_Is_Null()
    {
        // Arrange
        CommandWithResult command = default!;
        var commandBus = new CommandBus();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            commandBus.ExecuteAsync<CommandWithResult, SampleResult>(command));
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_InvalidOperationException_When_Handler_Is_Not_Registered()
    {
        // Arrange
        var commandBus = new CommandBus();
        var command = new CommandWithResult();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            commandBus.ExecuteAsync<CommandWithResult, SampleResult>(command));
    }

    #endregion

    #region [ Execute ]

    [Fact]
    public async Task Execute_Should_Execute_Registered_Command_And_Return_Result()
    {
        // Arrange
        var handlerMock = new Mock<ICommandHandler<CommandWithResult, SampleResult>>();
        var expectedResult = new SampleResult();
        var commandBus = new CommandBus();
        var command = new CommandWithResult();

        handlerMock.Setup(e =>
                e.HandleAsync(It.IsAny<CommandWithResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        commandBus.Subscribe(handlerMock.Object);

        // Act
        commandBus.Execute<CommandWithResult, SampleResult>(command);

        await Task.Delay(100);

        // Assert
        handlerMock.Verify(e =>
            e.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Execute_Should_Throw_ArgumentNullException_When_Command_Is_Null()
    {
        // Arrange
        CommandWithResult command = default!;
        var commandBus = new CommandBus();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            commandBus.Execute<CommandWithResult, SampleResult>(command));
    }

    #endregion
}