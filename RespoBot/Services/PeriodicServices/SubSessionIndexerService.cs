using RespoBot.Helpers;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace RespoBot.Services.PeriodicServices
{
    public class SubSessionIndexerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaskQueueService> _logger;
        private readonly TaskQueueService _taskQueueService;
        private readonly IDbContext _db;
        private readonly iRApi.IDataClient _iRacingDataClient;

        public SubSessionIndexerService(IConfiguration configuration, ILogger<TaskQueueService> logger, TaskQueueService taskQueueService, IDbContext db, iRApi.IDataClient iRacingDataClient)
        {
            _configuration = configuration;
            _logger = logger;
            _taskQueueService = taskQueueService;
            _db = db;
            _iRacingDataClient = iRacingDataClient;
        }

        public void Run()
        {
            Task.Run(() => { RunSubSessionIndexer(); });

            //PeriodicTask.Run(() => { RunSubSessionIndexer(); }, TimeSpan.FromMinutes(1));
        }

        private async void RunSubSessionIndexer()
        {
            List<DataContext.Member> members = _db.Members.FindAll().ToList();

            int numberOfDaysToSearch = _configuration.GetValue<int>("RespoBot:Searches:NumberOfDaysToSearchPerRequest");
            DateTime dateNow = DateTime.UtcNow;

            Guid hostedRequestGroup = Guid.NewGuid();
            Guid officialRequestGroup = Guid.NewGuid();

            Dictionary<int, int[]> subSessionIdentifiers = new();

            foreach (var member in members)
            {
                DateTime dateIterator = dateNow;

                while (dateIterator > member.MemberSince)
                {
                    DateTime startRangeBegin = (DateTime)((dateIterator.AddDays(-numberOfDaysToSearch) < member.MemberSince) ? member.MemberSince : dateIterator.AddDays(-numberOfDaysToSearch));
                    DateTime startRangeEnd = dateIterator;                    

                    _taskQueueService.QueueRequest(
                            () =>
                            {
                                _logger.LogDebug($"from {startRangeBegin.ToString("yyyy-MM-dd")} to {startRangeEnd.ToString("yyyy-MM-dd")}");

                                return _iRacingDataClient.SearchHostedResultsAsync(new iRApi.Searches.HostedSearchParameters()
                                {
                                    StartRangeBegin = startRangeBegin,
                                    StartRangeEnd = startRangeEnd,
                                    ParticipantCustomerId = member.IRacingMemberId
                                });
                            },
                            hostedRequestGroup
                        );

                    _taskQueueService.QueueRequest(
                            () =>
                            {
                                return _iRacingDataClient.SearchOfficialResultsAsync(new iRApi.Searches.OfficialSearchParameters
                                {
                                    StartRangeBegin = startRangeBegin,
                                    StartRangeEnd = startRangeEnd,
                                    ParticipantCustomerId = member.IRacingMemberId,
                                    EventTypes = new int[] { 5 }
                                });
                            },
                            officialRequestGroup
                        );

                    dateIterator = startRangeBegin;
                }
            }

            List<(iRApi.Searches.HostedResultsHeader, iRApi.Searches.HostedResultItem[])> hostedResponses;
            List<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])> officialResponses;

            try
            {
                hostedResponses = _taskQueueService.GetResponses<(iRApi.Searches.HostedResultsHeader, iRApi.Searches.HostedResultItem[])>(hostedRequestGroup).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                hostedResponses = null;
            }

            try
            {
                officialResponses = _taskQueueService.GetResponses<(iRApi.Searches.OfficialSearchResultHeader, iRApi.Searches.OfficialSearchResultItem[])>(officialRequestGroup).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
                officialResponses = null;
            }

            foreach((iRApi.Searches.HostedResultsHeader Header, iRApi.Searches.HostedResultItem[] Items) response in hostedResponses)
            {
                if (response.Items.Any())
                {
                    foreach (iRApi.Searches.HostedResultItem item in response.Items)
                    {
                        int participantCustomerId = (int)response.Header.Data.Params.ParticipantCustomerId;

                        if (subSessionIdentifiers.ContainsKey(item.SubsessionId))
                            subSessionIdentifiers[item.SubsessionId] = subSessionIdentifiers[item.SubsessionId].Append(participantCustomerId).ToArray();
                        else
                            subSessionIdentifiers.Add(item.SubsessionId, new int[] { participantCustomerId });
                    }
                }
            }

            foreach ((iRApi.Searches.OfficialSearchResultHeader Header, iRApi.Searches.OfficialSearchResultItem[] Items) response in officialResponses)
            {
                if (response.Items.Any())
                {
                    foreach (iRApi.Searches.OfficialSearchResultItem item in response.Items)
                    {
                        int participantCustomerId = (int)response.Header.Data.Params.ParticipantCustomerId;

                        if (subSessionIdentifiers.ContainsKey(item.SubsessionId))
                            subSessionIdentifiers[item.SubsessionId] = subSessionIdentifiers[item.SubsessionId].Append(participantCustomerId).ToArray();
                        else
                            subSessionIdentifiers.Add(item.SubsessionId, new int[] { participantCustomerId });
                    }
                }
            }
        }
    }
}
