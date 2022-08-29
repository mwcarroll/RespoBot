using System.IO;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RespoBot.Services;
using Serilog;

namespace RespoBot
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<Program>()
                .Build();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(configuration);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
            });

            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<ResultsService>();

            services.AddSingleton<EntryPoint>();

            return services;
        }
    }
}