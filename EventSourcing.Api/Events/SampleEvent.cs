using EventSourcing.Events;

namespace EventSourcing.Api.Events;

public record SampleEvent(int Number) : IEvent;