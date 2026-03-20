using System;
using System.Reflection;

namespace RespoBot.Client;

/// <summary>
/// Resolves the three rate-limit <see cref="PropertyInfo"/>
/// against <c>DataResponse&lt;T&gt;</c>'s non-generic base type.
/// </summary>
internal static class RateLimitPropertyCache
{
    // Resolve against the open-generic base so we only pay this cost once
    // for the entire lifetime of the application.
    private static readonly Type _baseType = typeof(iRApi.Common.DataResponse<>);

    public static readonly PropertyInfo? Remaining =
        _baseType.GetProperty("RateLimitRemaining");

    public static readonly PropertyInfo? Total =
        _baseType.GetProperty("TotalRateLimit");

    public static readonly PropertyInfo? Reset =
        _baseType.GetProperty("RateLimitReset");
}