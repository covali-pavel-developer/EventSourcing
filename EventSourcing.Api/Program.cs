using EventSourcing.Api.Commands;
using EventSourcing.Api.Events;
using EventSourcing.Api.Models;
using EventSourcing.Api.Queries;
using EventSourcing.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddHttpContextAccessor()
    .AddEventSourcing(typeof(SampleCommand));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region [ Commands ]

app.MapGet("/commands/execute-async", async (int number, CancellationToken ct) =>
    {
        return await new SampleCommand(number).ExecuteAsync(ct);
    })
    .WithName("ExecuteCommandAsync")
    .WithOpenApi();

app.MapGet("/commands/execute", (int number) =>
    {
        new SampleCommand(number).Execute();
    })
    .WithName("ExecuteCommand")
    .WithOpenApi();

app.MapGet("/commands/concurrent/execute-async", async (int number, int count, CancellationToken ct) =>
    {
        var tasks = new List<Task<BaseResult>>();

        for (var i = 0; i < count; i++)
        {
            tasks.Add(new ConcurrentCommand(number++).ExecuteAsync(ct));
        }

        return await Task.WhenAll(tasks);
    })
    .WithName("ExecuteConcurrentCommandAsync")
    .WithOpenApi();

app.MapGet("/commands/concurrent/execute", (int number, int count) =>
    {
        Parallel.For(0, count, (_) =>
        {
            new ConcurrentCommand(number++).Execute();
        });
    })
    .WithName("ExecuteConcurrentCommand")
    .WithOpenApi();

#endregion

#region [ Events ]

app.MapGet("/events/publish-async", async (int number) =>
    {
        await new SampleEvent(number).PublishAsync();
    })
    .WithName("PublishEventAsync")
    .WithOpenApi();

#endregion

#region [ Queries ]

app.MapGet("/queries/execute-async", async (int number) =>
    {
        return await new SampleQuery(number).ExecuteAsync();
    })
    .WithName("ExecuteQueryAsync")
    .WithOpenApi();

#endregion

app.Run();