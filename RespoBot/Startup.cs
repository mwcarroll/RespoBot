using System;
using System.IO;
using Aydsko.iRacingData;
using Dapper.Extensions.Caching.Memory;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RespoBot.Helpers;

namespace RespoBot
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: false)
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

            services.AddQuartz(q =>
            {
                q.SchedulerName = "S1";
            });

            MicroOrmConfig.SqlProvider = SqlProvider.MSSQL;
            MicroOrmConfig.UseQuotationMarks = true;

            services.AddSingleton(typeof(ISqlGenerator<>), typeof(SqlGenerator<>));
            services.AddTransient<IDbContext>(_ => new MsSqlDbContext(configuration.GetConnectionString("Default")));

            services.AddDapperCachingInMemory(new MemoryConfiguration
            {
                AllMethodsEnableCache = false
            });

            services.AddSingleton(
                new MapperConfiguration(
                    mc => {
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

            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<IDataClient>>());
            
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.EventHandlers.MemberInfoUpdatedEventHandlerService>>());
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.EventHandlers.SubSessionIdentifierIndexedEventHandlerService>>());
            
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.Periodic.MemberChartInfoPeriodicRespoBotTask>>());
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.Periodic.MemberInfoPeriodicRespoBotTask>>());
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.Periodic.SubSessionIndexerPeriodicRespoBotTask>>());

            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.Triggered.TrackInfoRespoBotTask>>());
            
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Tasks.Periodic.TestPeriodicRespoBotTask>>());

            services.AddSingleton<RateLimitedIRacingApiClient>();

            // services.AddSingleton<Services.EventHandlers.MemberInfoUpdatedEventHandlerService>();
            // services.AddSingleton<Services.EventHandlers.SubSessionIdentifierIndexedEventHandlerService>();
            //
            // services.AddSingleton<Services.Periodic.MemberChartInfoPeriodicService>();
            // services.AddTransient<Services.Periodic.MemberInfoPeriodicService>();
            
            services.AddTransient<Tasks.Periodic.TestPeriodicRespoBotTask>();

            return services;
        }
    }
}