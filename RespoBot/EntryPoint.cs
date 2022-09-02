using System;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using RespoBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace RespoBot
{
    public class EntryPoint
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly DiscordSocketClient Client;
        private readonly InteractionService Commands;

        public EntryPoint(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient client, InteractionService commands)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            Logger = logger;

            Client = client;
            Commands = commands;
        }

        public async Task Run(String[] args)
        {
            Client.Ready += Client_Ready;
            Client.Log += Log;
            Commands.Log += Log;

            await Client.LoginAsync(TokenType.Bot, Configuration["RespoBot:Token"]);
            await Client.StartAsync();

            await ServiceProvider.GetRequiredService<CommandHandler>().InitializeAsync();
            ServiceProvider.GetRequiredService<ResultsService>().Initialize();

            await Task.Delay(Timeout.Infinite);
        }

        private Task Client_Ready()
        {
            Logger.LogInformation($"Connected as -> [{Client.CurrentUser}]");

            return Task.CompletedTask;
        }

        private Task Log(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    Logger.LogCritical(message: arg.Message);
                    break;
                case LogSeverity.Error:
                    Logger.LogError(message: arg.Exception.Message);
                    break;
                case LogSeverity.Warning:
                    Logger.LogWarning(message: arg.Message);
                    break;
                case LogSeverity.Info:
                    Logger.LogInformation(message: arg.Message);
                    break;
                case LogSeverity.Verbose:
                    Logger.LogTrace(message: arg.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.LogDebug(message: arg.Message);
                    break;
            }

            return Task.CompletedTask;
        }        
    }
}