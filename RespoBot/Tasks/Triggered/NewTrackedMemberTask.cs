using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aydsko.iRacingData.Member;
using RespoBot.Helpers;

namespace RespoBot.Tasks.Triggered
{
    internal class NewTrackedMemberTask
    {
        private readonly ILogger<NewTrackedMemberTask> _logger;
        private readonly IConfiguration _configuration;
        private readonly RateLimitedIRacingApiClient _iRacing;

        private int _memberId;

        public NewTrackedMemberTask(ILogger<NewTrackedMemberTask> logger, IConfiguration configuration, RateLimitedIRacingApiClient iRacing)
        {
            _logger = logger;
            _configuration = configuration;
            _iRacing = iRacing;
        }

        public void Run(int memberId)
        {
            _memberId = memberId;
            
            Task.Run(NewTrackedMemberBotTaskAsync);
        }

        private async Task NewTrackedMemberBotTaskAsync()
        {
            DataContext.TrackedMember newTrackedMember = new DataContext.TrackedMember();
            
            List<Task<iRApi.Common.DataResponse<DriverInfo[]>>> driverInfoTasks = [];
            
            if (!_iRacing.DataClient.IsLoggedIn)
            {
                await _iRacing.DataClient.LoginExternalAsync();
            }
            
            driverInfoTasks.Add(
                _iRacing.ExecuteAsync<DriverInfo[]>(
                    () => _iRacing.DataClient.GetDriverInfoAsync([_memberId], true)
                )
            );
            
            await Task.WhenAll(driverInfoTasks.ToArray<Task>().Union(driverInfoTasks.ToArray<Task>()));
            
            List<DriverInfo[]> driverInfoList = driverInfoTasks.Select(x => x.Result.Data).ToList();

            foreach (DriverInfo driverInfo in driverInfoList.SelectMany(driverInfos => driverInfos))
            {
                _logger.Log(LogLevel.Debug, $"{driverInfo.CustomerId} - {driverInfo.MemberSince} - {driverInfo.DisplayName} - {driverInfo.LastLogin}");
                    
                if (driverInfo.Licenses == null) continue;
                foreach (LicenseInfo licenseInfo in driverInfo.Licenses)
                {
                    _logger.Log(LogLevel.Debug, $"{driverInfo.CustomerId} - {licenseInfo.Category} - {licenseInfo.IRating}");
                }
            }
        }
    }
}