using EventSourcing.Queries;

namespace EventSourcing.Tests.Unit.Queries.Stubs;

public record TestQuery : IQuery<string>;