using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RespoBot.Services;
using RespoBot.Services.PeriodicServices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RespoBot
{
    public class EntryPoint
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;

        public EntryPoint(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient client, InteractionService commands)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;

            _client = client;
            _commands = commands;
        }

        public async Task Run(string[] args)
        {
            await _serviceProvider.GetRequiredService<RateLimitService>().InitializeAsync();
            _serviceProvider.GetRequiredService<DataHelperService>().Run();

            //Client.Ready += Client_Ready;
            //Client.Log += Log;
            //Commands.Log += Log;

            //await Client.LoginAsync(TokenType.Bot, Configuration["RespoBot:Token"]);
            //await Client.StartAsync();

            //await ServiceProvider.GetRequiredService<CommandHandler>().InitializeAsync();
            //ServiceProvider.GetRequiredService<StatsMassUpdaterService>().Initialize();
            //ServiceProvider.GetRequiredService<PublicRacesService>().Initialize();

            await Task.Delay(Timeout.Infinite);
        }

        private Task Client_Ready()
        {
            _logger.LogInformation($"Connected as -> [{_client.CurrentUser}]");

            return Task.CompletedTask;
        }

        private Task Log(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(message: arg.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(message: arg.Exception.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(message: arg.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(message: arg.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(message: arg.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(message: arg.Message);
                    break;
            }

            return Task.CompletedTask;
        }        
    }
}