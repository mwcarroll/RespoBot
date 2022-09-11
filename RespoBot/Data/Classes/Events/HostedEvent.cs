using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace RespoBot.Data.Classes.Events
{
    [Table("HostedEvents")]
    public class HostedEvent : IEquatable<HostedEvent>
    {
        [Key]
        [Identity]
        public int Id { get; set; }
        [Key]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
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
        [Key]
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

        public bool Equals(HostedEvent other)
        {
            if (other == null) return false;

            return
                Equals(IRacingMemberId, other.IRacingMemberId) &&
                Equals(SessionId, other.SessionId) &&
                Equals(SubsessionId, other.SubsessionId) &&
                Equals(StartTime, other.StartTime) &&
                Equals(EndTime, other.EndTime) &&
                Equals(LicenseCategoryId, other.LicenseCategoryId) &&
                Equals(NumberOfDrivers, other.NumberOfDrivers) &&
                Equals(NumberOfCautions, other.NumberOfCautions) &&
                Equals(NumberOfCautionLaps, other.NumberOfCautionLaps) &&
                Equals(NumberOfLeadChanges, other.NumberOfLeadChanges) &&
                Equals(EventLapsComplete, other.EventLapsComplete) &&
                Equals(DriverChanges, other.DriverChanges) &&
                Equals(WinnerGroupId, other.WinnerGroupId) &&
                Equals(WinnerAi, other.WinnerAi) &&
                Equals(TrackId, other.TrackId) &&
                Equals(PrivateSessionId, other.PrivateSessionId) &&
                Equals(SessionName, other.SessionName) &&
                Equals(LeagueId, other.LeagueId) &&
                Equals(LeagueSeasonId, other.LeagueSeasonId) &&
                Equals(Created, other.Created) &&
                Equals(PracticeLength, other.PracticeLength) &&
                Equals(QualifyLength, other.QualifyLength) &&
                Equals(QualifyLaps, other.QualifyLaps) &&
                Equals(RaceLength, other.RaceLength) &&
                Equals(RaceLaps, other.RaceLaps) &&
                Equals(HeatRace, other.HeatRace) &&
                Equals(HostCustomerId, other.HostCustomerId) &&
                Equals(HostDisplayName, other.HostDisplayName);
        }

        public override bool Equals(object obj) => Equals(obj as HostedEvent);

        public static bool operator ==(HostedEvent left, HostedEvent right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(HostedEvent left, HostedEvent right)
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
                    NumberOfDrivers,
                    NumberOfCautions,
                    NumberOfCautionLaps,
                    NumberOfLeadChanges,
                    EventLapsComplete,
                    DriverChanges,
                    WinnerGroupId,
                    WinnerAi,
                    TrackId,
                    PrivateSessionId,
                    SessionName,
                    LeagueId,
                    LeagueSeasonId,
                    Created,
                    PracticeLength,
                    QualifyLength,
                    QualifyLaps,
                    RaceLength,
                    RaceLaps,
                    HeatRace,
                    HostCustomerId,
                    HostDisplayName
                ).GetHashCode();
        }
    }
    
    namespace Hosted
    {
        public class Host
        {
            public int CustomerId { get; set; }
            public string DisplayName { get; set; }
        }

        [Table("HostedEvents_CarInfos")]
        public class CarInfo : IEquatable<CarInfo>
        {
            public int PrivateSessionId { get; set; }
            public int CarId { get; set; }
            public string CarName { get; set; }
            public int CarClassId { get; set; }
            public string CarClassName { get; set; }
            public string CarClassShortName { get; set; }
            public string CarNameAbbreviated { get; set; }

            public bool Equals(CarInfo other)
            {
                if (other == null) return false;

                return
                    Equals(PrivateSessionId, other.PrivateSessionId) &&
                    Equals(CarId, other.CarId) &&
                    Equals(CarName, other.CarName) &&
                    Equals(CarClassId, other.CarClassId) &&
                    Equals(CarClassName, other.CarClassName) &&
                    Equals(CarClassShortName, other.CarClassShortName) &&
                    Equals(CarNameAbbreviated, other.CarNameAbbreviated);
            }

            public override bool Equals(object obj) => Equals(obj as CarInfo);

            public static bool operator ==(CarInfo left, CarInfo right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(CarInfo left, CarInfo right)
            {
                return !(left == right);
            }

            public override int GetHashCode()
            {
                return (
                        PrivateSessionId,
                        CarId,
                        CarName,
                        CarClassId,
                        CarClassName,
                        CarClassShortName,
                        CarNameAbbreviated
                    ).GetHashCode();
            }
        }
    }
}
