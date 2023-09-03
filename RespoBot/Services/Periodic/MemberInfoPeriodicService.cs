using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aydsko.iRacingData.Member;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;

namespace RespoBot.Services.Periodic
{
    internal class MemberInfoPeriodicService
    {
        private readonly ILogger<MemberInfoPeriodicService> _logger;
        private readonly IDbContext _db;
        private readonly RateLimitedIRacingApiClient _iRacing;

        private readonly Services.EventHandlers.MemberInfoUpdatedEventHandlerService _memberInfoUpdated;

        public event EventHandler<EventArgs.MemberInfoUpdatedEventArgs> MemberInfoUpdated;

        public MemberInfoPeriodicService(ILogger<MemberInfoPeriodicService> logger, IDbContext db, RateLimitedIRacingApiClient iRacing, Services.EventHandlers.MemberInfoUpdatedEventHandlerService memberInfoUpdated)
        {
            _logger = logger;
            _db = db;
            _iRacing = iRacing;

            _memberInfoUpdated = memberInfoUpdated;
        }

        public void Run()
        {
            MemberInfoUpdated += (sender, e) =>
            {
                _logger.LogDebug($"MemberInfoUpdated: Event triggered.");
                _memberInfoUpdated.Run(sender, e);
            };

            Task.Run(RunMemberInfo);
        }

        private async void RunMemberInfo()
        {
            List<DataContext.TrackedMember> members = (await _db.Members.FindAllAsync()).ToList();

            if (!_iRacing.DataClient.IsLoggedIn)
            {
                await _iRacing.DataClient.LoginExternalAsync();
            }

            iRApi.Member.DriverInfo[] driverInfos = (await _iRacing.ExecuteAsync(
                    () =>
                    {
                        return _iRacing.DataClient.GetDriverInfoAsync(members.Select(x => x.IRacingMemberId).ToArray(), true);
                    }
                )).Data;

            foreach (DataContext.TrackedMember member in members)
            {
                DriverInfo driverInfo = driverInfos.FirstOrDefault(x => x.CustomerId.Equals(member.IRacingMemberId));
                DateTime.TryParse(driverInfo?.MemberSince, out DateTime memberSince);
                member.MemberSince = memberSince;

                if (driverInfo?.Licenses == null) continue;
            
                SKCartesianChart iRatingChart = new SKCartesianChart
                {
                    Width = 900,
                    Height = 600,
                    Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = new double[] { driverInfo.Licenses.FirstOrDefault(x => x.Category.Equals("road"))!.IRating},
                            Fill = null,
                            GeometrySize = 0,
                            // use the line smoothness property to control the curve
                            // it goes from 0 to 1
                            // where 0 is a straight line and 1 the most curved
                            LineSmoothness = 1
                        }
                    }
                };
                
                iRatingChart.SaveImage("images/iRatingChart.png");
            }

            if (!await _db.Members.BulkUpdateAsync(members)) return;

            await _db.Members.BulkUpdateAsync(members);

            _logger.LogDebug($"Updated {members.Count} members.");

            MemberInfoUpdated?.Invoke(null, new EventArgs.MemberInfoUpdatedEventArgs { Members = members });
        }
    }
}
