using EventSourcing;
using EventSourcing.Api.Commands;
using EventSourcing.Api.Events;
using EventSourcing.Api.Models;
using EventSourcing.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEventSourcing(typeof(SampleCommand));

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/commands/sample", async (int number, CancellationToken ct) =>
    {
        return await new SampleCommand(number).ExecuteAsync(ct);
    })
    .WithName("SampleCommand")
    .WithOpenApi();

app.MapGet("/commands/concurrent", async (int startNumber, int count, CancellationToken ct) =>
    {
        var tasks = new List<Task<BaseResult>>();

        for (var i = 0; i < count; i++)
        {
            tasks.Add(new ConcurrentCommand(startNumber++).ExecuteAsync(ct));
        }

        return await Task.WhenAll(tasks);
    })
    .WithName("ConcurrentCommand")
    .WithOpenApi();

app.MapGet("/events/sample", async (int number) =>
    {
        await new SampleEvent(number).PublishAsync();
        return number;
    })
    .WithName("SampleEvent")
    .WithOpenApi();

app.Run();