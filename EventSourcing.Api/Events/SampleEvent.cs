using EventSourcing.Events;

namespace EventSourcing.Api.Events;

public class SampleEvent(int Number) : IEvent;