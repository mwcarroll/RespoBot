using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;


using iRApi = Aydsko.iRacingData;

namespace RespoBot.Services.PeriodicServices
{
    public class RateLimitService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly string _serviceName;

        private DateTime _nextRunTime;

        private readonly iRApi.IDataClient IRacingDataClient;

        private DateTimeOffset _rateLimitReset;
        private int _totalRateLimit;
        private int _rateLimitRemaining;

        private Dictionary<Task, Guid> _pendingRequests = new();
        private int _expectedRequests = 0;

        public RateLimitService(IConfiguration configuration, ILogger<EntryPoint> logger, iRApi.IDataClient iRacingDataClient)
        {
            Configuration = configuration;
            Logger = logger;

            _serviceName = nameof(RateLimitService);

            IRacingDataClient = iRacingDataClient;
        }

        public async Task UpdateRateLimits()
        {
            await IRacingDataClient.GetMyInfoAsync();
            iRApi.Common.DataResponse<iRApi.Lookups.LookupGroup[]> response = await IRacingDataClient.GetLookupsAsync();

            _rateLimitReset = (DateTimeOffset)response.RateLimitReset;
            _totalRateLimit = (int)response.TotalRateLimit;
            _rateLimitRemaining = (int)response.RateLimitRemaining;
        }

        public async Task InitializeAsync(bool runImmediate = true)
        {
            Logger.LogInformation($"Initializing {_serviceName}");

            CancellationTokenSource tokenSource = new();

            // run once, immediately
            if (runImmediate)
                await UpdateRateLimits();

            // continue running
            _ = RunPeriodically(UpdateRateLimits, DateTime.UtcNow + TimeSpan.FromMinutes(Configuration.GetValue<int>($"RespoBot:{_serviceName}Interval")), TimeSpan.FromMinutes(Configuration.GetValue<int>($"RespoBot:{_serviceName}Interval")), tokenSource.Token);
        }

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
            double rateLimitThreshold = Configuration.GetValue<double>($"RespoBot:RateLimit:Threshold");
            int minimumDelay = Configuration.GetValue<int>($"RespoBot:RateLimit:MinimumDelayMilliseconds");

            int delay = minimumDelay;

            if(_expectedRequests > ((int)(_rateLimitRemaining * rateLimitThreshold)))
                delay = (int)((_rateLimitReset.ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / (_totalRateLimit * rateLimitThreshold));

            return (delay > minimumDelay) ? delay : minimumDelay;
        }

        public async Task AddRequest<TData>(Task<TData> request, Guid requestGroup, int expectedRequests){
            _pendingRequests.Add(request, requestGroup);

            if(!_pendingRequests.Where(x => x.Value.Equals(requestGroup)).Any())
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
