using EventSourcing.Commands;
using EventSourcing.Commands.Extensions;
using EventSourcing.Tests.Unit.Commands.Stubs;

namespace EventSourcing.Tests.Unit.Commands;

public class DiCommandWithResultTests
{
    #region [ ExecuteAsync ]

    [Fact]
    public async Task ExecuteAsync_NoHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CommandWithResult();
        var serviceProviderMock = new Mock<IServiceProvider>();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ICommandHandler<CommandWithResult, SampleResult>)))
            .Returns((ICommandHandler<CommandWithResult, SampleResult>)default!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await command.ExecuteAsync(serviceProviderMock.Object));
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