using Discord;
using System.Configuration;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Discord.WebSocket;

namespace RespoBot.Services
{
    public class PeriodicService : IPeriodicService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly DiscordSocketClient DiscordClient;

        private readonly string _intervalConfigName;

        private DateTime _nextRunTime;


        public PeriodicService(IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient discordClient, string intervalConfigName)
        {
            Configuration = configuration;
            Logger = logger;

            DiscordClient = discordClient;

            _intervalConfigName = intervalConfigName;
        }

        public virtual async void Run()
        {

        }

        public void Initialize()
        {
            Logger.LogInformation($"Initializing ResultsService");

            DiscordClient.Ready += Client_Ready;
        }

        private Task Client_Ready()
        {
            CancellationTokenSource tokenSource = new();

            Task timerTask = RunPeriodically(Run, DateTime.UtcNow, TimeSpan.FromMinutes(Configuration.GetValue<int>($"RespoBot:{_intervalConfigName}Interval")), tokenSource.Token);

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
