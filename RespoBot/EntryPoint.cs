using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RespoBot.Services;
using RespoBot.Services.PeriodicServices;

namespace RespoBot
{
    public class EntryPoint
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly DiscordSocketClient _discordClient;
        private readonly InteractionService _commands;

        private readonly iRApi.IDataClient _iRacingDataClient;

        private readonly IDbContext _db;

        public EntryPoint(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient discordClient, InteractionService commands, iRApi.IDataClient iRacingDataClient, IDbContext db)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;

            _discordClient = discordClient;
            _commands = commands;

            _iRacingDataClient = iRacingDataClient;

            _db = db;
        }

        public async Task Run(string[] args)
        {
            //_discordClient.Ready += Client_Ready;
            //_discordClient.Log += Log;
            //_commands.Log += Log;

            //await _discordClient.LoginAsync(TokenType.Bot, _configuration["RespoBot:Token"]);
            //await _discordClient.StartAsync();

            // await _serviceProvider.GetRequiredService<CommandHandler>().InitializeAsync();

            _serviceProvider.GetRequiredService<TaskQueueService>().Run();
            _serviceProvider.GetRequiredService<SeriesIndexerService>().Run();
            //_serviceProvider.GetRequiredService<SubSessionIndexerService>().Run();

            await Task.Delay(Timeout.Infinite);
        }

        private Task Client_Ready()
        {
            _logger.LogInformation($"Connected as -> [{_discordClient.CurrentUser}]");

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