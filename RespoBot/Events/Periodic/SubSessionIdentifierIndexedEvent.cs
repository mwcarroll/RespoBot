using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RespoBot.Helpers;

namespace RespoBot.Events.Periodic
{
    internal class SubSessionIdentifierIndexedEvent(
        ILogger<SubSessionIdentifierIndexedEvent> logger,
        IDbContext db,
        IMapper mapper,
        RateLimitedIRacingApiClient iRacing)
    {
        private readonly IMapper _mapper = mapper;

        public async Task Run(object sender, Events.Args.SubSessionIdentifierIndexedEventArgs e)
        {
            logger.LogDebug($"Event received.");

            List<DataContext.SubSessionResultsOfficial> results = [];

            if (!iRacing.DataClient.IsLoggedIn)
            {
                await iRacing.DataClient.LoginExternalAsync();
            }

            List<Task<iRApi.Common.DataResponse<iRApi.Results.SubSessionResult>>> subSessionTasks =
                e.SubSessionIdentifiers.Select(subSession =>
                    iRacing.ExecuteAsync<iRApi.Results.SubSessionResult>(
                        () =>
                            iRacing.DataClient.GetSubSessionResultAsync(subSession.Key, true)
                        )
                    ).ToList();

            await Task.WhenAll(subSessionTasks.ToArray<Task>());

            List<iRApi.Results.SubSessionResult> subSessionResponses = subSessionTasks.Select(x => x.Result.Data).OrderBy(x => x.SubSessionId).ToList();

            foreach (iRApi.Results.SubSessionResult subSessionResponse in subSessionResponses)
            {
                iRApi.Results.Result[] raceSessionResults = subSessionResponse.SessionResults.FirstOrDefault(x => x.SimSessionTypeName.Equals("Race"))?.Results;

                if (raceSessionResults == null)
                {
                    logger.LogDebug($"Skipping subsession {subSessionResponse.SubSessionId}; no race results found.");
                    continue;
                }
                
                logger.LogDebug($"Adding subsession {subSessionResponse.SubSessionId}.");
                
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
                upserted = await db.SubSessionResultsOfficial.BulkInsertAsync(results).ConfigureAwait(false);
            }
            
            logger.LogDebug($"Event handled; {results.Count} {(e.AreSubSessionsHosted ? "hosted" : "official") } subsessions indexed.");
        }
    }
}