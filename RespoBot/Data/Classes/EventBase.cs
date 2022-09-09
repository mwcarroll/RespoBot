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
                object.Equals(this.Id, other.Id) &&
                object.Equals(this.iRacingMemberId, other.iRacingMemberId) &&
                object.Equals(this.SessionId, other.SessionId) &&
                object.Equals(this.SubsessionId, other.SubsessionId) &&
                object.Equals(this.StartTime, other.StartTime) &&
                object.Equals(this.EndTime, other.EndTime) &&
                object.Equals(this.LicenseCategoryId, other.LicenseCategoryId) &&
                object.Equals(this.LicenseCategory, other.LicenseCategory) &&
                object.Equals(this.NumberOfDrivers, other.NumberOfDrivers) &&
                object.Equals(this.NumberOfCautions, other.NumberOfCautions) &&
                object.Equals(this.NumberOfCautionLaps, other.NumberOfCautionLaps) &&
                object.Equals(this.NumberOfLeadChanges, other.NumberOfLeadChanges) &&
                object.Equals(this.EventLapsComplete, other.EventLapsComplete) &&
                object.Equals(this.DriverChanges, other.DriverChanges) &&
                object.Equals(this.WinnerGroupId, other.WinnerGroupId) &&
                object.Equals(this.WinnerName, other.WinnerName) &&
                object.Equals(this.WinnerAi, other.WinnerAi) &&
                object.Equals(this.TrackId, other.TrackId) &&
                object.Equals(this.OfficialSession, other.OfficialSession) &&
                object.Equals(this.SeasonId, other.SeasonId) &&
                object.Equals(this.SeasonYear, other.SeasonYear) &&
                object.Equals(this.SeasonQuarter, other.SeasonQuarter) &&
                object.Equals(this.EventType, other.EventType) &&
                object.Equals(this.EventTypeName, other.EventTypeName) &&
                object.Equals(this.SeriesId, other.SeriesId) &&
                object.Equals(this.SeriesName, other.SeriesName) &&
                object.Equals(this.SeriesShortName, other.SeriesShortName) &&
                object.Equals(this.RaceWeekIndex, other.RaceWeekIndex) &&
                object.Equals(this.EventStrengthOfField, other.EventStrengthOfField) &&
                object.Equals(this.EventAverageLap, other.EventAverageLap) &&
                object.Equals(this.EventBestLapTime, other.EventBestLapTime);
        }

        public override bool Equals(object obj) => Equals(obj as EventBase);

        public static bool operator ==(EventBase left, EventBase right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EventBase left, EventBase right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    this.Id,
                    this.iRacingMemberId,
                    this.SessionId,
                    this.SubsessionId,
                    this.StartTime,
                    this.EndTime,
                    this.LicenseCategoryId,
                    this.LicenseCategory,
                    this.NumberOfDrivers,
                    this.NumberOfCautions,
                    this.NumberOfCautionLaps,
                    this.NumberOfLeadChanges,
                    this.EventLapsComplete,
                    this.DriverChanges,
                    this.WinnerGroupId,
                    this.WinnerName,
                    this.WinnerAi,
                    this.TrackId,
                    this.OfficialSession,
                    this.SeasonId,
                    this.SeasonYear,
                    this.SeasonQuarter,
                    this.EventType,
                    this.EventTypeName,
                    this.SeriesId,
                    this.SeriesName,
                    this.SeriesShortName,
                    this.RaceWeekIndex,
                    this.EventStrengthOfField,
                    this.EventAverageLap,
                    this.EventBestLapTime
                ).GetHashCode();
        }
    }
}
