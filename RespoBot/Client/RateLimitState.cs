using System;

namespace RespoBot.Client;

/// <summary>Immutable snapshot of the last-observed iRacing rate-limit headers.</summary>
public sealed record RateLimitState(
    int? RateLimitRemaining,
    int? TotalRateLimit,
    DateTimeOffset? RateLimitReset);