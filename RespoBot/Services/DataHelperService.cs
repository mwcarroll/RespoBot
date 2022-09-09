using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Reflection.PortableExecutable;

using Aydsko.iRacingData;
using iRApiCars = Aydsko.iRacingData.Cars;
using iRApiCommon = Aydsko.iRacingData.Common;
using iRApiConstants = Aydsko.iRacingData.Constants;
using iRApiLookups = Aydsko.iRacingData.Lookups;
using iRApiSearches = Aydsko.iRacingData.Searches;
using iRApiTracks = Aydsko.iRacingData.Tracks;

using DataContext = RespoBot.Data.Classes;
using RespoBot.Data.DbContexts;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Discord.Net;

namespace RespoBot.Services
{
    public class DataHelperService
    {
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
            try
            {
                ConcurrentBag<DataContext.Events.PublicEvents> mappedRaces = new();

                int[] eventIdsToSearch = new int[] { Db.EventTypes.Find(x => x.Label == "Race").Value };

                IEnumerable<DataContext.Member> members = Db.Members.FindAll<DataContext.MemberInfo>(null, p => p.MemberInfo);

                await IRacingDataClient.GetMyInfoAsync();
                iRApiCommon.DataResponse<iRApiLookups.LookupGroup[]> initial = await IRacingDataClient.GetLookupsAsync();
                DateTimeOffset rateLimitReset = (DateTimeOffset)initial.RateLimitReset;
                int totalRateLimit = (int)initial.TotalRateLimit;

                List<Func<Task<iRApiCommon.DataResponse<(iRApiSearches.OfficialSearchResultHeader Header, iRApiSearches.OfficialSearchResultItem[] Items)>>>> searchTasks = new();
                
                foreach(DataContext.Member member in members)
                {
                    DateTime iterator = DateTime.UtcNow;

                    while(iterator > member.MemberInfo.MemberSince)
                    {
                        DateTime newIterator = (iterator.AddDays(-90) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : iterator.AddDays(-90);

                        // we need "local" copies of the iterator and member due to lambda usage
                        DateTime localIterator = iterator;
                        DataContext.Member localMember = member;

                        searchTasks.Add(() => Task.Run(() =>
                        {
                            return IRacingDataClient.SearchOfficialResultsAsync(new iRApiSearches.OfficialSearchParameters()
                                {
                                    StartRangeBegin = newIterator,
                                    StartRangeEnd = localIterator,
                                    ParticipantCustomerId = member.iRacingMemberId,
                                    EventTypes = eventIdsToSearch
                                });

                        }));

                        iterator = newIterator;
                    }
                }

                // TODO: throttle task invoke, within the rate limit values
                var responses = await Task.WhenAll(searchTasks.Select(async task => await task.Invoke()));

                foreach (iRApiCommon.DataResponse<(iRApiSearches.OfficialSearchResultHeader Header, iRApiSearches.OfficialSearchResultItem[] Items)> response in responses)
                {
                    if(response.Data.Items.Any())
                        foreach(iRApiSearches.OfficialSearchResultItem item in response.Data.Items)
                        {
                            mappedRaces
                                .Add(
                                    Mapper.Map<DataContext.Events.PublicEvents>(
                                        item,
                                        opts =>
                                        {
                                            opts.AfterMap((src, dest) =>
                                            {
                                                dest.iRacingMemberId = (int)response.Data.Header.Data.Params.ParticipantCustomerId;
                                            });
                                        }
                                    )
                                );
                        }
                }

                Db.PublicEvents.Delete(predicate: null);
                Db.PublicEvents.BulkInsert(mappedRaces.OrderBy(x => x.StartTime).ToList());
                Db.Members.BulkUpdate(members);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, ex.Message);
                return;
            }
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
