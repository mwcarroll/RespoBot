using System;
using System.IO;
using Aydsko.iRacingData;
using Dapper.Extensions.Caching.Memory;
using MicroOrm.Dapper.Repositories.Config;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
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
            services.AddSingleton<IDbContext>(_ => new MsSqlDbContext(configuration.GetConnectionString("Default")!));

            services.AddDapperCachingInMemory(new MemoryConfiguration
            {
                AllMethodsEnableCache = false
            });

            services.AddSingleton(
                new MapperConfiguration(
                    mc => {
                            mc.AddProfile(new MappingProfile());
                        },
                        new NullLoggerFactory()
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
            services.AddSingleton<ILogger>(svc => svc.GetRequiredService<ILogger<Events.Triggered.NewTrackedMemberEvent>>());

            services.AddSingleton<RateLimitedIRacingApiClient>();

            services.AddTransient<Events.Triggered.NewTrackedMemberEvent>();

            return services;
        }
    }
}