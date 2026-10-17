/*
name: Task Extensions
description: Helper extensions for safer Task operations
tags: core, extensions, async
*/
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Helper extensions for running actions/tasks safely with cancellation and timeout support.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Default timeout used by the <c>RunSafely*</c> helpers (30 seconds).
    /// </summary>
    public const int DefaultTimeoutMs = 30000;

    /// <summary>
    /// Runs an action on the thread pool with a linked cancellation token and an optional timeout.
    /// Cancellations (manual or timeout) are swallowed; unexpected exceptions fault the returned task
    /// so they can be observed/awaited by the caller instead of silently disappearing.
    /// </summary>
    /// <param name="action">The action to run.</param>
    /// <param name="cancellationToken">Cancellation token (optional).</param>
    /// <param name="timeoutMs">Timeout in milliseconds. Use 0 or a negative value to disable the timeout.</param>
    /// <returns>The running task.</returns>
    public static Task RunSafely(
        Action action,
        CancellationToken cancellationToken = default,
        int timeoutMs = DefaultTimeoutMs
    )
    {
        ArgumentNullException.ThrowIfNull(action);

        return Task.Run(
            () =>
            {
                using var cts = CreateLinkedSource(cancellationToken, timeoutMs);
                try
                {
                    // Only meaningful for synchronous work: cooperative checks inside the action
                    // must observe cts.Token for this to abort early.
                    cts.Token.ThrowIfCancellationRequested();
                    action();
                }
                catch (OperationCanceledException) when (IsExpectedCancellation(cts, cancellationToken))
                {
                    // Expected cancellation (caller cancelled or timeout elapsed) - no action needed.
                }
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Runs an action on the thread pool with a linked cancellation token and an optional timeout,
    /// returning a result. Same semantics as <see cref="RunSafely(Action, CancellationToken, int)"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="func">The function to run.</param>
    /// <param name="cancellationToken">Cancellation token (optional).</param>
    /// <param name="timeoutMs">Timeout in milliseconds. Use 0 or a negative value to disable the timeout.</param>
    /// <returns>A task that completes with the function result, or is canceled/timed out.</returns>
    public static Task<TResult> RunSafely<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken = default,
        int timeoutMs = DefaultTimeoutMs
    )
    {
        ArgumentNullException.ThrowIfNull(func);

        return Task.Run(
            () =>
            {
                using var cts = CreateLinkedSource(cancellationToken, timeoutMs);
                try
                {
                    cts.Token.ThrowIfCancellationRequested();
                    return func();
                }
                catch (OperationCanceledException) when (IsExpectedCancellation(cts, cancellationToken))
                {
                    // Re-throw so the returned task ends up in the Canceled state (awaitable with OCE).
                    throw;
                }
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Runs an async action with a linked cancellation token and an optional timeout.
    /// The action receives the linked token and should honor it for the timeout to take effect.
    /// Cancellations (manual or timeout) are swallowed; unexpected exceptions fault the returned task.
    /// </summary>
    /// <param name="asyncAction">The async action to run.</param>
    /// <param name="cancellationToken">Cancellation token (optional).</param>
    /// <param name="timeoutMs">Timeout in milliseconds. Use 0 or a negative value to disable the timeout.</param>
    /// <returns>The running task.</returns>
    public static Task RunSafelyAsync(
        Func<CancellationToken, Task> asyncAction,
        CancellationToken cancellationToken = default,
        int timeoutMs = DefaultTimeoutMs
    )
    {
        ArgumentNullException.ThrowIfNull(asyncAction);

        return Task.Run(
            async () =>
            {
                using var cts = CreateLinkedSource(cancellationToken, timeoutMs);
                try
                {
                    await asyncAction(cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (IsExpectedCancellation(cts, cancellationToken))
                {
                    // Expected cancellation (caller cancelled or timeout elapsed) - no action needed.
                }
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Runs an async function with a linked cancellation token and an optional timeout, returning a result.
    /// Same semantics as <see cref="RunSafelyAsync(Func{CancellationToken, Task}, CancellationToken, int)"/>,
    /// except a cancellation/timeout leaves the returned task in the Canceled state.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="asyncFunc">The async function to run.</param>
    /// <param name="cancellationToken">Cancellation token (optional).</param>
    /// <param name="timeoutMs">Timeout in milliseconds. Use 0 or a negative value to disable the timeout.</param>
    /// <returns>A task that completes with the function result, or is canceled/timed out.</returns>
    public static Task<TResult> RunSafelyAsync<TResult>(
        Func<CancellationToken, Task<TResult>> asyncFunc,
        CancellationToken cancellationToken = default,
        int timeoutMs = DefaultTimeoutMs
    )
    {
        ArgumentNullException.ThrowIfNull(asyncFunc);

        return Task.Run(
            async () =>
            {
                using var cts = CreateLinkedSource(cancellationToken, timeoutMs);
                try
                {
                    return await asyncFunc(cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (IsExpectedCancellation(cts, cancellationToken))
                {
                    // Re-throw so the returned task ends up in the Canceled state (awaitable with OCE).
                    throw;
                }
            },
            cancellationToken
        );
    }

    /// <summary>
    /// Waits for the task to complete within the given timeout without blocking a thread-pool thread.
    /// </summary>
    /// <param name="task">The task to wait on.</param>
    /// <param name="timeoutMs">Timeout in milliseconds. Values &lt;= 0 mean "do not wait".</param>
    /// <returns><c>true</c> if the task completed before the timeout elapsed; <c>false</c> otherwise.</returns>
    public static async Task<bool> WaitWithTimeoutAsync(this Task task, int timeoutMs)
    {
        ArgumentNullException.ThrowIfNull(task);

        if (task.IsCompleted)
            return true;

        if (timeoutMs <= 0)
            return false;

        using var delay = Task.Delay(timeoutMs);
        var completed = await Task.WhenAny(task, delay).ConfigureAwait(false);
        if (completed != task)
        {
            // Keep the original task's exceptions observed even after we give up waiting.
            _ = task.ContinueWith(
                t => _ = t.Exception,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            );
            return false;
        }

        // Propagate faults to the caller instead of reporting "completed" for a failed task.
        await task.ConfigureAwait(false);
        return true;
    }

    private static CancellationTokenSource CreateLinkedSource(CancellationToken cancellationToken, int timeoutMs)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (timeoutMs > 0)
            cts.CancelAfter(timeoutMs);
        return cts;
    }

    private static bool IsExpectedCancellation(CancellationTokenSource cts, CancellationToken cancellationToken)
    {
        // Expected when the caller explicitly cancelled, or when only the internal timeout fired.
        return cts.IsCancellationRequested;
    }
}
