using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;


namespace RespoBot.Services.PeriodicServices
{
    public class RateLimitService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly string _serviceName;

        private DateTime _nextRunTime;

        private readonly iRApi.IDataClient _racingDataClient;

        private DateTimeOffset _rateLimitReset;
        private int _totalRateLimit;
        private int _rateLimitRemaining;

        private Dictionary<Task, Guid> _pendingRequests = new();
        private int _expectedRequests = 0;

        public RateLimitService(IConfiguration configuration, ILogger<EntryPoint> logger, iRApi.IDataClient iRacingDataClient)
        {
            _configuration = configuration;
            _logger = logger;

            _serviceName = nameof(RateLimitService);

            _racingDataClient = iRacingDataClient;
        }

        public async Task UpdateRateLimits()
        {
            await _racingDataClient.GetMyInfoAsync();
            iRApi.Common.DataResponse<iRApi.Lookups.LookupGroup[]> response = await _racingDataClient.GetLookupsAsync();

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

        private int GetPerRequestDelay()
        {
            double rateLimitThreshold = _configuration.GetValue<double>($"RespoBot:RateLimit:Threshold");
            int minimumDelay = _configuration.GetValue<int>($"RespoBot:RateLimit:MinimumDelayMilliseconds");

            int delay = minimumDelay;

            if(_expectedRequests > ((int)(_rateLimitRemaining * rateLimitThreshold)))
                delay = (int)((_rateLimitReset.ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / (_totalRateLimit * rateLimitThreshold));

            return (delay > minimumDelay) ? delay : minimumDelay;
        }

        public async Task AddRequest<TData>(Task<TData> request, Guid requestGroup, int expectedRequests){
            _pendingRequests.Add(request, requestGroup);

            if(!_pendingRequests.Any(x => x.Value.Equals(requestGroup)))
                _expectedRequests += expectedRequests;

            await Task.Delay(GetPerRequestDelay());
        }

        public List<Task<TData>> GetResponses<TData>(Guid requestGroup)
        {
            List<Task<TData>> responses = _pendingRequests.Where(x => x.Value.Equals(requestGroup)).Select(x => (Task<TData>) x.Key).ToList();

            _pendingRequests = _pendingRequests.Where(x => !x.Value.Equals(requestGroup)).ToDictionary(x => x.Key, x => x.Value);

            Task.WhenAll(responses);

            if (!_pendingRequests.Any())
                _expectedRequests = 0;
            else 
                _expectedRequests = ((_expectedRequests - responses.Count) >= 0) ? (_expectedRequests - responses.Count) : 0;

            return responses;
        }
    }
}
