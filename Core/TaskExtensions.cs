/*
name: Task Extensions
description: Helper extensions for safer Task operations
tags: core, extensions, async
*/
using System;
using System.Threading;
using System.Threading.Tasks;

public static class TaskExtensions
{
    /// <summary>
    /// Runs a task with automatic cancellation token and timeout
    /// </summary>
    /// <param name="action">The action to run</param>
    /// <param name="cancellationToken">Cancellation token (optional)</param>
    /// <param name="timeoutMs">Timeout in milliseconds (default: 30 seconds)</param>
    /// <returns>The running task</returns>
    public static Task RunSafely(Action action, CancellationToken cancellationToken = default, int timeoutMs = 30000)
    {
        return Task.Run(() =>
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                if (timeoutMs > 0)
                    cts.CancelAfter(timeoutMs);
                
                cts.Token.ThrowIfCancellationRequested();
                action();
            }
            catch (OperationCanceledException)
            {
                // Expected cancellation - no action needed
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Runs an async task with automatic cancellation token and timeout
    /// </summary>
    /// <param name="asyncAction">The async action to run</param>
    /// <param name="cancellationToken">Cancellation token (optional)</param>
    /// <param name="timeoutMs">Timeout in milliseconds (default: 30 seconds)</param>
    /// <returns>The running task</returns>
    public static Task RunSafelyAsync(Func<CancellationToken, Task> asyncAction, CancellationToken cancellationToken = default, int timeoutMs = 30000)
    {
        return Task.Run(async () =>
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                if (timeoutMs > 0)
                    cts.CancelAfter(timeoutMs);
                
                await asyncAction(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected cancellation - no action needed
            }
        }, cancellationToken);
    }
}