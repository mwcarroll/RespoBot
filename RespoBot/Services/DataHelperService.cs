using AutoMapper;
using Aydsko.iRacingData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using RespoBot.Data.DbContexts;

using iRApiCars = Aydsko.iRacingData.Cars;
using iRApiConstants = Aydsko.iRacingData.Constants;
using iRApiSearches = Aydsko.iRacingData.Searches;
using iRApiTracks = Aydsko.iRacingData.Tracks;

using DataContext = RespoBot.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Reflection.PortableExecutable;

namespace RespoBot.Services
{
    public class DataHelperService
    {
        private readonly string _connectionString;

        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly IDbContext Db;

        private readonly IMapper Mapper;

        private readonly IDataClient IRacingDataClient;

        public DataHelperService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, IDataClient iRacingDataClient)
        {
            Configuration = configuration;
            Logger = logger;

            Db = db;

            Mapper = mapper;

            IRacingDataClient = iRacingDataClient;

            _connectionString = Configuration.GetConnectionString("Default");
        }

        public void Run()
        {
            // UpdateEventTypes();
            // UpdateTracks();
            // UpdateCars();
            RepopulatePublicRaces();
        }

        public async void RepopulatePublicRaces()
        {
            List<DataContext.Events.PublicEvents> mappedRaces = new();

            foreach (DataContext.Member member in Db.Members.FindAll<DataContext.MemberInfo>(null, p => p.MemberInfo))
            {
                DateTime dateToSearch = DateTime.UtcNow;

                while (dateToSearch > member.MemberInfo.MemberSince)
                {
                    try
                    {
                        (iRApiSearches.OfficialSearchResultHeader header, iRApiSearches.OfficialSearchResultItem[] races) =
                            (await IRacingDataClient.SearchOfficialResultsAsync(new iRApiSearches.OfficialSearchParameters()
                            {
                                StartRangeBegin = dateToSearch.AddDays(-90),
                                StartRangeEnd = dateToSearch,
                                ParticipantCustomerId = member.iRacingMemberId,
                                EventTypes = new int[] { Db.EventTypes.Find(x => x.Label == "Race").Value }
                            })).Data;

                        mappedRaces.AddRange(
                            Mapper.Map<DataContext.Events.PublicEvents[]>(
                                races,
                                opts =>
                                    opts.AfterMap((src, dest) =>
                                    {
                                        for (int i = 0; i < dest.Length; i++)
                                        {
                                            dest[i].iRacingMemberId = member.iRacingMemberId;
                                        }
                                    }
                                    )
                            )
                        );

                        dateToSearch = dateToSearch.AddDays(-90);
                    }
                    catch(ArgumentNullException ex)
                    {
                        if (ex.Message.Equals("Value cannot be null. (Parameter 'uriString')"))
                        {
                            Logger.LogWarning("Value cannot be null. (Parameter 'uriString') -- can likely ignore as this just means we didn't get any search results");
                            dateToSearch = dateToSearch.AddDays(-90);
                        }
                        else
                            Logger.LogCritical(ex, ex.Message);
                    }
                    catch(Exception ex)
                    {
                        Logger.LogCritical(ex, ex.Message);
                        return;
                    }
                }

                
            }

            mappedRaces = mappedRaces.OrderBy(x => x.StartTime).ToList();

            Db.PublicEvents.Delete(predicate: null);
            Db.PublicEvents.BulkInsert(mappedRaces);
        }

        public async void UpdateCars()
        {
            iRApiCars.CarInfo[] rawCars = (await IRacingDataClient.GetCarsAsync()).Data;

            DataContext.CarInfo[] mappedCars = Mapper.Map<DataContext.CarInfo[]>(rawCars);

            Db.CarInfos.Delete(predicate: null);
            Db.CarInfos.BulkInsert(mappedCars);
        }

        public async void UpdateEventTypes()
        {
            iRApiConstants.EventType[] rawEventTypes = (await IRacingDataClient.GetEventTypesAsync()).Data;

            DataContext.EventType[] mappedEventTypes = Mapper.Map<DataContext.EventType[]>(rawEventTypes);

            Db.EventTypes.Delete(predicate: null);
            Db.EventTypes.BulkInsert(mappedEventTypes);
        }

        public async void UpdateTracks()
        {
            iRApiTracks.Track[] rawTracks = (await IRacingDataClient.GetTracksAsync()).Data;

            DataContext.Track[] mappedTracks = Mapper.Map<DataContext.Track[]>(rawTracks);

            Db.Tracks.Delete(predicate: null);
            Db.Tracks.BulkInsert(mappedTracks);
        }
    }
}
