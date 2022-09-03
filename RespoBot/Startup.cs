using System;
using System.Data.Common;
using System.IO;
using AutoMapper;
using Aydsko.iRacingData;
using Discord.Interactions;
using Discord.WebSocket;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RespoBot.Data.DbContexts;
using RespoBot.Services;

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
                .AddEnvironmentVariables("RespoBot:")
                .Build();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(configuration);

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddSimpleConsole();
            });

            MicroOrmConfig.SqlProvider = SqlProvider.MSSQL;

            services.AddSingleton(typeof(ISqlGenerator<>), typeof(SqlGenerator<>));
            services.AddSingleton<IDbContext>(x => new MsSqlDbContext(configuration.GetConnectionString("Default")));

            services.AddSingleton(
                new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                }
                )
                .CreateMapper()
            );

            services.AddIRacingDataApi(options =>
            {
                options.Username = configuration.GetValue<string>("Aydsko.iRacing:User:Username");
                options.Password = configuration.GetValue<string>("Aydsko.iRacing:User:Password");
                options.UserAgentProductName = "RespoBot";
                options.UserAgentProductVersion = new Version(1, 0);
            });

            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
            services.AddSingleton<CommandHandler>();

            services.AddSingleton<StatsMassUpdaterService>();
            //services.AddSingleton<PublicRacesService>();
            //services.AddSingleton<HostedRacesService>();

            services.AddSingleton<EntryPoint>();

            return services;
        }
    }
}