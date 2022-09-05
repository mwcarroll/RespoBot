using Aydsko.iRacingData;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RespoBot.Data.DbContexts;
using AutoMapper;
using DataContext = RespoBot.Data.Classes;
using System.Collections.Generic;
using System;
using Searches = Aydsko.iRacingData.Searches;
using Constants = Aydsko.iRacingData.Constants;
using Aydsko.iRacingData.Exceptions;
using System.Linq;
using RespoBot.Data.Classes.Events;

namespace RespoBot.Services.PeriodicServices
{
    public class PublicRacesService : PeriodicService
    {
        private readonly string _connectionString;

        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly IDbContext Db;

        private readonly IMapper Mapper;

        private readonly IDataClient IRacingDataClient;

        public PublicRacesService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, IDataClient iRacingDataClient, DiscordSocketClient discordClient) :
            base(configuration, logger, discordClient, "PublicRacesService")
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

            IEnumerable<DataContext.Member> members = Db.Members.FindAll<DataContext.MemberInfo>(x => x.iRacingMemberId == 386110, p => p.MemberInfo);

            List<PublicEvents> racesToInsert = new();
            List<PublicEvents> racesToUpdate = new();

            DateTime startDateTime = DateTime.MinValue;

            foreach (DataContext.Member member in members) {
                try
                {
                    DateTime now = DateTime.UtcNow;

                    Constants.EventType[] eventTypes = (await IRacingDataClient.GetEventTypesAsync()).Data;

                    (Searches.OfficialSearchResultHeader header, Searches.OfficialSearchResultItem[] races) = (await IRacingDataClient.SearchOfficialResultsAsync(new Searches.OfficialSearchParameters()
                    {
                        StartRangeBegin = now.AddDays(-90),
                        StartRangeEnd = now,
                        ParticipantCustomerId = member.iRacingMemberId,
                        EventTypes = new int[] { eventTypes.Where(x => x.Label.Equals("Race")).Select(x => x.Value).FirstOrDefault() }
                    })).Data;


                }
                catch (iRacingUnauthorizedResponseException ex)
                {
                    Logger.LogInformation(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    Logger.LogError(ex.Message);
                }
                catch(iRacingDataClientException ex)
                {
                    Logger.LogError(ex.Message);
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex.Message);
                }
                

                //foreach (var race in searchResultRaces)
                //{
                //    DataContext.Race cachedRace = Db.Races.Find(x => x.SubsessionId == race.SubsessionId && x.iRacingMemberId == member.iRacingMemberId);

                //    DataContext.Race mappedRace = Mapper.Map<DataContext.Race>(race);

                //    if (cachedRace == null)
                //        racesToInsert.Add(mappedRace);
                //    else if (!mappedRace.Equals(cachedRace))
                //        racesToUpdate.Add(mappedRace);
                //}
            }

            //if(racesToInsert.Count > 0)
            //    Db.Races.BulkInsert(racesToInsert);
            //if(racesToUpdate.Count > 0)
            //    Db.Races.BulkUpdate(racesToUpdate);
        }
    }
}