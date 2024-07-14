using EventSourcing.Queries;

namespace EventSourcing.Tests.Unit.Queries.Stubs;

internal sealed class NonPublicQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> HandleAsync(TestQuery query, CancellationToken ct = default)
    {
        return Task.FromResult("TestResult");
    }
}