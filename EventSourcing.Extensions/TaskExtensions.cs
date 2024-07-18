using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EventSourcing.Extensions;

public static class TaskExtensions
{
    /// <summary>
    ///     Executes the specified task with a stopwatch.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
    /// <param name="type">The type associated with the task.</param>
    /// <param name="task">The task to be executed.</param>
    public static async Task<TResult> WithWatcher<TResult>(this Task<TResult> task, Type type)
    {
        return await WithWatcher(task, type.Name);
    }

    public static async Task<TResult> WithWatcher<TResult, TSource>(this Task<TResult> task)
    {
        return await WithWatcher(task, typeof(TSource).Name);
    }

    /// <summary>
    ///     Executes the specified task with a stopwatch.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
    /// <param name="type">The type associated with the task.</param>
    /// <param name="task">The task to be executed.</param>
    public static async Task<TResult> WithWatcher<TResult>(this Task<TResult> task, string name)
    {
        if (!EventSourcingFactory.Logger
                .IsEnabled(LogLevel.Debug))
            return await task;

        var sw = new Stopwatch();
        EventSourcingFactory.Logger.LogDebug("{Operation} started execution.", name);
        sw.Start();

        var result = await task;

        sw.Stop();

        EventSourcingFactory.Logger.LogDebug("{Operation} finished execution in {ElapsedMilliseconds} ms.",
            name, sw.ElapsedMilliseconds);

        return result;
    }

    /// <summary>
    ///     Executes the specified task with a stopwatch.
    /// </summary>
    /// <param name="type">The type associated with the task.</param>
    /// <param name="task">The task to be executed.</param>
    public static async Task WithWatcher(this Task task, Type type)
    {
        if (!EventSourcingFactory.Logger.IsEnabled(LogLevel.Debug))
        {
            await task;
            return;
        }

        var sw = new Stopwatch();

        EventSourcingFactory.Logger.LogDebug(
            "{Operation} started execution.", type.Name
        );

        sw.Start();
        await task;
        sw.Stop();

        EventSourcingFactory.Logger.LogDebug(
            "{Operation} finished execution in {ElapsedMilliseconds} ms.",
            type.Name, sw.ElapsedMilliseconds
        );
    }
}