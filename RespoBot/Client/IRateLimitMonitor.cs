using Aydsko.iRacingData;

namespace RespoBot.Client;

/// <summary>
/// Exposes the current rate-limit state without requiring callers to know
/// the concrete proxy type. Cast the <see cref="IDataClient"/> returned by
/// <see cref="RateLimitedDataClient.Create"/> to this interface.
/// </summary>
public interface IRateLimitMonitor
{
    RateLimitState CurrentRateLimitState { get; }
}