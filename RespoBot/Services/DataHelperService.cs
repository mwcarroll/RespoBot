using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using RespoBot.Services.PeriodicServices;

namespace RespoBot.Services
{
    public class DataHelperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly IDbContext _db;

        private readonly IMapper _mapper;

        private readonly iRApi.IDataClient _racingDataClient;

        private readonly RateLimitService _rateLimitService;

        public DataHelperService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, iRApi.IDataClient iRacingDataClient, RateLimitService rateLimitService)
        {
            _configuration = configuration;
            _logger = logger;

            _db = db;

            _mapper = mapper;

            _racingDataClient = iRacingDataClient;

            _rateLimitService = rateLimitService;
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
                int[] eventIdsToSearch = new int[] { _db.EventTypes.Find(x => x.Label == "Race").Value };
                IEnumerable<DataContext.Member> members = _db.Members.FindAll<DataContext.MemberInfo>(null, p => p.MemberInfo);
                DateTime dateNow = DateTime.UtcNow;
                int expectedRequests = (int) members.Sum(member => Math.Ceiling((dateNow - member.MemberInfo.MemberSince).TotalDays / 90));

                Guid requestGroup = Guid.NewGuid();
                ConcurrentBag<DataContext.Events.PublicEvents> mappedRaces = new();

                foreach (DataContext.Member member in members)
                {
                    DateTime dateIterator = dateNow;

                    while(dateIterator > member.MemberInfo.MemberSince)
                    {
                        await _rateLimitService.AddRequest(
                                _racingDataClient.SearchOfficialResultsAsync(new iRApi.Searches.OfficialSearchParameters()
                                {
                                    StartRangeBegin = (dateIterator.AddDays(-90) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-90),
                                    StartRangeEnd = dateIterator,
                                    ParticipantCustomerId = member.IRacingMemberId,
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
                    _rateLimitService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])>> (requestGroup);

                Parallel.ForEach(
                    responses,
                    (response) =>
                    {
                        if (response.Result.Data.Items.Any())
                            Parallel.ForEach(
                                response.Result.Data.Items,
                                (item) => {
                                    mappedRaces.Add(
                                        _mapper.Map<DataContext.Events.PublicEvents>(
                                            item,
                                            (opts) => {
                                                opts.AfterMap((src, dest) =>
                                                {
                                                    dest.IRacingMemberId = (int)response.Result.Data.Header.Data.Params.ParticipantCustomerId;
                                                });
                                            }
                                        ));
                                });
                    });

                _db.PublicEvents.Delete(predicate: null);
                _db.PublicEvents.BulkInsert(mappedRaces.OrderBy(x => x.StartTime).ToList());
                _db.Members.BulkUpdate(members);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                return;
            }
        }

        public async void UpdateCars()
        {
            iRApi.Cars.CarInfo[] rawCars = (await _racingDataClient.GetCarsAsync()).Data;

            DataContext.CarInfo[] mappedCars = _mapper.Map<DataContext.CarInfo[]>(rawCars);

            _db.CarInfos.Delete(predicate: null);
            _db.CarInfos.BulkInsert(mappedCars);
        }

        public async void UpdateEventTypes()
        {
            iRApi.Constants.EventType[] rawEventTypes = (await _racingDataClient.GetEventTypesAsync()).Data;

            DataContext.EventType[] mappedEventTypes = _mapper.Map<DataContext.EventType[]>(rawEventTypes);

            _db.EventTypes.Delete(predicate: null);
            _db.EventTypes.BulkInsert(mappedEventTypes);
        }

        public async void UpdateTracks()
        {
            iRApi.Tracks.Track[] rawTracks = (await _racingDataClient.GetTracksAsync()).Data;

            DataContext.Track[] mappedTracks = _mapper.Map<DataContext.Track[]>(rawTracks);

            _db.Tracks.Delete(predicate: null);
            _db.Tracks.BulkInsert(mappedTracks);
        }
    }
}
