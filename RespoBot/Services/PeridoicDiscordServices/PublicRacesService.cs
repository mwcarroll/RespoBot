using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;

using iRApi = Aydsko.iRacingData;

using Discord.WebSocket;

using DataContext = RespoBot.Data.Classes;
using RespoBot.Data.DbContexts;

namespace RespoBot.Services.PeriodicServices
{
    public class PublicRacesService : PeriodicDiscordService
    {
        private readonly string _connectionString;

        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly IDbContext Db;

        private readonly IMapper Mapper;

        private readonly iRApi.IDataClient IRacingDataClient;

        public PublicRacesService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, iRApi.IDataClient iRacingDataClient, DiscordSocketClient discordClient) :
            base(configuration, logger, discordClient, nameof(PublicRacesService))
        {
            Configuration = configuration;
            Logger = logger;

            Db = db;

            Mapper = mapper;

            IRacingDataClient = iRacingDataClient;

            _connectionString = Configuration.GetConnectionString("Default");
        }

        public override async void Run()
        {
            Logger.LogInformation("iRacing Stats - Public Races Fired");

            IEnumerable<DataContext.Member> members = Db.Members.FindAll<DataContext.MemberInfo>(predicate: null, p => p.MemberInfo);

            foreach (DataContext.Member member in members) {

            }
        }
    }
}