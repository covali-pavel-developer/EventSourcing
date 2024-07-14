using EventSourcing.Queries;

namespace EventSourcing.Tests.Unit.Queries.Stubs;

public sealed class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> HandleAsync(TestQuery query, CancellationToken ct = default)
    {
        return Task.FromResult("TestResult");
    }
}