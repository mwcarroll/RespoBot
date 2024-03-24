using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using RespoBot.Helpers;
using SkiaSharp;

namespace RespoBot.Tasks.Periodic
{
    internal class MemberChartInfoPeriodicRespoBotTask : PeriodicRespoBotTask
    {
        private readonly ILogger<MemberChartInfoPeriodicRespoBotTask> _logger;
        private readonly IDbContext _db;
        private readonly RateLimitedIRacingApiClient _iRacing;

        private readonly EventHandlers.MemberInfoUpdatedEventHandlerService _memberInfoUpdated;

        public event EventHandler<EventArgs.MemberInfoUpdatedEventArgs> MemberInfoUpdated;
        
        private const int PlebLine = 2500;

        private readonly Dictionary<int, SKColor> _userColors = new Dictionary<int, SKColor>()
        {
            { 336569, SKColor.Parse("#f1c40f") },
            { 369469, SKColor.Parse("#d10000") },
            { 386110, SKColor.Parse("#00AAFF") },
            { 386413, SKColor.Parse("#2ecc71") },
            { 401371, SKColor.Parse("#3c79a1") },
            { 412407, SKColor.Parse("#FFFFFF") },
            { 438221, SKColor.Parse("#71368a") },
            { 714312, SKColor.Parse("#005c5c") }
        };

        public MemberChartInfoPeriodicRespoBotTask(ILogger<MemberChartInfoPeriodicRespoBotTask> logger, IDbContext db, RateLimitedIRacingApiClient iRacing, EventHandlers.MemberInfoUpdatedEventHandlerService memberInfoUpdated)
            : base(60000)
        {
            _logger = logger;
            _db = db;
            _iRacing = iRacing;

            _memberInfoUpdated = memberInfoUpdated;
        }

        public override Task Run()
        {
            MemberInfoUpdated += (sender, e) =>
            {
                _logger.LogDebug($"MemberInfoUpdated: Event triggered.");
                _memberInfoUpdated.Run(sender, e);
            };

            return base.Run();
        }

        protected override async void Main()
        {
            List<DataContext.TrackedMember> members = (await _db.Members.FindAllAsync()).ToList();

            if (!_iRacing.DataClient.IsLoggedIn)
            {
                await _iRacing.DataClient.LoginExternalAsync();
            }

            iRApi.Constants.Category[] categories = (await _iRacing.ExecuteAsync<iRApi.Constants.Category[]>(
                () =>
                    _iRacing.DataClient.GetCategoriesAsync()
            )).Data;
            
            iRApi.Member.DriverInfo[] driverInfos = (await _iRacing.ExecuteAsync(
                () =>
                {
                    return _iRacing.DataClient.GetDriverInfoAsync(members.Select(x => x.IRacingMemberId).ToArray(), true);
                }
            )).Data;

            List<Task<iRApi.Common.DataResponse<iRApi.Member.MemberChart>>> memberChartTasks =
                members.Select(
                    member => _iRacing.ExecuteAsync<iRApi.Member.MemberChart>(
                        () =>
                            _iRacing.DataClient.GetMemberChartData(
                                member.IRacingMemberId,
                                (int) categories.FirstOrDefault(x => x.Label.Equals("road", StringComparison.InvariantCultureIgnoreCase))?.Value,
                                iRApi.Member.MemberChartType.IRating
                            )
                        )
                    ).ToList();

            // wait for all responses to return
            await Task.WhenAll(memberChartTasks.ToArray<Task>());
            
            List<iRApi.Member.MemberChart> memberChartResponses = memberChartTasks.Select(x => x.Result.Data).ToList();

            Guid directoryGuid = Guid.NewGuid();

            DirectoryInfo destinationDirectory = new DirectoryInfo(Path.GetDirectoryName($"images/{directoryGuid}/")!);
            
            if (!destinationDirectory.Exists)
                destinationDirectory.Create();
            
            RespoBotSKCartesianChart iRatingCombinedChart = new();
            
            iRatingCombinedChart.SetTitleText($"Respo Racing iRating Graph");

            List<iRApi.Member.MemberChartDataPoint> orderedMembersByMaximumRating = memberChartResponses.Select(x => x.Points.MaxBy(y => y.Value)).ToList();
            List<iRApi.Member.MemberChartDataPoint> orderedMembersByMinimumRating = memberChartResponses.Select(x => x.Points.MinBy(y => y.Value)).ToList();
            int allMembersLowestRating = orderedMembersByMinimumRating.First().Value;
            int allMembersHighestRating = orderedMembersByMaximumRating.Last().Value;
            
            int allMembersGraphYLowPoint = (allMembersLowestRating - (allMembersLowestRating % 500));
            int allMembersGraphYHighPoint = (int)(500 * ((int)Math.Ceiling((double)(allMembersHighestRating / 500)) + 1));
            
            List<double> allUsersGraphCustomYearSeparators = new();

            for(int i = members.MinBy(x => x.MemberSince).MemberSince.Value.Year; i <= DateTime.UtcNow.Year; i++)
            {
                allUsersGraphCustomYearSeparators.Add((double) new DateTime(i, 1, 1).Ticks);
            }
            
            iRatingCombinedChart.SetXAxesLabeler((value) => new DateTime((long) value).ToString("yyyy-MM-dd"));
            iRatingCombinedChart.SetXAxesUnitWidth(TimeSpan.FromDays(365.25).Ticks);
            iRatingCombinedChart.SetXAxesCustomSeparators(allUsersGraphCustomYearSeparators);
            iRatingCombinedChart.SetXAxesMaxLimit(DateTime.UtcNow.Ticks);
            iRatingCombinedChart.SetXAxesMinLimit(members.MinBy(x => x.MemberSince).MemberSince.Value.Ticks);
            
            List<double> allUsersGraphCustomRatingSeparators = new();
            for (
                int i = allMembersGraphYLowPoint;
                i <= ((allMembersGraphYHighPoint <= PlebLine) ? PlebLine + 500 : allMembersGraphYHighPoint);
                i += 500
            )
            {
                allUsersGraphCustomRatingSeparators.Add(i);
            }

            iRatingCombinedChart.SetYAxesLabeler((value) => (int) value == 2500 ? "Pleb Line" : value.ToString(CultureInfo.InvariantCulture));
            iRatingCombinedChart.SetYAxesMinStep(500);
            iRatingCombinedChart.SetYAxesCustomSeparators(allUsersGraphCustomRatingSeparators);
            iRatingCombinedChart.SetYAxesMaxLimit(allUsersGraphCustomRatingSeparators.Last());
            iRatingCombinedChart.SetYAxesMinLimit(allUsersGraphCustomRatingSeparators.First());
            
            RespoBotLineSeries<DateTimePoint> plebLineSeries =
                new RespoBotLineSeries<DateTimePoint>(SKColors.DarkRed, 1)
                {
                    IsVisibleAtLegend = false,
                    Values = new List<DateTimePoint>
                    {
                        new DateTimePoint((DateTime)members.MinBy(x => x.MemberSince).MemberSince, PlebLine),
                        new DateTimePoint(DateTime.UtcNow, PlebLine)
                    }
                };
            
            iRatingCombinedChart.AddSeries(plebLineSeries);

            foreach (iRApi.Member.DriverInfo driverInfo in driverInfos.OrderByDescending(
                 x => x.Licenses.First(
                        y => y.Category.Equals("road", StringComparison.InvariantCultureIgnoreCase)
                    ).IRating
                )
            )
            {
                iRApi.Member.MemberChart memberChart = memberChartResponses.First(x => x.CustomerId.Equals(driverInfo.CustomerId));
                
                DataContext.TrackedMember member = members.FirstOrDefault(x => x.IRacingMemberId.Equals(memberChart.CustomerId));
                
                if (member?.MemberSince == null) continue;
                
                int memberLowestRating = memberChart.Points.MinBy(x => x.Value).Value;
                int memberHighestRating = memberChart.Points.MaxBy(x => x.Value).Value;

                int graphLowPoint = (memberLowestRating - (memberLowestRating % 500));
                int graphHighPoint = (int)(500 * ((int)Math.Ceiling((double)(memberHighestRating / 500)) + 1));

                List<double> singleUserCustomRatingSeparators = new();
                for (
                    int i = graphLowPoint;
                    i <= ((graphHighPoint <= PlebLine) ? PlebLine + 500 : graphHighPoint);
                    i += 500
                )
                {
                    singleUserCustomRatingSeparators.Add(i);
                }

                List<double> singleUserCustomYearSeparators = new();

                for(int i = member.MemberSince.Value.Year; i <= DateTime.UtcNow.Year; i++)
                {
                    singleUserCustomYearSeparators.Add((double) new DateTime(i, 1, 1).Ticks);
                }

                RespoBotSKCartesianChart iRatingChart = new()
                {
                    Legend = null
                };

                iRatingChart.SetTitleText($"iRating Graph for {member.Name} ({memberChart.Points.Last().Value.ToString()})");
                
                iRatingChart.SetXAxesLabeler((value) => new DateTime((long) value).ToString("yyyy-MM-dd"));
                iRatingChart.SetXAxesUnitWidth(TimeSpan.FromDays(365.25).Ticks);
                iRatingChart.SetXAxesCustomSeparators(singleUserCustomYearSeparators);
                iRatingChart.SetXAxesMaxLimit(DateTime.UtcNow.Ticks);
                iRatingChart.SetXAxesMinLimit(member.MemberSince.Value.Ticks);
                
                iRatingChart.SetYAxesLabeler((value) => (int) value == 2500 ? "Pleb Line" : value.ToString(CultureInfo.InvariantCulture));
                iRatingChart.SetYAxesMinStep(500);
                iRatingChart.SetYAxesCustomSeparators(singleUserCustomRatingSeparators);
                iRatingChart.SetYAxesMaxLimit(singleUserCustomRatingSeparators.Last());
                iRatingChart.SetYAxesMinLimit(singleUserCustomRatingSeparators.First());

                // generate pleb line x axis values
                
                RespoBotLineSeries<DateTimePoint> singleUserPlebLineSeries =
                    new RespoBotLineSeries<DateTimePoint>(SKColors.DarkRed, 1)
                    {
                        IsVisibleAtLegend = false,
                        Values = new List<DateTimePoint>
                        {
                            new DateTimePoint((DateTime)member.MemberSince, PlebLine),
                            new DateTimePoint(DateTime.UtcNow, PlebLine)
                        }
                    };
                
                // add customized pleb line
                iRatingChart.AddSeries(singleUserPlebLineSeries);
                
                // generate user line
                List<DateTimePoint> singleUserRatingValues = memberChart.Points.Select(
                    memberChartDataPoint =>
                        new DateTimePoint(memberChartDataPoint.Day.ToDateTime(TimeOnly.MinValue), memberChartDataPoint.Value)
                ).ToList();
                
                singleUserRatingValues.Add(new DateTimePoint(DateTime.UtcNow, singleUserRatingValues.Last().Value));

                RespoBotLineSeries<DateTimePoint> userRatingLineSeries =
                    new RespoBotLineSeries<DateTimePoint>(_userColors[member.IRacingMemberId], 2)
                    {
                        Name = $"{member.Name} ({driverInfo.Licenses.First(x => x.Category.Equals("road", StringComparison.InvariantCultureIgnoreCase)).IRating})",
                        Values = singleUserRatingValues
                        
                    };
                
                // add user line to both charts
                iRatingCombinedChart.AddSeries(userRatingLineSeries);
                iRatingChart.AddSeries(userRatingLineSeries);

                iRatingChart.SaveImage($"images/{directoryGuid}/iRatingChart-{memberChart.CustomerId}-{memberChart.CategoryId}.png");
            }

            iRatingCombinedChart.Series = iRatingCombinedChart.Series;
            iRatingCombinedChart.SaveImage($"images/{directoryGuid}/iRatingCombinedChart-{categories.FirstOrDefault(x => x.Label.Equals("road", StringComparison.InvariantCultureIgnoreCase))?.Value}.png");

            if (!await _db.Members.BulkUpdateAsync(members)) return;

            await _db.Members.BulkUpdateAsync(members);

            _logger.LogDebug($"Updated {members.Count} members.");

            MemberInfoUpdated?.Invoke(null, new EventArgs.MemberInfoUpdatedEventArgs { Members = members });
        }
    }
}
