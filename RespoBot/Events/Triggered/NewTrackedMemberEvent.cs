using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aydsko.iRacingData.Member;
using RespoBot.Events.Args;
using RespoBot.Helpers;

namespace RespoBot.Events.Triggered
{
    internal class NewTrackedMemberEvent
    {
        public event EventHandler<NewTrackedMemberEventArgs> NewTrackedMember;
        
        private readonly ILogger<NewTrackedMemberEvent> _logger;
        private readonly IConfiguration _configuration;
        private readonly RateLimitedIRacingApiClient _iRacing;
        private readonly IDbContext _db;

        private int _iRacingMemberId;
        private long _discordMemberId;

        public NewTrackedMemberEvent(ILogger<NewTrackedMemberEvent> logger, IConfiguration configuration, RateLimitedIRacingApiClient iRacing, IDbContext db)
        {
            _logger = logger;
            _configuration = configuration;
            _iRacing = iRacing;
            _db = db;

            NewTrackedMember += Run;
        }
        
        public virtual void OnNewTrackedMember(NewTrackedMemberEventArgs e)
        {
            NewTrackedMember?.Invoke(this, e);
        }

        private void Run(object? sender, NewTrackedMemberEventArgs args)
        {
            _iRacingMemberId = args.IRacingMemberId;
            _discordMemberId = args.DiscordMemberId;
            
            Task.Run(NewTrackedMemberBotTaskAsync);
        }

        private async Task NewTrackedMemberBotTaskAsync()
        {
            List<Task<iRApi.Common.DataResponse<DriverInfo[]>>> driverInfoTasks = [];
            
            if (!_iRacing.DataClient.IsLoggedIn)
            {
                await _iRacing.DataClient.LoginExternalAsync();
            }
            
            driverInfoTasks.Add(
                _iRacing.ExecuteAsync<DriverInfo[]>(
                    () => _iRacing.DataClient.GetDriverInfoAsync([_iRacingMemberId], true)
                )
            );
            
            await Task.WhenAll(driverInfoTasks.ToArray<Task>().Union(driverInfoTasks.ToArray<Task>()));
            
            List<DriverInfo[]> driverInfoList = driverInfoTasks.Select(x => x.Result.Data).ToList();

            foreach (DriverInfo driverInfo in driverInfoList.SelectMany(driverInfos => driverInfos))
            {
                DataContext.TrackedMember newTrackedMember = new()
                {
                    IRacingMemberId = _iRacingMemberId,
                    DiscordMemberId = _discordMemberId,
                    Name = driverInfo.DisplayName,
                    MemberSince = DateTime.Parse(driverInfo.MemberSince)
                };

                _logger.Log(LogLevel.Debug, "Attempting to insert a new tracked member with unique ID {iRacingMemberId}:{DiscordMemberId}", newTrackedMember.IRacingMemberId, newTrackedMember.DiscordMemberId);
                
                bool trackedMemberInserted = await _db.TrackedMembers.InsertAsync(newTrackedMember);
                
                _logger.Log(LogLevel.Debug, "Insertion of tracked member with unique ID {iRacingMemberId}:{DiscordMemberId}: {Successful}", newTrackedMember.IRacingMemberId, newTrackedMember.DiscordMemberId, trackedMemberInserted ? "succeeded" : "failed");
                
                if (driverInfo.Licenses == null) continue;
                foreach (LicenseInfo licenseInfo in driverInfo.Licenses)
                {
                    DataContext.LicenseInfo newLicense = new()
                    {
                        IRacingMemberId = _iRacingMemberId,
                        CategoryId = licenseInfo.CategoryId,
                        Category = licenseInfo.Category,
                        LicenseLevel = licenseInfo.LicenseLevel,
                        SafetyRating = licenseInfo.SafetyRating,
                        Color = licenseInfo.Color,
                        GroupName = licenseInfo.GroupName,
                        GroupId = licenseInfo.GroupId,
                        CornersPerIncident = licenseInfo.CornersPerIncident,
                        IRating = licenseInfo.IRating,
                        TtRating = licenseInfo.TTRating,
                        MprNumberOfRaces = licenseInfo.MprNumberOfRaces,
                        MprNumberOfTimeTrials = licenseInfo.MprNumberOfTimeTrials
                    };
                    
                    _logger.Log(LogLevel.Debug, "Attempting to insert a {Category} license for new tracked member with ID {iRacingMemberId}", newLicense.Category, newLicense.IRacingMemberId);
                    
                    bool licenseInserted = await _db.LicenseInfos.InsertAsync(newLicense);
                    
                    _logger.Log(LogLevel.Debug, "Insertion of {Category} license for new tracked member with ID {iRacingMemberId}: {Successful}", newLicense.Category, newLicense.IRacingMemberId, licenseInserted ? "succeeded" : "failed");
                }
            }
        }
    }
}