using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace RespoBot.Services.PeriodicServices
{
    public class RateLimitService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly string _serviceName;

        private DateTime _nextRunTime;

        private readonly iRApi.IDataClient _iRacingDataClient;

        private static DateTimeOffset _rateLimitReset;
        private static int _totalRateLimit;
        private static int _rateLimitRemaining;

        public RateLimitService(IConfiguration configuration, ILogger<EntryPoint> logger, iRApi.IDataClient iRacingDataClient)
        {
            _configuration = configuration;
            _logger = logger;

            _serviceName = nameof(RateLimitService);

            _iRacingDataClient = iRacingDataClient;
        }

        private async Task UpdateRateLimits()
        {
            await _iRacingDataClient.GetMyInfoAsync();
            iRApi.Common.DataResponse<iRApi.Lookups.LookupGroup[]> response = await _iRacingDataClient.GetLookupsAsync();

            if (response.RateLimitReset != null) _rateLimitReset = (DateTimeOffset)response.RateLimitReset;
            if (response.TotalRateLimit != null) _totalRateLimit = (int)response.TotalRateLimit;
            if (response.RateLimitRemaining != null) _rateLimitRemaining = (int)response.RateLimitRemaining;
        }

        public async Task InitializeAsync(bool runImmediate = true)
        {
            _logger.LogInformation($"Initializing {_serviceName}");

            CancellationTokenSource tokenSource = new();

            // run once, immediately
            if (runImmediate)
                await UpdateRateLimits();

            // continue running
            _ = RunPeriodically(UpdateRateLimits, DateTime.UtcNow + TimeSpan.FromMinutes(_configuration.GetValue<int>($"RespoBot:{_serviceName}Interval")), TimeSpan.FromMinutes(_configuration.GetValue<int>($"RespoBot:{_serviceName}Interval")), tokenSource.Token);
        }

        public RateLimitData GetLimitData()
        {
            return new RateLimitData();
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        private async Task RunPeriodically(Func<Task> action, DateTime startTime, TimeSpan interval, CancellationToken token)
        {
            _nextRunTime = startTime;

            while (true)
            {
                TimeSpan delay = _nextRunTime - DateTime.UtcNow;

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, token);
                }

                await action();
                _nextRunTime += interval;
            }
        }

        public struct RateLimitData
        {
            public static DateTimeOffset RateLimitReset => _rateLimitReset;
            public static int TotalRateLimit => _totalRateLimit;
            public static int RateLimitRemaining => _rateLimitRemaining;
        }
    }
}
