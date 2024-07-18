using Microsoft.Extensions.Logging;

namespace EventSourcing.Extensions;

public static class EventSourcingFactory
{
    /// <summary>
    ///     Gets the service provider.
    /// </summary>
    public static IServiceProvider Provider { get; internal set; }

    /// <summary>
    ///     Gets the logger.
    /// </summary>
    public static ILogger Logger { get; internal set; }
}