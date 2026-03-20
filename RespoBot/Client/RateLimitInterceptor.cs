using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace RespoBot.Client;

public sealed class RateLimitInterceptor : AsyncInterceptorBase, IRateLimitMonitor, IAsyncDisposable
{
    /// <summary>
    /// Calls subtracted from <c>RateLimitRemaining</c> as a cushion to absorb
    /// clock skew or other processes sharing the same credentials.
    /// </summary>
    private const int SafetyBuffer = 2;

    private readonly ILogger<RateLimitInterceptor>? _logger;

    // Unbounded channel serialises all outgoing calls through one reader.
    private readonly Channel<QueueEntry> _channel =
        Channel.CreateUnbounded<QueueEntry>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });

    private readonly CancellationTokenSource _workerCts = new();
    private readonly Task _workerTask;

    // Volatile so reads from any thread always see the most recent snapshot.
    private volatile RateLimitState _state = new(null, null, null);
    
    public RateLimitInterceptor(ILogger<RateLimitInterceptor>? logger = null)
    {
        _logger = logger;
        _workerTask = RunWorkerAsync(_workerCts.Token);
    }
    
    public RateLimitState CurrentRateLimitState => _state;

    /// <summary>
    /// Handles the rare synchronous or plain-<see cref="Task"/> methods.
    /// </summary>
    protected override async Task InterceptAsync(
        IInvocation invocation,
        IInvocationProceedInfo proceedInfo,
        Func<IInvocation, IInvocationProceedInfo, Task> proceed)
    {
        if (invocation.Method.Name == "Dispose" && invocation.Method.ReturnType == typeof(void))
        {
            // Handle disposal logic if necessary, then simply return.
            // Do NOT call invocation.Proceed() here.
            return;
        }
        
        await EnqueueAsync(async ct =>
        {
            ct.ThrowIfCancellationRequested();
            await proceed(invocation, proceedInfo).ConfigureAwait(false);
            return true; // dummy value - unified with generic path
        }, ExtractCancellationToken(invocation)).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles every <c>Task&lt;TResult&gt;</c>-returning method.
    /// Rate-limit headers are captured from the result before it is returned.
    /// </summary>
    protected override async Task<TResult> InterceptAsync<TResult>(
        IInvocation invocation,
        IInvocationProceedInfo proceedInfo,
        Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
    {
        return await EnqueueAsync(async ct =>
        {
            ct.ThrowIfCancellationRequested();
            TResult result = await proceed(invocation, proceedInfo).ConfigureAwait(false);
            TryCaptureRateLimit(result);
            return result;
        }, ExtractCancellationToken(invocation)).ConfigureAwait(false);
    }

    private async Task<T> EnqueueAsync<T>(
        Func<CancellationToken, Task<T>> work,
        CancellationToken callerToken)
    {
        TaskCompletionSource<T> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        await _channel.Writer
            .WriteAsync(new QueueEntry(Wrapper, callerToken), callerToken)
            .ConfigureAwait(false);

        return await tcs.Task.ConfigureAwait(false);

        async Task Wrapper(CancellationToken ct)
        {
            try
            {
                tcs.TrySetResult(await work(ct).ConfigureAwait(false));
            }
            catch (OperationCanceledException oce)
            {
                tcs.TrySetCanceled(oce.CancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
                throw;
            }
        }
    }

    private async Task RunWorkerAsync(CancellationToken ct)
    {
        await foreach (QueueEntry entry in _channel.Reader.ReadAllAsync(ct).ConfigureAwait(false))
        {
            // Caller cancelled while waiting in the queue - skip without burning quota.
            if (entry.CallerToken.IsCancellationRequested)
                continue;

            await ApplyThrottleAsync(ct).ConfigureAwait(false);

            using CancellationTokenSource linked = CancellationTokenSource
                .CreateLinkedTokenSource(ct, entry.CallerToken);
            try
            {
                await entry.Work(linked.Token).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Individual failure must not kill the worker.
                _logger?.LogWarning(ex, "Queued iRacing API call failed.");
            }
        }
    }

    /// <summary>
    /// Optionally delays before the next request based on three inputs:
    /// <list type="bullet">
    ///   <item><c>RateLimitRemaining</c> - calls still allowed this window.</item>
    ///   <item><c>RateLimitReset</c>     - when the window resets.</item>
    ///   <item>Queue depth               - calls waiting behind this one.</item>
    /// </list>
    ///
    /// <para>
    /// <b>Exhausted budget</b>: waits until <c>RateLimitReset</c>.<br/>
    /// <b>Queue deeper than budget</b>: spreads the remaining calls evenly
    /// across the remaining window time (<c>delay = windowLeft / remaining</c>).<br/>
    /// <b>Queue shallower than budget</b>: fires immediately.
    /// </para>
    /// </summary>
    private async Task ApplyThrottleAsync(CancellationToken ct)
    {
        RateLimitState state = _state;

        // No data yet - let the very first call through.
        if (state.RateLimitRemaining is null || state.RateLimitReset is null)
            return;

        DateTimeOffset resetAt = state.RateLimitReset.Value;

        // Check window expiry first - if it has already elapsed, the next
        // response will supply a fresh budget, so there is nothing to throttle.
        // Re-read the clock here (not cached) so the comparison is accurate.
        if (resetAt - DateTimeOffset.UtcNow <= TimeSpan.Zero)
            return;

        int effective = Math.Max(0, state.RateLimitRemaining.Value - SafetyBuffer);

        // Budget exhausted - re-read the clock for an accurate wait duration
        // then block until the window resets.
        if (effective <= 0)
        {
            TimeSpan waitFor = resetAt - DateTimeOffset.UtcNow;
            if (waitFor <= TimeSpan.Zero) return;
            
            _logger?.LogInformation(
                "Rate limit exhausted. Waiting {Secs:F1}s for reset at {Reset}.",
                waitFor.TotalSeconds, resetAt);

            await Task.Delay(waitFor, ct).ConfigureAwait(false);
            
            return;
        }

        // +1 to account for the current item already dequeued by the worker.
        int pending = _channel.Reader.Count + 1;

        // Always apply a small courtesy delay when multiple requests are queued
        // to smooth out bursts even when well within the rate-limit budget.
        TimeSpan minimumDelay = pending > 1 ? TimeSpan.FromMilliseconds(20) : TimeSpan.Zero;

        if (pending < effective)
        {
            if (minimumDelay > TimeSpan.Zero)
                await Task.Delay(minimumDelay, ct).ConfigureAwait(false);
            return;
        }

        // Pace the remaining calls evenly across the remaining window, but
        // never less than the minimum courtesy delay.
        // Re-read the clock so the delay is based on how much time is truly left.
        TimeSpan windowLeft = resetAt - DateTimeOffset.UtcNow;
        TimeSpan delay = TimeSpan.FromTicks(Math.Max(minimumDelay.Ticks, (windowLeft / effective).Ticks));

        if (delay > TimeSpan.Zero)
        {
            _logger?.LogDebug(
                "Throttling: {Pending} queued, {Remaining} budget, " +
                "{Window:F1}s left → sleeping {Delay:F2}s.",
                pending, effective, windowLeft.TotalSeconds, delay.TotalSeconds);

            await Task.Delay(delay, ct).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Reads rate-limit properties from <paramref name="result"/>.
    /// All <see cref="iRApi.IDataClient"/> methods return <c>DataResponse&lt;T&gt;</c>,
    /// so the three property accessors are resolved once against that open
    /// generic's base type and reused for every call.
    /// </summary>
    private void TryCaptureRateLimit<TResult>(TResult result)
    {
        if (result is null) return;

        int? remaining = RateLimitPropertyCache.Remaining?.GetValue(result) as int?;
        int? total = RateLimitPropertyCache.Total?.GetValue(result) as int?;
        DateTimeOffset? reset = RateLimitPropertyCache.Reset?.GetValue(result) as DateTimeOffset?;

        if (remaining is null && total is null && reset is null) return;

        _state = new RateLimitState(remaining, total, reset);

        _logger?.LogTrace(
            "Rate-limit: {Remaining}/{Total}, resets {Reset}.",
            remaining, total, reset);
    }
    
    private static CancellationToken ExtractCancellationToken(IInvocation invocation)
    {
        object?[] args = invocation.Arguments;
        return args.Length > 0 && args[^1] is CancellationToken ct
            ? ct
            : CancellationToken.None;
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();
        await _workerCts.CancelAsync().ConfigureAwait(false);

        try { await _workerTask.ConfigureAwait(false); }
        catch (OperationCanceledException) { /* expected */ }

        _workerCts.Dispose();
    }

    private sealed record QueueEntry(
        Func<CancellationToken, Task> Work,
        CancellationToken CallerToken);
}