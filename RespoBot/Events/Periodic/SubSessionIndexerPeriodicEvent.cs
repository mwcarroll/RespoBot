// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Threading.Tasks;
// using Aydsko.iRacingData.Searches;
// using RespoBot.Helpers;
//
// namespace RespoBot.Events.Periodic
// {
//     internal class SubSessionIndexerPeriodicRespoBotTask : PeriodicRespoBotTask
//     {
//         private readonly ILogger<SubSessionIndexerPeriodicRespoBotTask> _logger;
//         private readonly IConfiguration _configuration;
//         private readonly IDbContext _db;
//         private readonly IMapper _mapper;
//         private readonly RateLimitedIRacingApiClient _iRacing;
//
//         private readonly EventHandlers.SubSessionIdentifierIndexedEventHandlerService _subSessionIdentifierIndexed;
//
//         public event EventHandler<EventArgs.SubSessionIdentifierIndexedEventArgs> SubSessionsIndexedEvent;
//
//         public SubSessionIndexerPeriodicRespoBotTask(ILogger<SubSessionIndexerPeriodicRespoBotTask> logger, IConfiguration configuration, IDbContext db, IMapper mapper, RateLimitedIRacingApiClient iRacing, EventHandlers.SubSessionIdentifierIndexedEventHandlerService subSessionIdentifierIndexed)
//             : base(60000)
//         {
//             _logger = logger;
//             _configuration = configuration;
//             _db = db;
//             _mapper = mapper;
//             _iRacing = iRacing;
//
//             _subSessionIdentifierIndexed = subSessionIdentifierIndexed;
//         }
//
//         public override Task Run()
//         {
//             SubSessionsIndexedEvent += (sender, e) =>
//             {
//                 _logger.LogDebug($"Event triggered.");
//                 _ = _subSessionIdentifierIndexed.Run(sender, e);
//             };
//             
//             return base.Run();
//         }
//
//         protected override async void Main()
//         {
//             Stopwatch timer = Stopwatch.StartNew();
//             List<DataContext.TrackedMember> members = (await _db.Members.FindAllAsync()).ToList();
//
//             int numberOfDaysToSearch = _configuration.GetValue<int>("RespoBot:Searches:NumberOfDaysToSearchPerRequest");
//             DateTime dateNow = DateTime.UtcNow;
//
//             Dictionary<int, int[]> subSessionIdentifiersOfficial = new();
//             Dictionary<int, int[]> subSessionIdentifiersHosted = new();
//
//             List<Task<iRApi.Common.DataResponse<(HostedResultsHeader, HostedResultItem[])>>> hostedResponseTasks = new();
//             List<Task<iRApi.Common.DataResponse<(OfficialSearchResultHeader, OfficialSearchResultItem[])>>> officialResponseTasks = new();
//
//             if (!_iRacing.DataClient.IsLoggedIn)
//             {
//                 await _iRacing.DataClient.LoginExternalAsync();
//             }
//
//             foreach (DataContext.TrackedMember member in members)
//             {
//                 DateTime dateIterator = dateNow;
//
//                 // DateTime? dateToGoBackTo = new[] { member.LastCheckedHosted, member.LastCheckedOfficial, member.MemberSince, dateNow.AddDays(-numberOfDaysToSearch) }.Min();
//                 
//                 DateTime? dateToGoBackTo = new[] { dateNow.AddDays(-numberOfDaysToSearch) }.Min();
//
//                 while (dateIterator > dateToGoBackTo)
//                 {
//                     DateTime startRangeBegin = (DateTime)((dateIterator.AddDays(-numberOfDaysToSearch) < member.MemberSince) ? member.MemberSince : dateIterator.AddDays(-15));
//                     DateTime startRangeEnd = dateIterator;
//
//                     // hostedResponseTasks.Add(
//                     //     _iRacing.ExecuteAsync<(iRApi.Searches.HostedResultsHeader, iRApi.Searches.HostedResultItem[])>(
//                     //         () => _iRacing.DataClient.SearchHostedResultsAsync(new iRApi.Searches.HostedSearchParameters()
//                     //         {
//                     //             StartRangeBegin = startRangeBegin,
//                     //             StartRangeEnd = startRangeEnd,
//                     //             ParticipantCustomerId = member.IRacingMemberId
//                     //         })
//                     //     )
//                     // );
//
//                     officialResponseTasks.Add(
//                     _iRacing.ExecuteAsync<(OfficialSearchResultHeader, OfficialSearchResultItem[])>(
//                             () => _iRacing.DataClient.SearchOfficialResultsAsync(new OfficialSearchParameters
//                             {
//                                 StartRangeBegin = startRangeBegin,
//                                 StartRangeEnd = startRangeEnd,
//                                 ParticipantCustomerId = member.IRacingMemberId,
//                                 EventTypes = new int[] { (int) iRApi.Common.EventType.Race }
//                             })
//                         )
//                     );
//
//                     dateIterator = startRangeBegin;
//                 }
//             }
//
//             // wait for all responses to return
//             await Task.WhenAll(hostedResponseTasks.ToArray<Task>().Union(officialResponseTasks.ToArray<Task>()));
//
//             List<(HostedResultsHeader, HostedResultItem[])> hostedResponses = hostedResponseTasks.Select(x => x.Result.Data).ToList();
//             List<(OfficialSearchResultHeader, OfficialSearchResultItem[])> officialResponses = officialResponseTasks.Select(x => x.Result.Data).ToList();
//
//             foreach ((HostedResultsHeader header, HostedResultItem[] items) in hostedResponses)
//             {
//                 if (!items.Any()) continue;
//
//                 foreach (HostedResultItem item in items)
//                 {
//                     if (header.Data.Params.ParticipantCustomerId == null) continue;
//                     
//                     // ignore sessions without a race
//                     if (item.RaceLaps.Equals(0)) continue;
//
//                     int participantCustomerId = (int)header.Data.Params.ParticipantCustomerId;
//
//                     if (subSessionIdentifiersHosted.ContainsKey(item.SubsessionId))
//                         subSessionIdentifiersHosted[item.SubsessionId] = subSessionIdentifiersHosted[item.SubsessionId].Append(participantCustomerId).ToArray();
//                     else
//                         subSessionIdentifiersHosted.Add(item.SubsessionId, new int[] { participantCustomerId });
//                 }
//             }
//             
//             List<HostedResultItem> hostedSessions = hostedResponses
//                 .SelectMany(x => x.Item2)
//                 .DistinctBy(x => x.SubsessionId)
//                 .ToList();
//             
//             _logger.LogDebug($"SubSessionIds: {subSessionIdentifiersHosted.Count}, OfficialSessions: {hostedSessions.Count}");
//
//             foreach ((OfficialSearchResultHeader header, OfficialSearchResultItem[] items) in officialResponses)
//             {
//                 if (!items.Any()) continue;
//
//                 foreach (OfficialSearchResultItem item in items)
//                 {
//                     if (header.Data.Params.ParticipantCustomerId == null) continue;
//
//                     int participantCustomerId = (int)header.Data.Params.ParticipantCustomerId;
//
//                     if (subSessionIdentifiersOfficial.ContainsKey(item.SubsessionId))
//                         subSessionIdentifiersOfficial[item.SubsessionId] = subSessionIdentifiersOfficial[item.SubsessionId].Append(participantCustomerId).ToArray();
//                     else
//                         subSessionIdentifiersOfficial.Add(item.SubsessionId, new int[] { participantCustomerId });
//                 }
//             }
//
//             List<OfficialSearchResultItem> officialSessions = officialResponses
//                 .SelectMany(x => x.Item2)
//                 .DistinctBy(x => x.SubsessionId)
//                 .ToList();
//             
//             _logger.LogDebug($"SubSessionIds: {subSessionIdentifiersOfficial.Count}, OfficialSessions: {officialSessions.Count}");
//
//             timer.Stop();
//             
//             // await _db.SubSessionsHosted.BulkInsertAsync(_mapper.Map<List<iRApi.Searches.HostedResultItem>, List<DataContext.SubSessionsHosted>>(hostedSessions));
//             await _db.SubSessionsOfficial.BulkUpdateAsync(_mapper.Map<List<OfficialSearchResultItem>, List<DataContext.SubSessionsOfficial>>(officialSessions));
//
//             _logger.LogDebug($"Sub-sessions Identified: {subSessionIdentifiersOfficial.Count + subSessionIdentifiersHosted.Count} Elapsed: { timer.Elapsed } Requests: { hostedResponses.Count + officialResponses.Count }");
//
//             SubSessionsIndexedEvent?.Invoke(this, 
//                 new EventArgs.SubSessionIdentifierIndexedEventArgs
//                 {
//                     SubSessionIdentifiers = subSessionIdentifiersOfficial,
//                     AreSubSessionsHosted = false
//                 });
//             SubSessionsIndexedEvent?.Invoke(this,
//                 new EventArgs.SubSessionIdentifierIndexedEventArgs
//                 {
//                     SubSessionIdentifiers = subSessionIdentifiersHosted,
//                     AreSubSessionsHosted = true
//                 });
//         }
//     }
// }
