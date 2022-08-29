using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RespoBot.Services
{
    public class ResultsService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly DiscordSocketClient Client;

        private DateTime _nextRunTime;

        public ResultsService(IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient client)
        {
            Configuration = configuration;
            Logger = logger;

            Client = client;
        }

        public void Initialize()
        {
            Logger.LogInformation($"Initializing ResultsService");

            Client.Ready += Client_Ready;
        }

        private Task Client_Ready()
        {
            CancellationTokenSource tokenSource = new();

            Task timerTask = RunPeriodically(CheckIRacingResultsForUsers, DateTime.UtcNow, TimeSpan.FromMinutes(Configuration.GetValue<int>("RespoBot:ResultsServiceInterval")), tokenSource.Token);

            return Task.CompletedTask;
        }

        private void CheckIRacingResultsForUsers()
        {
            Logger.LogInformation($"{DateTime.Now:HH:mm:ss} Check iRacing Results - Fired");
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
