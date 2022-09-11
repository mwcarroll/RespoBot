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

        private readonly iRApi.IDataClient _iRacingDataClient;

        private readonly RequestHandlerService _requestHandlerService;

        public DataHelperService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, iRApi.IDataClient iIRacingDataClient, RequestHandlerService requestHandlerService)
        {
            _configuration = configuration;
            _logger = logger;

            _db = db;

            _mapper = mapper;

            _iRacingDataClient = iIRacingDataClient;

            _requestHandlerService = requestHandlerService;
        }

        public void Run()
        {
            //UpdateEventTypes();
            //UpdateTracks();
            //UpdateCars();
            //RepopulatePublicRaces();
            //RepopulateHostedRaces();
        }

        private async void RepopulateHostedRaces()
        {
            try
            {
                int[] eventIdsToSearch = { (await _db.EventTypes.FindAsync(x => x.Label == "Race").ConfigureAwait(false))!.Value };
                int numberOfDaysToSearch = _configuration.GetValue<int>("RespoBot:Searches:NumberOfDaysToSearchPerRequest");
                
                IEnumerable<DataContext.Member> members = await _db.Members.FindAllAsync<DataContext.MemberInfo>(null, p => p.MemberInfo).ConfigureAwait(false);

                DateTime dateNow = DateTime.UtcNow;

                int expectedRequests = (int) members.Sum(member => Math.Ceiling((dateNow - member.MemberInfo.MemberSince).TotalDays / numberOfDaysToSearch));

                Guid requestGroup = Guid.NewGuid();
                ConcurrentBag<DataContext.Events.HostedEvent> mappedRaces = new();
                ConcurrentBag<DataContext.Events.Hosted.CarInfo> mappedCars = new();

                foreach (DataContext.Member member in members)
                {
                    DateTime dateIterator = dateNow;

                    while (dateIterator > member.MemberInfo.MemberSince)
                    {
                        await _requestHandlerService.AddRequest(
                                _iRacingDataClient.SearchHostedResultsAsync(new iRApi.Searches.HostedSearchParameters()
                                {
                                    StartRangeBegin = (dateIterator.AddDays(-numberOfDaysToSearch) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-numberOfDaysToSearch),
                                    StartRangeEnd = dateIterator,
                                    ParticipantCustomerId = member.IRacingMemberId
                                }),
                                requestGroup,
                                expectedRequests
                            ).ConfigureAwait(false);

                        dateIterator = (dateIterator.AddDays(-numberOfDaysToSearch) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-numberOfDaysToSearch);
                    }

                    member.LastCheckedHosted = dateNow;
                }

                List<Task<iRApi.Common.DataResponse<(iRApi.Searches.HostedResultsHeader Header, iRApi.Searches.HostedResultItem[] Items)>>> responses =
                    _requestHandlerService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.HostedResultsHeader, iRApi.Searches.HostedResultItem[])>>(requestGroup);

                Parallel.ForEach(
                    responses,
                    response =>
                    {
                        if (response.Result.Data.Items.Any())
                            Parallel.ForEach(
                                response.Result.Data.Items,
                                item =>
                                {
                                    // we only care about hosted events that were races
                                    if (item.RaceLaps > 0 || item.RaceLength > 0)
                                    {
                                        if (item.Cars.Any())
                                            Parallel.ForEach(
                                                item.Cars,
                                                car =>
                                                {
                                                    mappedCars.Add(
                                                        _mapper.Map<DataContext.Events.Hosted.CarInfo>(
                                                            car,
                                                            opts =>
                                                            {
                                                                opts.AfterMap((_, dest) =>
                                                                {
                                                                    dest.PrivateSessionId = item.PrivateSessionId;
                                                                });
                                                            }
                                                        ));
                                                });

                                        mappedRaces.Add(_mapper.Map<DataContext.Events.HostedEvent>(
                                                item,
                                                opts =>
                                                {
                                                    opts.AfterMap((_, dest) =>
                                                    {
                                                        if (response.Result.Data.Header.Data.Params.ParticipantCustomerId != null)
                                                            dest.IRacingMemberId = (int)response.Result.Data.Header.Data.Params.ParticipantCustomerId;
                                                        else
                                                            throw new iRApi.Exceptions.iRacingDataClientException("Participant Customer Id null");
                                                    });
                                                }
                                            ));
                                    }
                                });
                    });

                await _db.HostedEventCarInfos.DeleteAsync(predicate: null).ConfigureAwait(false);
                await _db.HostedEventCarInfos.BulkInsertAsync(mappedCars.OrderBy(x => x.PrivateSessionId).ToList()).ConfigureAwait(false);

                await _db.HostedEvents.DeleteAsync(predicate: null).ConfigureAwait(false);
                await _db.HostedEvents.BulkInsertAsync(mappedRaces).ConfigureAwait(false);

                await _db.Members.BulkUpdateAsync(members).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
            }
        }

        private async void RepopulatePublicRaces()
        {
            try
            {
                int[] eventIdsToSearch = { (await _db.EventTypes.FindAsync(x => x.Label == "Race").ConfigureAwait(false))!.Value };
                int numberOfDaysToSearch = _configuration.GetValue<int>("RespoBot:Searches:NumberOfDaysToSearchPerRequest");

                IEnumerable<DataContext.Member> members = await _db.Members.FindAllAsync<DataContext.MemberInfo>(null, p => p.MemberInfo).ConfigureAwait(false);

                DateTime dateNow = DateTime.UtcNow;

                int expectedRequests = (int)members.Sum(member => Math.Ceiling((dateNow - member.MemberInfo.MemberSince).TotalDays / numberOfDaysToSearch));

                Guid requestGroup = Guid.NewGuid();
                ConcurrentBag<DataContext.Events.OfficialEvent> mappedRaces = new();

                foreach (DataContext.Member member in members)
                {
                    DateTime dateIterator = dateNow;

                    while (dateIterator > member.MemberInfo.MemberSince)
                    {
                        await _requestHandlerService.AddRequest(
                                _iRacingDataClient.SearchOfficialResultsAsync(new iRApi.Searches.OfficialSearchParameters
                                {
                                    StartRangeBegin = (dateIterator.AddDays(-numberOfDaysToSearch) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-numberOfDaysToSearch),
                                    StartRangeEnd = dateIterator,
                                    ParticipantCustomerId = member.IRacingMemberId,
                                    EventTypes = eventIdsToSearch
                                }),
                                requestGroup,
                                expectedRequests
                            );

                        dateIterator = (dateIterator.AddDays(-numberOfDaysToSearch) < member.MemberInfo.MemberSince) ? member.MemberInfo.MemberSince : dateIterator.AddDays(-numberOfDaysToSearch);
                    }

                    member.LastCheckedOfficial = dateNow;
                }

                List<Task<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader Header, iRApi.Searches.OfficialSearchResultItem[] Items)>>> responses =
                    _requestHandlerService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])>>(requestGroup);

                Parallel.ForEach(
                    responses,
                    response =>
                    {
                        if (response.Result.Data.Items.Any())
                            Parallel.ForEach(
                                response.Result.Data.Items,
                                item => {
                                    mappedRaces.Add(
                                        _mapper.Map<DataContext.Events.OfficialEvent>(
                                            item,
                                            opts => {
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

                await _db.OfficialEvents.DeleteAsync(predicate: null).ConfigureAwait(false);
                await _db.OfficialEvents.BulkInsertAsync(mappedRaces.OrderBy(x => x.StartTime).ToList()).ConfigureAwait(false);

                await _db.Members.BulkUpdateAsync(members).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
            }
        }

        private async void UpdateCars()
        {
            iRApi.Cars.CarInfo[] rawCars = (await _iRacingDataClient.GetCarsAsync()).Data;

            DataContext.CarInfo[] mappedCars = _mapper.Map<DataContext.CarInfo[]>(rawCars);

            await _db.CarInfos.DeleteAsync(predicate: null);
            await _db.CarInfos.BulkInsertAsync(mappedCars);
        }

        private async void UpdateEventTypes()
        {
            iRApi.Constants.EventType[] rawEventTypes = (await _iRacingDataClient.GetEventTypesAsync()).Data;

            DataContext.EventType[] mappedEventTypes = _mapper.Map<DataContext.EventType[]>(rawEventTypes);

            await _db.EventTypes.DeleteAsync(predicate: null);
            await _db.EventTypes.BulkInsertAsync(mappedEventTypes);
        }

        private async void UpdateTracks()
        {
            iRApi.Tracks.Track[] rawTracks = (await _iRacingDataClient.GetTracksAsync()).Data;

            DataContext.Track[] mappedTracks = _mapper.Map<DataContext.Track[]>(rawTracks);

            await _db.Tracks.DeleteAsync(predicate: null);
            await _db.Tracks.BulkInsertAsync(mappedTracks);
        }
    }
}
