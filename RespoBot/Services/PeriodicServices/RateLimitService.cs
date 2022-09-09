using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

using Aydsko.iRacingData;
using iRApiCommon = Aydsko.iRacingData.Common;
using iRApiLookups = Aydsko.iRacingData.Lookups;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace RespoBot.Services.PeriodicServices
{
    public class RateLimitService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly string _serviceName;

        private DateTime _nextRunTime;

        private readonly IDataClient IRacingDataClient;

        private DateTimeOffset _rateLimitReset;
        private int _totalRateLimit;
        private int _rateLimitRemaining;

        private Dictionary<Task, Guid> pendingRequests = new();

        public RateLimitService(IConfiguration configuration, ILogger<EntryPoint> logger, IDataClient iRacingDataClient)
        {
            Configuration = configuration;
            Logger = logger;

            _serviceName = nameof(RateLimitService);

            IRacingDataClient = iRacingDataClient;
        }

        public async Task UpdateRateLimits()
        {
            await IRacingDataClient.GetMyInfoAsync();
            iRApiCommon.DataResponse<iRApiLookups.LookupGroup[]> response = await IRacingDataClient.GetLookupsAsync();

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

        public int GetPerRequestDelay(int expectedRequests)
        {
            double rateLimitThreshold = Configuration.GetValue<double>($"RespoBot:RateLimit:Threshold");

            if (expectedRequests < ((int)(_rateLimitRemaining * rateLimitThreshold)))
                return Configuration.GetValue<int>($"RespoBot:RateLimit:MinimumDelayMilliseconds");
            if (expectedRequests < _totalRateLimit)
                return (int)((_rateLimitReset.ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / (expectedRequests * rateLimitThreshold));
            else
                return (int)((_rateLimitReset.ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / (_totalRateLimit * rateLimitThreshold));
        }

        

        public async Task AddRequest<TData>(Task<TData> request, Guid requestGroup, int expectedRequests){
            pendingRequests.Add(request, requestGroup);

            await Task.Delay(GetPerRequestDelay(expectedRequests));
        }

        public List<Task<TData>> GetResponses<TData>(Guid requestGroup)
        {
            List<Task<TData>> responses = pendingRequests.Where(x => x.Value.Equals(requestGroup)).Select(x => (Task<TData>) x.Key).ToList();

            pendingRequests = pendingRequests.Where(x => !x.Value.Equals(requestGroup)).ToDictionary(x => x.Key, x => x.Value);

            Task.WhenAll(responses);

            return responses;
        }
    }
}
