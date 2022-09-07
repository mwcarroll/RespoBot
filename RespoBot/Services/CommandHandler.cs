using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace RespoBot.Services
{
    public class CommandHandler
    {

        private readonly IServiceProvider ServiceProvider;
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly DiscordSocketClient Client;
        private readonly InteractionService Commands;

        public CommandHandler(IServiceProvider services, IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient client, InteractionService commands)
        {
            ServiceProvider = services;
            Configuration = configuration;
            Logger = logger;
            Client = client;
            Commands = commands;
        }

        public async Task InitializeAsync()
        {
            Logger.LogInformation($"Initializing CommandHandler");

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);

            Client.Ready += Client_Ready;

            Client.InteractionCreated += HandleInteraction;

            Commands.SlashCommandExecuted += SlashCommandExecuted;
        }

        private async Task Client_Ready()
        {
            if (Program.IsDebug())
            {
                await Commands.RegisterCommandsToGuildAsync(Configuration.GetValue<ulong>("RespoBot:TestGuildId"), true);
            }                
            else
                await Commands.RegisterCommandsGloballyAsync(true);
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                SocketInteractionContext ctx = new(Client, arg);
                await Commands.ExecuteCommandAsync(ctx, ServiceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        Logger.LogInformation(message: arg3.ErrorReason, arg3);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        Logger.LogInformation(message: arg3.ErrorReason, arg3);
                        break;
                    case InteractionCommandError.BadArgs:
                        Logger.LogInformation(message: arg3.ErrorReason, arg3);
                        break;
                    case InteractionCommandError.Exception:
                        Logger.LogCritical(message: arg3.ErrorReason, arg3);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        Logger.LogError(message: arg3.ErrorReason, arg3);
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
