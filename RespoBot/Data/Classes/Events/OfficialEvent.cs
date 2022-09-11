using MicroOrm.Dapper.Repositories.Attributes.Joins;
using MicroOrm.Dapper.Repositories.Attributes;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace RespoBot.Data.Classes.Events
{
    [Table("OfficialEvents")]
    public class OfficialEvent : IEquatable<OfficialEvent>
    {
        [Key]
        [Identity]
        public int Id { get; set; }
        [Key]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public int IRacingMemberId { get; set; }
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
        [InnerJoin(tableName: "Tracks", key: "TrackId", externalKey: "Id")]
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

        public bool Equals(OfficialEvent other)
        {
            if (other == null) return false;

            return
                Equals(IRacingMemberId, other.IRacingMemberId) &&
                Equals(SessionId, other.SessionId) &&
                Equals(SubsessionId, other.SubsessionId) &&
                Equals(StartTime, other.StartTime) &&
                Equals(EndTime, other.EndTime) &&
                Equals(LicenseCategoryId, other.LicenseCategoryId) &&
                Equals(LicenseCategory, other.LicenseCategory) &&
                Equals(NumberOfDrivers, other.NumberOfDrivers) &&
                Equals(NumberOfCautions, other.NumberOfCautions) &&
                Equals(NumberOfCautionLaps, other.NumberOfCautionLaps) &&
                Equals(NumberOfLeadChanges, other.NumberOfLeadChanges) &&
                Equals(EventLapsComplete, other.EventLapsComplete) &&
                Equals(DriverChanges, other.DriverChanges) &&
                Equals(WinnerGroupId, other.WinnerGroupId) &&
                Equals(WinnerName, other.WinnerName) &&
                Equals(WinnerAi, other.WinnerAi) &&
                Equals(TrackId, other.TrackId) &&
                Equals(OfficialSession, other.OfficialSession) &&
                Equals(SeasonId, other.SeasonId) &&
                Equals(SeasonYear, other.SeasonYear) &&
                Equals(SeasonQuarter, other.SeasonQuarter) &&
                Equals(EventType, other.EventType) &&
                Equals(EventTypeName, other.EventTypeName) &&
                Equals(SeriesId, other.SeriesId) &&
                Equals(SeriesName, other.SeriesName) &&
                Equals(SeriesShortName, other.SeriesShortName) &&
                Equals(RaceWeekIndex, other.RaceWeekIndex) &&
                Equals(EventStrengthOfField, other.EventStrengthOfField) &&
                Equals(EventAverageLap, other.EventAverageLap) &&
                Equals(EventBestLapTime, other.EventBestLapTime);
        }

        public override bool Equals(object obj) => Equals(obj as OfficialEvent);

        public static bool operator ==(OfficialEvent left, OfficialEvent right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(OfficialEvent left, OfficialEvent right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    IRacingMemberId,
                    SessionId,
                    SubsessionId,
                    StartTime,
                    EndTime,
                    LicenseCategoryId,
                    LicenseCategory,
                    NumberOfDrivers,
                    NumberOfCautions,
                    NumberOfCautionLaps,
                    NumberOfLeadChanges,
                    EventLapsComplete,
                    DriverChanges,
                    WinnerGroupId,
                    WinnerName,
                    WinnerAi,
                    TrackId,
                    OfficialSession,
                    SeasonId,
                    SeasonYear,
                    SeasonQuarter,
                    EventType,
                    EventTypeName,
                    SeriesId,
                    SeriesName,
                    SeriesShortName,
                    RaceWeekIndex,
                    EventStrengthOfField,
                    EventAverageLap,
                    EventBestLapTime
                ).GetHashCode();
        }
    }
}
