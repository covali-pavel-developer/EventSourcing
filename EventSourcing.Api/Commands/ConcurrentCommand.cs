﻿using EventSourcing.Api.Models;
using EventSourcing.Commands.Concurrent;

namespace EventSourcing.Api.Commands;

public record ConcurrentCommand(int Number) : IConcurrentCommand<BaseResult>;

public sealed class ConcurrentCommandHandler(ILogger<ConcurrentCommandHandler> logger)
    : IConcurrentCommandHandler<ConcurrentCommand, BaseResult>
{
    public int ConcurrentCount { get; init; } = 1;

    public async Task<BaseResult> HandleAsync(ConcurrentCommand command, CancellationToken ct = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        var message = $"{nameof(ConcurrentCommandHandler)} with number: {command.Number} handled at {DateTime.Now:HH:mm:ss.fff}";
        logger.LogInformation(message);
        return new BaseResult(command.Number, message);
    }
}

public sealed class AnotherConcurrentCommandHandler(ILogger<ConcurrentCommandHandler> logger)
    : IConcurrentCommandHandler<ConcurrentCommand, BaseResult>
{
    public int ConcurrentCount { get; init; } = 2;

    public async Task<BaseResult> HandleAsync(ConcurrentCommand command, CancellationToken ct = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        var message = $"{nameof(AnotherConcurrentCommandHandler)} with number: {command.Number} handled at {DateTime.Now:HH:mm:ss.fff}";
        logger.LogInformation(message);
        return new BaseResult(command.Number, message);
    }
}