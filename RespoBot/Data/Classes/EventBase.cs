using Aydsko.iRacingData.Results;
using MicroOrm.Dapper.Repositories.Attributes;
using System.ComponentModel.DataAnnotations;
using System;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace RespoBot.Data.Classes
{
    public class EventBase
    {
        [Key]
        [Identity]
        public int Id { get; set; }
        [Key]
        public int iRacingMemberId { get; set; }
        public int SessionId { get; set; }
        [Key]
        public int SubsessionId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int LicenseCategoryId { get; set; }
        public string LicenseCategory { get; set; }
        public int NumberOfDrivers { get; set; }
        public int NumberOfCautions { get; set; }
        public int NumberOfCautionLaps { get; set; }
        public int NumberOfLeadChanges { get; set; }
        public int EventLapsComplete { get; set; }
        public bool DriverChanges { get; set; }
        public int WinnerGroupId { get; set; }
        public string WinnerName { get; set; }
        public bool WinnerAi { get; set; }
        [InnerJoin(tableName: "Tracks", key: "TrackId", externalKey:"Id")]
        public int TrackId { get; set; }
        public bool OfficialSession { get; set; }
        public int SeasonId { get; set; }
        public int SeasonYear { get; set; }
        public int SeasonQuarter { get; set; }
        public int EventType { get; set; }
        public string EventTypeName { get; set; }
        public int SeriesId { get; set; }
        public string SeriesName { get; set; }
        public string SeriesShortName { get; set; }
        public int RaceWeekIndex { get; set; }
        public int EventStrengthOfField { get; set; }
        public long EventAverageLap { get; set; }
        public long EventBestLapTime { get; set; }
    }
}
