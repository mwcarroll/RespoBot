// namespace RespoBot.Services
// {
//     public class CommandHandler
//     {
//
//         //private readonly IServiceProvider _serviceProvider;
//         //private readonly IConfiguration _configuration;
//         //private readonly ILogger<EntryPoint> _logger;
//
//         //private readonly DiscordSocketClient _client;
//         //private readonly InteractionService _commands;
//
//         //public CommandHandler(IServiceProvider services, IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient client, InteractionService commands)
//         //{
//         //    _serviceProvider = services;
//         //    _configuration = configuration;
//         //    _logger = logger;
//         //    _client = client;
//         //    _commands = commands;
//         //}
//
//         //public async Task InitializeAsync()
//         //{
//         //    _logger.LogInformation("Initializing CommandHandler");
//
//         //    await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
//
//         //    _client.Ready += Client_Ready;
//
//         //    _client.InteractionCreated += HandleInteraction;
//
//         //    _commands.SlashCommandExecuted += SlashCommandExecuted;
//         //}
//
//         //private async Task Client_Ready()
//         //{
//         //    if (Program.IsDebug())
//         //    {
//         //        await _commands.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("RespoBot:TestGuildId"));
//         //    }                
//         //    else
//         //        await _commands.RegisterCommandsGloballyAsync();
//         //}
//
//         //private async Task HandleInteraction(SocketInteraction arg)
//         //{
//         //    try
//         //    {
//         //        SocketInteractionContext ctx = new(_client, arg);
//         //        await _commands.ExecuteCommandAsync(ctx, _serviceProvider);
//         //    }
//         //    catch (Exception ex)
//         //    {
//         //        Console.WriteLine(ex);
//         //        if (arg.Type == InteractionType.ApplicationCommand)
//         //        {
//         //            await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
//         //        }
//         //    }
//         //}
//
//         //private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
//         //{
//         //    if (!arg3.IsSuccess)
//         //    {
//         //        switch (arg3.Error)
//         //        {
//         //            case InteractionCommandError.UnmetPrecondition:
//         //                _logger.LogInformation(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.UnknownCommand:
//         //                _logger.LogInformation(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.BadArgs:
//         //                _logger.LogInformation(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.Exception:
//         //                _logger.LogCritical(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.Unsuccessful:
//         //                _logger.LogError(message: arg3.ErrorReason, arg3);
//         //                break;
//         //        }
//         //    }
//
//         //    return Task.CompletedTask;
//         //}
//     }
// }
// namespace RespoBot.Services
// {
//     public class CommandHandler
//     {
//
//         //private readonly IServiceProvider _serviceProvider;
//         //private readonly IConfiguration _configuration;
//         //private readonly ILogger<EntryPoint> _logger;
//
//         //private readonly DiscordSocketClient _client;
//         //private readonly InteractionService _commands;
//
//         //public CommandHandler(IServiceProvider services, IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient client, InteractionService commands)
//         //{
//         //    _serviceProvider = services;
//         //    _configuration = configuration;
//         //    _logger = logger;
//         //    _client = client;
//         //    _commands = commands;
//         //}
//
//         //public async Task InitializeAsync()
//         //{
//         //    _logger.LogInformation("Initializing CommandHandler");
//
//         //    await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
//
//         //    _client.Ready += Client_Ready;
//
//         //    _client.InteractionCreated += HandleInteraction;
//
//         //    _commands.SlashCommandExecuted += SlashCommandExecuted;
//         //}
//
//         //private async Task Client_Ready()
//         //{
//         //    if (Program.IsDebug())
//         //    {
//         //        await _commands.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("RespoBot:TestGuildId"));
//         //    }                
//         //    else
//         //        await _commands.RegisterCommandsGloballyAsync();
//         //}
//
//         //private async Task HandleInteraction(SocketInteraction arg)
//         //{
//         //    try
//         //    {
//         //        SocketInteractionContext ctx = new(_client, arg);
//         //        await _commands.ExecuteCommandAsync(ctx, _serviceProvider);
//         //    }
//         //    catch (Exception ex)
//         //    {
//         //        Console.WriteLine(ex);
//         //        if (arg.Type == InteractionType.ApplicationCommand)
//         //        {
//         //            await arg.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
//         //        }
//         //    }
//         //}
//
//         //private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
//         //{
//         //    if (!arg3.IsSuccess)
//         //    {
//         //        switch (arg3.Error)
//         //        {
//         //            case InteractionCommandError.UnmetPrecondition:
//         //                _logger.LogInformation(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.UnknownCommand:
//         //                _logger.LogInformation(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.BadArgs:
//         //                _logger.LogInformation(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.Exception:
//         //                _logger.LogCritical(message: arg3.ErrorReason, arg3);
//         //                break;
//         //            case InteractionCommandError.Unsuccessful:
//         //                _logger.LogError(message: arg3.ErrorReason, arg3);
//         //                break;
//         //        }
//         //    }
//
//         //    return Task.CompletedTask;
//         //}
//     }
// }
