using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aydsko.iRacingData.Results;

namespace RespoBot.Services.EventHandlers
{
    internal class SubSessionIdentifierIndexedEventHandlerService
    {
        private readonly ILogger<SubSessionIdentifierIndexedEventHandlerService> _logger;
        private readonly IDbContext _db;
        private readonly IMapper _mapper;
        private readonly RateLimitedIRacingApiClient _iRacing;

        public SubSessionIdentifierIndexedEventHandlerService(ILogger<SubSessionIdentifierIndexedEventHandlerService> logger, IDbContext db, IMapper mapper, RateLimitedIRacingApiClient iRacing)
        {
            _logger = logger;
            _db = db;
            _iRacing = iRacing;
            _mapper = mapper;
        }

        public async Task Run(object sender, EventArgs.SubSessionIdentifierIndexedEventArgs e)
        {
            _logger.LogDebug($"Event received.");

            List<DataContext.SubSessionResultsOfficial> results = new();

            if (!_iRacing.DataClient.IsLoggedIn)
            {
                await _iRacing.DataClient.LoginExternalAsync();
            }

            List<Task<iRApi.Common.DataResponse<iRApi.Results.SubSessionResult>>> subSessionTasks =
                e.SubSessionIdentifiers.Select(subSession =>
                    _iRacing.ExecuteAsync<iRApi.Results.SubSessionResult>(
                        () =>
                            _iRacing.DataClient.GetSubSessionResultAsync(subSession.Key, true)
                        )
                    ).ToList();

            await Task.WhenAll(subSessionTasks.ToArray<Task>());

            List<iRApi.Results.SubSessionResult> subSessionResponses = subSessionTasks.Select(x => x.Result.Data).OrderBy(x => x.SubSessionId).ToList();

            foreach (iRApi.Results.SubSessionResult subSessionResponse in subSessionResponses)
            {
                iRApi.Results.Result[] raceSessionResults = subSessionResponse.SessionResults.FirstOrDefault(x => x.SimSessionTypeName.Equals("Race"))?.Results;

                if (raceSessionResults == null)
                {
                    _logger.LogDebug($"Skipping subsession {subSessionResponse.SubSessionId}; no race results found.");
                    continue;
                }
                
                _logger.LogDebug($"Adding subsession {subSessionResponse.SubSessionId}.");
                
                // results.AddRange(
                //     e.SubSessionIdentifiers[subSessionResponse.SubSessionId].Select(
                //         memberId =>
                //             raceSessionResults?.FirstOrDefault(
                //                 x =>
                //                     (x.CustomerId.Equals(memberId)) ||
                //                     (
                //                         (x.DriverResults != null) &&
                //                         (x.DriverResults.Any(y => y.CustomerId.Equals(memberId)))
                //                     )
                //             )
                //         )
                //     );

                results.AddRange(from memberId in e.SubSessionIdentifiers[subSessionResponse.SubSessionId]
                    let memberResult = raceSessionResults.FirstOrDefault(x => (x.CustomerId.Equals(memberId)) || ((x.DriverResults != null) && (x.DriverResults.Any(y => y.CustomerId.Equals(memberId)))))
                    
                    select new DataContext.SubSessionResultsOfficial
                    {
                        SubSessionId = subSessionResponse.SubSessionId,
                        IRacingMemberId = memberId,
                        ClassName = subSessionResponse.CarClasses.FirstOrDefault(x => x.CarClassId.Equals(memberResult!.CarClassId))!.Name,
                        CarNumber = raceSessionResults
                            .Where(x => x.CarClassId.Equals(memberResult.CarClassId))
                            .OrderByDescending(x => x.OldIRating)
                            .ToList()
                            .FindIndex(x => x.CustomerId.Equals(memberId)) + 1,
                        QualifyPosition = memberResult!.StartingPosition,
                        FinishPosition = memberResult!.FinishPosition,
                        IRatingNew = memberResult!.NewIRating,
                        IRatingChange = memberResult!.NewIRating - memberResult!.OldIRating,
                        SafetyRatingNew = memberResult!.NewSafetyRating,
                        SafetyRatingChange = memberResult!.NewSafetyRating - memberResult!.OldSafetyRating,
                        IncidentPoints = memberResult.Incidents
                    });
            }

            int upserted;

            if (e.AreSubSessionsHosted)
            {
                
            }
            else
            {
                upserted = await _db.SubSessionResultsOfficial.BulkInsertAsync(results).ConfigureAwait(false);
            }
            
            _logger.LogDebug($"Event handled; {results.Count} {(e.AreSubSessionsHosted ? "hosted" : "official") } subsessions indexed.");
        }
    }
}