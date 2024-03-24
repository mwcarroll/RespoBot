using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace RespoBot.Helpers
{
    internal class RateLimitedIRacingApiClient
    {
        public iRApi.IDataClient DataClient { get; }
        
        private readonly Polly.Wrap.AsyncPolicyWrap _policy;
        private readonly ILogger<RateLimitedIRacingApiClient> _logger;

        private int _counter = 0;

        public RateLimitedIRacingApiClient(ILogger<RateLimitedIRacingApiClient> logger, iRApi.IDataClient dataClient)
        {
            _logger = logger;
            DataClient = dataClient;

            // handle DataClient unauthorized
            Polly.Retry.AsyncRetryPolicy authPolicy = Policy
                .Handle<iRApi.Exceptions.iRacingUnauthorizedResponseException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt), onRetry: async (response, timespan) => {
                    await DataClient.LoginExternalAsync();
                });

            // circuit breaker for iRacing rate limiting
            Polly.CircuitBreaker.AsyncCircuitBreakerPolicy circuitBreakerPolicy = Policy
                .Handle<iRApi.Exceptions.iRacingRateLimitExceededException>()
                .CircuitBreakerAsync(1, TimeSpan.FromSeconds(0),
                    onBreak: (result, state, _, __) =>
                    {
                        if (state == Polly.CircuitBreaker.CircuitState.Open) return;
                        if (result.InnerException != null) throw result.InnerException;
                    },
                    onReset: (_) => { },
                    onHalfOpen: () => { }
                );

            CancellationTokenSource throttlingEndSignal;

            // retry when rate limited by iRacing
            Polly.Retry.AsyncRetryPolicy circuitBreakerRetryPolicy = Policy
                .Handle<iRApi.Exceptions.iRacingRateLimitExceededException>()
                .Or<Polly.CircuitBreaker.IsolatedCircuitException>()
                .WaitAndRetryForeverAsync(
                    _ => TimeSpan.FromSeconds(3),
                    onRetry: (ex, __) =>
                    {
                        if (ex.InnerException is not iRApi.Exceptions.iRacingRateLimitExceededException irleex) return;

                        circuitBreakerPolicy.Isolate();
                        throttlingEndSignal = new CancellationTokenSource(irleex.HResult);
                        throttlingEndSignal.Token.Register(() => circuitBreakerPolicy.Reset());
                    });


            // rate limit
            Polly.RateLimit.AsyncRateLimitPolicy rateLimitPolicy = Policy
                .RateLimitAsync(220, TimeSpan.FromMinutes(1), 20);

            // retry when rate limited by policy
            Polly.Retry.AsyncRetryPolicy rateLimitRetryPolicy = Policy
                .Handle<Polly.RateLimit.RateLimitRejectedException>(e => e.RetryAfter > TimeSpan.Zero)
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: (i, e, ctx) => ((Polly.RateLimit.RateLimitRejectedException)e).RetryAfter,
                    onRetryAsync: (e, ts, i, ctx) => Task.CompletedTask
                );

            _policy = Policy.WrapAsync(
                    authPolicy,
                    circuitBreakerRetryPolicy,
                    circuitBreakerPolicy,
                    rateLimitRetryPolicy,
                    rateLimitPolicy
                );
        }

        public async Task<iRApi.Common.DataResponse<TData>> ExecuteAsync<TData>(Func<Task<iRApi.Common.DataResponse<TData>>> action)
        {
            return await _policy.ExecuteAsync(action);
        }
    }
}
