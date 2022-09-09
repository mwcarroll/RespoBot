using System;
using System.Collections.Generic;
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
        private int _expectedRequests = 0;

        public RequestHandlerService(IConfiguration configuration, ILogger<EntryPoint> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private int GetPerRequestDelay()
        {
            double rateLimitThreshold = _configuration.GetValue<double>($"RespoBot:RateLimit:Threshold");
            int minimumDelay = _configuration.GetValue<int>($"RespoBot:RateLimit:MinimumDelayMilliseconds");

            int delay = minimumDelay;

            if (_expectedRequests > ((int)(RateLimitData.RateLimitRemaining * rateLimitThreshold)))
                delay = (int)((RateLimitData.RateLimitReset.ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / (RateLimitData.TotalRateLimit * rateLimitThreshold));

            return (delay > minimumDelay) ? delay : minimumDelay;
        }

        public async Task AddRequest<TData>(Task<TData> request, Guid requestGroup, int expectedRequests)
        {
            _pendingRequests.Add(request, requestGroup);

            if (!_pendingRequests.Any(x => x.Value.Equals(requestGroup)))
                _expectedRequests += expectedRequests;

            await Task.Delay(GetPerRequestDelay());
        }

        public List<Task<TData>> GetResponses<TData>(Guid requestGroup)
        {
            List<Task<TData>> responses = _pendingRequests.Where(x => x.Value.Equals(requestGroup)).Select(x => (Task<TData>)x.Key).ToList();

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
