using EventSourcing.Events;

namespace EventSourcing.Tests.Unit.Events.Stubs;

public class SampleEventHandler : IEventHandler<SampleEvent>
{
    public int InvokesCount { get; set; }

    public Task HandleAsync(SampleEvent eventModel)
    {
        InvokesCount++;
        return Task.CompletedTask;
    }
}