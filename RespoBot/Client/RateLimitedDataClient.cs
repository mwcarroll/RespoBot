using System;
using Castle.DynamicProxy;

namespace RespoBot.Client;

/// <summary>
/// Creates an <see cref="iRApi.IDataClient"/> proxy that transparently queues every
/// call and paces execution to respect iRacing API rate limits.
/// </summary>
public static class RateLimitedDataClient
{
    private static readonly ProxyGenerator _generator = new();

    /// <summary>
    /// Wraps <paramref name="inner"/> with a rate-limit-aware queuing proxy.
    ///
    /// The returned object implements <see cref="iRApi.IDataClient"/> and also
    /// <see cref="IRateLimitMonitor"/> (cast to inspect the current state)
    /// and <see cref="IAsyncDisposable"/> (cast to shut down cleanly).
    /// </summary>
    public static iRApi.IDataClient Create(
        iRApi.IDataClient inner,
        ILogger<RateLimitInterceptor>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(inner);

        RateLimitInterceptor interceptor = new(logger);

        return (iRApi.IDataClient)_generator.CreateInterfaceProxyWithTarget(
            interfaceToProxy: typeof(iRApi.IDataClient),
            additionalInterfacesToProxy: [typeof(IRateLimitMonitor), typeof(IAsyncDisposable)],
            target: inner,
            interceptors: interceptor);
    }
}