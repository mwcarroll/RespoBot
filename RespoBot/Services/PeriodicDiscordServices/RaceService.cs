using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RespoBot.Services.PeriodicDiscordServices
{
    public class RaceService : PeriodicDiscordService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly IDbContext _db;

        private readonly DiscordSocketClient _discordClient;

        private readonly IMapper _mapper;

        private readonly iRApi.IDataClient _iRacingDataClient;

        private readonly RequestHandlerService _requestHandlerService;

        public RaceService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, DiscordSocketClient discordClient, IMapper mapper, iRApi.IDataClient iRacingDataClient, RequestHandlerService requestHandlerService)
            : base(configuration, logger, discordClient, nameof(RaceService))
        {
            _configuration = configuration;
            _logger = logger;

            _db = db;

            _discordClient = discordClient;

            _mapper = mapper;

            _iRacingDataClient = iRacingDataClient;

            _requestHandlerService = requestHandlerService;
        }

        public async void Run2()
        {
            int[] eventIdsToSearch = { (await _db.EventTypes.FindAsync(x => x.Label == "Race").ConfigureAwait(false))!.Value };

            IEnumerable<DataContext.Member> members = await _db.Members.FindAllAsync<DataContext.MemberInfo>(null, m => m.MemberInfo).ConfigureAwait(false);

            DateTime dateNow = DateTime.UtcNow;

            Guid hostedRequestGroup = Guid.NewGuid();
            Guid officalRequestGroup = Guid.NewGuid();
            Guid driverInfoRequestGroup = Guid.NewGuid();

            int expectedHostedRequests = members.Count();
            int expectedOfficialRequests = members.Count();

            foreach (DataContext.Member member in members)
            {
                await _requestHandlerService.AddRequest(
                        _iRacingDataClient.SearchHostedResultsAsync(new iRApi.Searches.HostedSearchParameters()
                        {
                            FinishRangeBegin = member.LastCheckedHosted,
                            FinishRangeEnd = dateNow,
                            ParticipantCustomerId = member.IRacingMemberId
                        }),
                        hostedRequestGroup,
                        expectedHostedRequests
                    );
                await _requestHandlerService.AddRequest(
                        _iRacingDataClient.SearchOfficialResultsAsync(new iRApi.Searches.OfficialSearchParameters
                        {
                            FinishRangeBegin = member.LastCheckedOfficial,
                            FinishRangeEnd = dateNow,
                            ParticipantCustomerId = member.IRacingMemberId,
                            EventTypes = eventIdsToSearch
                        }),
                        officalRequestGroup,
                        expectedOfficialRequests
                    );

                member.LastCheckedHosted = dateNow;
                member.LastCheckedOfficial = dateNow;
            }

            await _requestHandlerService.AddRequest(
                    _iRacingDataClient.GetDriverInfoAsync(
                        members.Select(x => x.IRacingMemberId).ToArray(),
                        true
                    ),
                    driverInfoRequestGroup,
                    1
                );

            List<Task<iRApi.Common.DataResponse<(iRApi.Searches.HostedResultsHeader Header, iRApi.Searches.HostedResultItem[] Items)>>> hostedResponses
                = _requestHandlerService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.HostedResultsHeader, iRApi.Searches.HostedResultItem[])>>(hostedRequestGroup);

            List<Task<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader Header, iRApi.Searches.OfficialSearchResultItem[] Items)>>> officialResponses
                = _requestHandlerService.GetResponses<iRApi.Common.DataResponse<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])>>(officalRequestGroup);

            List<Task<iRApi.Common.DataResponse<iRApi.Member.DriverInfo[]>>> driverInfoResponses
                = _requestHandlerService.GetResponses<iRApi.Common.DataResponse<iRApi.Member.DriverInfo[]>>(driverInfoRequestGroup);

            ConcurrentBag <DataContext.Events.Hosted.CarInfo> mappedCars = new();
            ConcurrentBag<DataContext.Events.HostedEvent> mappedHostedEvents = new();
            ConcurrentBag<DataContext.Events.OfficialEvent> mappedOfficialEvents = new();
            ConcurrentBag<DataContext.LicenseInfo> mappedLicenseInfos = new();

            Parallel.ForEach(
                hostedResponses,
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

                                    mappedHostedEvents.Add(_mapper.Map<DataContext.Events.HostedEvent>(
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

            Parallel.ForEach(
                officialResponses,
                response =>
                {
                    if (response.Result.Data.Items.Any())
                        Parallel.ForEach(
                            response.Result.Data.Items,
                            item => {
                                mappedOfficialEvents.Add(
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

            Parallel.ForEach(
                driverInfoResponses,
                response => { 
                    if(response.Result.Data != null && response.Result.Data.Length > 0)
                    {
                        Parallel.ForEach(
                            response.Result.Data,
                            item => {
                                Parallel.ForEach(
                                    item.Licenses,
                                    license => {
                                        mappedLicenseInfos.Add(
                                            _mapper.Map<DataContext.LicenseInfo>(
                                                license,
                                                opts =>
                                                {
                                                    opts.AfterMap((_, dest) =>
                                                    {
                                                        dest.IRacingMemberId = item.CustomerId;
                                                    });
                                                }
                                            ));
                                    });
                            });
                    }
                });

            List<DataContext.Events.Hosted.CarInfo> distinctMappedCarsList = mappedCars.DistinctBy(x => new { x.PrivateSessionId, x.CarId }).ToList();
            List<int> distinctMappedCarInfoIdentifiers = distinctMappedCarsList.Select(x => x.PrivateSessionId).ToList();

            IEnumerable<DataContext.Events.Hosted.CarInfo> carInfosInDb = await _db.HostedEventCarInfos.FindAllAsync(x => distinctMappedCarInfoIdentifiers.Contains(x.PrivateSessionId)).ConfigureAwait(false);

            distinctMappedCarsList.RemoveAll(x => carInfosInDb.Contains(x));

            if(distinctMappedCarsList.Any())
                await _db.HostedEventCarInfos.BulkInsertAsync(distinctMappedCarsList.OrderBy(x => new { x.PrivateSessionId, x.CarId })).ConfigureAwait(false);

            List<DataContext.Events.HostedEvent> distinctMappedHostedEvents = mappedHostedEvents.DistinctBy(x => new { x.PrivateSessionId, x.IRacingMemberId }).ToList();
            List<int> distinctMappedHostedEventIdentifiers = distinctMappedHostedEvents.Select(x => x.PrivateSessionId).ToList();

            IEnumerable<DataContext.Events.HostedEvent> hostedEventsInDb = await _db.HostedEvents.FindAllAsync(x => distinctMappedHostedEventIdentifiers.Contains(x.PrivateSessionId)).ConfigureAwait(false);

            distinctMappedHostedEvents.RemoveAll(x => hostedEventsInDb.Contains(x));

            if (distinctMappedHostedEvents.Any())
                await _db.HostedEvents.BulkInsertAsync(distinctMappedHostedEvents.OrderBy(x => x.StartTime)).ConfigureAwait(false);

            List<DataContext.Events.OfficialEvent> distinctMappedOfficialEvents = mappedOfficialEvents.DistinctBy(x => new { x.SessionId, x.SubsessionId, x.IRacingMemberId }).ToList();
            List<int> distinctMappedOfficialEventIdentifiers = distinctMappedOfficialEvents.Select(x => x.SubsessionId).ToList();

            IEnumerable<DataContext.Events.OfficialEvent> officialEventsInDb = await _db.OfficialEvents.FindAllAsync(x => distinctMappedOfficialEventIdentifiers.Contains(x.SubsessionId)).ConfigureAwait(false);

            distinctMappedOfficialEvents.RemoveAll(x => officialEventsInDb.Contains(x));

            if (distinctMappedHostedEvents.Any())
                await _db.OfficialEvents.BulkInsertAsync(distinctMappedOfficialEvents.OrderBy(x => x.StartTime)).ConfigureAwait(false);

            List<DataContext.LicenseInfo> distinctMappedLicenseInfos = mappedLicenseInfos.DistinctBy(x => new { x.IRacingMemberId, x.CategoryId }).ToList();
            List<int> distinctMappedLicenseInfosIdentifiers = distinctMappedLicenseInfos.Select(x => x.IRacingMemberId).ToList();

            IEnumerable<DataContext.LicenseInfo> licensesInfosInDb = await _db.LicenseInfos.FindAllAsync(x => distinctMappedLicenseInfosIdentifiers.Contains(x.IRacingMemberId)).ConfigureAwait(false);

            IEnumerable<DataContext.LicenseInfo> updatedLicenseInfos = licensesInfosInDb.Join(
                distinctMappedLicenseInfos,
                a => new { a.IRacingMemberId, a.CategoryId },
                b => new { b.IRacingMemberId, b.CategoryId },
                (a, b) => new DataContext.LicenseInfo
                {
                    Id = a.Id,
                    IRacingMemberId = a.IRacingMemberId,
                    Category = a.Category,
                    CategoryId = a.CategoryId,
                    LicenseLevel = b.LicenseLevel,
                    SafetyRating = b.SafetyRating,
                    Color = b.Color,
                    GroupName = b.GroupName,
                    GroupId = b.GroupId,
                    CornersPerIncident = b.CornersPerIncident,
                    IRating = b.IRating,
                    TtRating = b.TtRating,
                    MprNumberOfRaces = b.MprNumberOfRaces,
                    MprNumberOfTimeTrials = b.MprNumberOfTimeTrials
                });

            IEnumerable<DataContext.LicenseInfo> missingLicenseInfos = distinctMappedLicenseInfos.Except(updatedLicenseInfos);
            updatedLicenseInfos = updatedLicenseInfos.Except(licensesInfosInDb);

            if (updatedLicenseInfos.Any())
                await _db.LicenseInfos.BulkUpdateAsync(updatedLicenseInfos).ConfigureAwait(false);            

            if (missingLicenseInfos.Any())
                await _db.LicenseInfos.BulkInsertAsync(missingLicenseInfos).ConfigureAwait(false);

            await _db.Members.BulkUpdateAsync(members).ConfigureAwait(false);
        }
    }
}
