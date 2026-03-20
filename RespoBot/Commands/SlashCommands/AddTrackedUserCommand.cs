using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aydsko.iRacingData.Common;
using Aydsko.iRacingData.Member;
using Discord.Interactions;
using Discord.WebSocket;

namespace RespoBot.Commands.SlashCommands;

public class AddTrackedUserCommand(
    ILogger<DiscordSocketClient> logger,
    iRApi.IDataClient iRacing,
    IDbContext db) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add-tracked-user", "Adds a user to be tracked.")]
    public async Task AddTrackedUser(
        [Summary("DiscordUser", "The user's discord to be added.")] SocketGuildUser user,
        [Summary("iRacingMemberID", "The user's iRacing member ID to be added.")] int iRacingMemberId)
    {
        try
        {
            Task<DataResponse<DriverInfo[]>> driverInfoTask = iRacing.GetDriverInfoAsync([iRacingMemberId], true);

            await Task.WhenAll(driverInfoTask);

            DriverInfo[] driverInfos = driverInfoTask.Result.Data;

            foreach (DriverInfo driverInfo in driverInfos)
            {
                DataContext.TrackedMember newTrackedMember = new()
                {
                    IRacingMemberId = iRacingMemberId,
                    DiscordMemberId = (long) user.Id,
                    Name = driverInfo.DisplayName,
                    MemberSince = DateTime.Parse(driverInfo.MemberSince)
                };

                logger.Log(LogLevel.Debug, "Attempting to insert a new tracked member with unique ID {iRacingMemberId}:{DiscordMemberId}", newTrackedMember.IRacingMemberId, newTrackedMember.DiscordMemberId);
                
                bool trackedMemberInserted = await db.TrackedMembers.InsertAsync(newTrackedMember);
                
                logger.Log(LogLevel.Debug, "Insertion of tracked member with unique ID {iRacingMemberId}:{DiscordMemberId}: {Successful}", newTrackedMember.IRacingMemberId, newTrackedMember.DiscordMemberId, trackedMemberInserted ? "succeeded" : "failed");
                
                if (driverInfo.Licenses == null) continue;
                foreach (LicenseInfo licenseInfo in driverInfo.Licenses)
                {
                    DataContext.LicenseInfo newLicense = new()
                    {
                        iRacingMemberId = iRacingMemberId,
                        CategoryId = licenseInfo.CategoryId,
                        Category = licenseInfo.Category,
                        LicenseLevel = licenseInfo.LicenseLevel,
                        SafetyRating = licenseInfo.SafetyRating,
                        Color = licenseInfo.Color,
                        GroupName = licenseInfo.GroupName,
                        GroupId = licenseInfo.GroupId,
                        CornersPerIncident = licenseInfo.CornersPerIncident,
                        iRating = licenseInfo.iRating,
                        TimeTrialRating = licenseInfo.TimeTrialRating,
                        MinimumParticipationRequirementNumberOfRaces = licenseInfo.MinimumParticipationRequirementNumberOfRaces,
                        MinimumParticipationRequirementNumberOfTimeTrials = licenseInfo.MinimumParticipationRequirementNumberOfTimeTrials
                    };
                    
                    logger.Log(LogLevel.Debug, "Attempting to insert a {Category} license for new tracked member with ID {iRacingMemberId}", newLicense.Category, newLicense.iRacingMemberId);
                    
                    bool licenseInserted = await db.LicenseInfos.InsertAsync(newLicense);
                    
                    logger.Log(LogLevel.Debug, "Insertion of {Category} license for new tracked member with ID {iRacingMemberId}: {Successful}", newLicense.Category, newLicense.iRacingMemberId, licenseInserted ? "succeeded" : "failed");
                }
            }
        }
        catch (Exception ex)
        {
            await RespondAsync($"Error: {ex}", ephemeral: true);
        }
    }
}