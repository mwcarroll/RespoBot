using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using iRApi = Aydsko.iRacingData;

using DataContext = RespoBot.Data.Classes;
using RespoBot.Data.DbContexts;
using RespoBot.Services.PeriodicServices;

namespace RespoBot.Services
{
    public class DataHelperService
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly IDbContext Db;

        private readonly IMapper Mapper;

        private readonly iRApi.IDataClient IRacingDataClient;

        private readonly RateLimitService RateLimitService;

        public DataHelperService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, iRApi.IDataClient iRacingDataClient, RateLimitService rateLimitService)
        {
            Configuration = configuration;
            Logger = logger;

            Db = db;

            Mapper = mapper;

            IRacingDataClient = iRacingDataClient;

            RateLimitService = rateLimitService;
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
                int[] eventIdsToSearch = new int[] { Db.EventTypes.Find(x => x.Label == "Race").Value };
                IEnumerable<DataContext.Member> members = Db.Members.FindAll<DataContext.MemberInfo>(null, p => p.MemberInfo);
                DateTime dateNow = DateTime.UtcNow;
                int expectedRequests = (int) members.Sum(member => Math.Ceiling((dateNow - member.MemberInfo.MemberSince).TotalDays / 90));

                Guid requestGroup = Guid.NewGuid();
                ConcurrentBag<DataContext.Events.PublicEvents> mappedRaces = new();

                foreach (DataContext.Member member in members)
                {
                    DateTime dateIterator = dateNow;

                    while(dateIterator > member.MemberInfo.MemberSince)
                    {
                        await RateLimitService.AddRequest(
                                IRacingDataClient.SearchOfficialResultsAsync(new iRApi.Searches.OfficialSearchParameters()
                                {
                                    StartRangeBegin = (dateIterator.AddDays(-90) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-90),
                                    StartRangeEnd = dateIterator,
                                    ParticipantCustomerId = member.iRacingMemberId,
                                    EventTypes = eventIdsToSearch
                                }),
                                requestGroup,
                                expectedRequests
                            );

                        dateIterator = (dateIterator.AddDays(-90) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-90);
                    }

                    member.LastChecked = dateNow;
                }

                List<Task<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader Header, iRApi.Searches.OfficialSearchResultItem[] Items)>>> responses =
                    RateLimitService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])>> (requestGroup);

                Parallel.ForEach(
                    responses,
                    (response) =>
                    {
                        if (response.Result.Data.Items.Any())
                            Parallel.ForEach(
                                response.Result.Data.Items,
                                (item) => {
                                    mappedRaces.Add(
                                        Mapper.Map<DataContext.Events.PublicEvents>(
                                            item,
                                            (opts) => {
                                                opts.AfterMap((src, dest) =>
                                                {
                                                    dest.iRacingMemberId = (int)response.Result.Data.Header.Data.Params.ParticipantCustomerId;
                                                });
                                            }
                                        ));
                                });
                    });

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
            iRApi.Cars.CarInfo[] rawCars = (await IRacingDataClient.GetCarsAsync()).Data;

            DataContext.CarInfo[] mappedCars = Mapper.Map<DataContext.CarInfo[]>(rawCars);

            Db.CarInfos.Delete(predicate: null);
            Db.CarInfos.BulkInsert(mappedCars);
        }

        public async void UpdateEventTypes()
        {
            iRApi.Constants.EventType[] rawEventTypes = (await IRacingDataClient.GetEventTypesAsync()).Data;

            DataContext.EventType[] mappedEventTypes = Mapper.Map<DataContext.EventType[]>(rawEventTypes);

            Db.EventTypes.Delete(predicate: null);
            Db.EventTypes.BulkInsert(mappedEventTypes);
        }

        public async void UpdateTracks()
        {
            iRApi.Tracks.Track[] rawTracks = (await IRacingDataClient.GetTracksAsync()).Data;

            DataContext.Track[] mappedTracks = Mapper.Map<DataContext.Track[]>(rawTracks);

            Db.Tracks.Delete(predicate: null);
            Db.Tracks.BulkInsert(mappedTracks);
        }
    }
}
