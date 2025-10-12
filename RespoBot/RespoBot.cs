using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aydsko.iRacingData;
using Dapper.Extensions.Caching.Memory;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz;
using RespoBot.Helpers;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace RespoBot
{
    public class RespoBot
    {
        private static IConfiguration Configuration { get; set; }
        private static IServiceCollection Services { get; set; }
        public static IServiceProvider ServiceProvider { get; private set; }

        public RespoBot()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables("RespoBot:")
                .Build();

            Services = ConfigureServices();
            ServiceProvider = Services.BuildServiceProvider();

            _ = SetupDiscordService();
        }
        
        private static IServiceCollection ConfigureServices()
        {
            Services = new ServiceCollection();

            Services.AddSingleton(Configuration);

            Services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddSimpleConsole();
            });

            Services.AddQuartz(q =>
            {
                q.SchedulerName = "S1";
            });

            MicroOrmConfig.SqlProvider = SqlProvider.MSSQL;
            MicroOrmConfig.UseQuotationMarks = true;

            Services.AddSingleton(typeof(ISqlGenerator<>), typeof(SqlGenerator<>));
            Services.AddSingleton<IDbContext>(_ => new MsSqlDbContext(Configuration.GetConnectionString("Default")!));

            Services.AddDapperCachingInMemory(new MemoryConfiguration
            {
                AllMethodsEnableCache = false
            });

            Services.AddSingleton(
                new MapperConfiguration(
                    mc => {
                            mc.AddProfile(new MappingProfile());
                        },
                        new NullLoggerFactory()
                    )
                .CreateMapper()
            );
            
            Services.AddSingleton<DiscordSocketClient>();

            Services.AddIRacingDataApi(options =>
            {
                options.Username = Configuration.GetValue<string>("Aydsko.iRacing:User:Username");
                options.Password = Configuration.GetValue<string>("Aydsko.iRacing:User:Password");
                options.UserAgentProductName = "RespoBot";
                options.UserAgentProductVersion = new Version(1, 0);
            });

            Services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<DiscordSocketClient>>());
            Services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<IDataClient>>());
            Services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Events.Triggered.NewTrackedMemberEvent>>());

            Services.AddSingleton<RateLimitedIRacingApiClient>();

            Services.AddTransient<Events.Triggered.NewTrackedMemberEvent>();

            return Services;
        }

        private static async Task SetupDiscordService()
        {
            try
            {
                DiscordSocketClient discordClient = ServiceProvider.GetRequiredService<DiscordSocketClient>();

                discordClient.Log += message =>
                {
                    ILogger<DiscordSocketClient> logger = ServiceProvider.GetRequiredService<ILogger<DiscordSocketClient>>();
                    
                    switch (message.Severity)
                    {
                        case LogSeverity.Critical:
                            logger.Log(LogLevel.Critical, message.Exception, message.Message);
                            break;
                        case LogSeverity.Error:
                            logger.Log(LogLevel.Error, message.Exception, message.Message);
                            break;
                        case LogSeverity.Warning:
                            logger.Log(LogLevel.Warning, message.Exception, message.Message);
                            break;
                        case LogSeverity.Info:
                            logger.Log(LogLevel.Information, message.Exception, message.Message);
                            break;
                        case LogSeverity.Verbose:
                            logger.Log(LogLevel.Trace, message.Exception, message.Message);
                            break;
                        case LogSeverity.Debug:
                            logger.Log(LogLevel.Debug, message.Exception, message.Message);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Task.CompletedTask;
                };
                
                await discordClient.LoginAsync(TokenType.Bot, Configuration.GetValue<string>("Respobot:Discord:Token"), true);
                await discordClient.StartAsync();

                discordClient.Ready += async () =>
                {
                    InteractionService interactionService = new(discordClient);
                    await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
                    await interactionService.RegisterCommandsGloballyAsync();

                    discordClient.InteractionCreated += async interaction =>
                    {
                        IServiceScope scope = ServiceProvider.CreateScope();
                        SocketInteractionContext ctx = new(discordClient, interaction);
                        await interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
                    };
                };
                
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }
    }
}