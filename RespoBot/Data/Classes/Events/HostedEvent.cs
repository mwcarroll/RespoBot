using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RespoBot.Data.Classes.Events
{
    [Table("HostedEvents")]
    public class HostedEvent
    {
        public int IRacingMemberId { get; set; }
        public int SessionId { get; set; }
        public int SubsessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LicenseCategoryId { get; set; }
        public int NumberOfDrivers { get; set; }
        public int NumberOfCautions { get; set; }
        public int NumberOfCautionLaps { get; set; }
        public int NumberOfLeadChanges { get; set; }
        public int EventLapsComplete { get; set; }
        public bool DriverChanges { get; set; }
        public int WinnerGroupId { get; set; }
        public bool WinnerAi { get; set; }
        public int TrackId { get; set; }
        public int PrivateSessionId { get; set; }
        public string SessionName { get; set; }
        public int LeagueId { get; set; }
        public int LeagueSeasonId { get; set; }
        public DateTime Created { get; set; }
        public int PracticeLength { get; set; }
        public int QualifyLength { get; set; }
        public int QualifyLaps { get; set; }
        public int RaceLength { get; set; }
        public int RaceLaps { get; set; }
        public bool HeatRace { get; set; }
        public int HostCustomerId { get; set; }
        public string HostDisplayName { get; set; }
        [InnerJoin(tableName: "HostedEvents_CarInfos", key: "PrivateSessionId", externalKey: "PrivateSessionId")]
        public Hosted.CarInfo[] Cars { get; set; }
    }
    
    namespace Hosted
    {
        public class Host
        {
            public int CustomerId { get; set; }
            public string DisplayName { get; set; }
        }

        [Table("HostedEvents_CarInfos")]
        public class CarInfo
        {
            public int IRacingMemberId { get; set; }
            public int PrivateSessionId { get; set; }
            public int CarId { get; set; }
            public string CarName { get; set; }
            public int CarClassId { get; set; }
            public string CarClassName { get; set; }
            public string CarClassShortName { get; set; }
            public string CarNameAbbreviated { get; set; }
        }
    }
}
