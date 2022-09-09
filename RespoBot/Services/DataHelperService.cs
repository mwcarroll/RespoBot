using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RespoBot.Services
{
    public class DataHelperService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly IDbContext _db;

        private readonly IMapper _mapper;

        private readonly iRApi.IDataClient _racingDataClient;

        private readonly RequestHandlerService _requestHandlerService;

        public DataHelperService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, iRApi.IDataClient iRacingDataClient, RequestHandlerService requestHandlerService)
        {
            _configuration = configuration;
            _logger = logger;

            _db = db;

            _mapper = mapper;

            _racingDataClient = iRacingDataClient;

            _requestHandlerService = requestHandlerService;
        }

        public void Run()
        {
            // UpdateEventTypes();
            // UpdateTracks();
            // UpdateCars();
            RepopulatePublicRaces();
        }

        private async void RepopulatePublicRaces()
        {
            try
            {
                int[] eventIdsToSearch = { (await _db.EventTypes.FindAsync(x => x.Label == "Race"))!.Value };
                IEnumerable<DataContext.Member> members = await _db.Members.FindAllAsync<DataContext.MemberInfo>(null, p => p.MemberInfo);
                DateTime dateNow = DateTime.UtcNow;
                int expectedRequests = (int) members.Sum(member => Math.Ceiling((dateNow - member.MemberInfo.MemberSince).TotalDays / 90));

                Guid requestGroup = Guid.NewGuid();
                ConcurrentBag<DataContext.Events.PublicEvents> mappedRaces = new();

                foreach (DataContext.Member member in members)
                {
                    DateTime dateIterator = dateNow;

                    while(dateIterator > member.MemberInfo.MemberSince)
                    {
                        await _requestHandlerService.AddRequest(
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
                    _requestHandlerService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])>> (requestGroup);

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
                                                opts.AfterMap((_, dest) =>
                                                {
                                                    if (response.Result.Data.Header.Data.Params.ParticipantCustomerId != null)
                                                        dest.IRacingMemberId = (int)response.Result.Data.Header.Data.Params.ParticipantCustomerId;
                                                    else
                                                        throw new iRApi.Exceptions.iRacingDataClientException("Participant Customer Id null");
                                                });
                                            }
                                        ));
                                });
                    });

                await _db.PublicEvents.DeleteAsync(predicate: null);
                await _db.PublicEvents.BulkInsertAsync(mappedRaces.OrderBy(x => x.StartTime).ToList());
                await _db.Members.BulkUpdateAsync(members);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
            }
        }

        public async void UpdateCars()
        {
            iRApi.Cars.CarInfo[] rawCars = (await _racingDataClient.GetCarsAsync()).Data;

            DataContext.CarInfo[] mappedCars = _mapper.Map<DataContext.CarInfo[]>(rawCars);

            await _db.CarInfos.DeleteAsync(predicate: null);
            await _db.CarInfos.BulkInsertAsync(mappedCars);
        }

        public async void UpdateEventTypes()
        {
            iRApi.Constants.EventType[] rawEventTypes = (await _racingDataClient.GetEventTypesAsync()).Data;

            DataContext.EventType[] mappedEventTypes = _mapper.Map<DataContext.EventType[]>(rawEventTypes);

            await _db.EventTypes.DeleteAsync(predicate: null);
            await _db.EventTypes.BulkInsertAsync(mappedEventTypes);
        }

        public async void UpdateTracks()
        {
            iRApi.Tracks.Track[] rawTracks = (await _racingDataClient.GetTracksAsync()).Data;

            DataContext.Track[] mappedTracks = _mapper.Map<DataContext.Track[]>(rawTracks);

            await _db.Tracks.DeleteAsync(predicate: null);
            await _db.Tracks.BulkInsertAsync(mappedTracks);
        }
    }
}
