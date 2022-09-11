using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;
using static RespoBot.Services.PeriodicServices.RateLimitService;

namespace RespoBot.Services
{
    public class RequestHandlerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private Dictionary<Task, Guid> _pendingRequests = new();
        private Dictionary<Guid, DateTimeOffset> _taskGroupStartedAt = new();
        private int _expectedRequests;

        object _lock = new();

        public RequestHandlerService(IConfiguration configuration, ILogger<EntryPoint> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private int GetPerRequestDelay(Guid requestGroup)
        {
            DateTimeOffset reset = RateLimitData.RateLimitReset;
            _taskGroupStartedAt.TryGetValue(requestGroup, out DateTimeOffset now);

            TimeSpan difference = reset - now;

            double rateLimitThreshold = _configuration.GetValue<double>("RespoBot:RateLimit:Threshold");
            int configuredMinimumDelay = _configuration.GetValue<int>("RespoBot:RateLimit:MinimumDelayMilliseconds");

            var delay = (_expectedRequests > (RateLimitData.RateLimitRemaining * rateLimitThreshold)) ? Math.Max(
                    configuredMinimumDelay,
                    (int)(difference.TotalMilliseconds / (RateLimitData.RateLimitRemaining * rateLimitThreshold))
                ) : configuredMinimumDelay;

            return delay;
        }

        public async Task AddRequest<TData>(Task<TData> request, Guid requestGroup, int expectedRequests)
        {
            lock (_lock)
            {
                if (!_taskGroupStartedAt.ContainsKey(requestGroup))
                {
                    _expectedRequests += expectedRequests;
                    _taskGroupStartedAt.Add(requestGroup, DateTimeOffset.UtcNow);
                }
            }

            _pendingRequests.Add(request, requestGroup);

            await Task.Delay(GetPerRequestDelay(requestGroup));
        }

        public List<Task<TData>> GetResponses<TData>(Guid requestGroup)
        {
            List<Task<TData>> responses = _pendingRequests.Where(x => x.Value.Equals(requestGroup)).Select(x => (Task<TData>)x.Key).ToList();

            _pendingRequests = _pendingRequests.Where(x => !x.Value.Equals(requestGroup)).ToDictionary(x => x.Key, x => x.Value);

            Task.WhenAll(responses).ConfigureAwait(false);

            if (!_pendingRequests.Any())
                _expectedRequests = 0;
            else
                _expectedRequests = ((_expectedRequests - responses.Count) >= 0) ? (_expectedRequests - responses.Count) : 0;

            return responses;
        }
    }
}
