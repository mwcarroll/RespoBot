using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System.ComponentModel.DataAnnotations;
using System;

namespace RespoBot.Data.Classes
{
    public class EventBase : IEquatable<EventBase>
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

        public bool Equals(EventBase other)
        {
            if (other == null) return false;

            return
                this.Id.Equals(other.Id) &&
                this.iRacingMemberId.Equals(other.iRacingMemberId) &&
                this.SessionId.Equals(other.SessionId) &&
                this.SubsessionId.Equals(other.SubsessionId) &&
                this.StartTime.Equals(other.StartTime) &&
                this.EndTime.Equals(other.EndTime) &&
                this.LicenseCategoryId.Equals(other.LicenseCategoryId) &&
                this.LicenseCategory.Equals(other.LicenseCategory) &&
                this.NumberOfDrivers.Equals(other.NumberOfDrivers) &&
                this.NumberOfCautions.Equals(other.NumberOfCautions) &&
                this.NumberOfCautionLaps.Equals(other.NumberOfCautionLaps) &&
                this.NumberOfLeadChanges.Equals(other.NumberOfLeadChanges) &&
                this.EventLapsComplete.Equals(other.EventLapsComplete) &&
                this.DriverChanges.Equals(other.DriverChanges) &&
                this.WinnerGroupId.Equals(other.WinnerGroupId) &&
                this.WinnerName.Equals(other.WinnerName) &&
                this.WinnerAi.Equals(other.WinnerAi) &&
                this.TrackId.Equals(other.TrackId) &&
                this.OfficialSession.Equals(other.OfficialSession) &&
                this.SeasonId.Equals(other.SeasonId) &&
                this.SeasonYear.Equals(other.SeasonYear) &&
                this.SeasonQuarter.Equals(other.SeasonQuarter) &&
                this.EventType.Equals(other.EventType) &&
                this.EventTypeName.Equals(other.EventTypeName) &&
                this.SeriesId.Equals(other.SeriesId) &&
                this.SeriesName.Equals(other.SeriesName) &&
                this.SeriesShortName.Equals(other.SeriesShortName) &&
                this.RaceWeekIndex.Equals(other.RaceWeekIndex) &&
                this.EventStrengthOfField.Equals(other.EventStrengthOfField) &&
                this.EventAverageLap.Equals(other.EventAverageLap) &&
                this.EventBestLapTime.Equals(other.EventBestLapTime);
        }
    }
}
