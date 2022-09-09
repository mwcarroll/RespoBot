using System;
using System.Threading;
using System.Threading.Tasks;

using Discord.WebSocket;

namespace RespoBot.Services
{
    public class PeriodicDiscordService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly DiscordSocketClient _discordClient;

        private readonly string _serviceName;

        private DateTime _nextRunTime;


        public PeriodicDiscordService(IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient discordClient, string serviceName)
        {
            _configuration = configuration;
            _logger = logger;

            _discordClient = discordClient;

            _serviceName = serviceName;
        }

        public virtual void Run()
        {

        }

        public void Initialize()
        {
            _logger.LogInformation($"Initializing {_serviceName}");

            _discordClient.Ready += Client_Ready;
        }

        private Task Client_Ready()
        {
            CancellationTokenSource tokenSource = new();

            Task timerTask = RunPeriodically(Run, DateTime.UtcNow, TimeSpan.FromMinutes(_configuration.GetValue<int>($"RespoBot:{_serviceName}Interval")), tokenSource.Token);

            return Task.CompletedTask;
        }

        private async Task RunPeriodically(Action action, DateTime startTime, TimeSpan interval, CancellationToken token)
        {
            _nextRunTime = startTime;

            while (true)
            {
                TimeSpan delay = _nextRunTime - DateTime.UtcNow;

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, token);
                }

                action();
                _nextRunTime += interval;
            }
        }
    }
}
