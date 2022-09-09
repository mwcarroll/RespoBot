using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("licenseInfos")]
    public class LicenseInfo : IEquatable<LicenseInfo>
    {
        [Key]
        [Identity]
        public int Id { get; set; }
        public int iRacingMemberId { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int LicenseLevel { get; set; }
        public float SafetyRating { get; set; }
        public string Color { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public float CornersPerIncident { get; set; }
        public int IRating { get; set; }
        public int TTRating { get; set; }
        public int MprNumberOfRaces { get; set; }
        public int MprNumberOfTimeTrials { get; set; }

        public bool Equals(LicenseInfo other)
        {
            if (other == null) return false;

            return
                object.Equals(this.Id, other.Id) &&
                object.Equals(this.iRacingMemberId, other.iRacingMemberId) &&
                object.Equals(this.CategoryId, other.CategoryId) &&
                object.Equals(this.Category, other.Category) &&
                object.Equals(this.LicenseLevel, other.LicenseLevel) &&
                object.Equals(this.SafetyRating, other.SafetyRating) &&
                object.Equals(this.Color, other.Color) &&
                object.Equals(this.GroupName, other.GroupName) &&
                object.Equals(this.GroupId, other.GroupId) &&
                object.Equals(this.CornersPerIncident, other.CornersPerIncident) &&
                object.Equals(this.IRating, other.IRating) &&
                object.Equals(this.TTRating, other.TTRating) &&
                object.Equals(this.MprNumberOfRaces, other.MprNumberOfRaces) &&
                object.Equals(this.MprNumberOfTimeTrials, other.MprNumberOfTimeTrials);
        }

        public override bool Equals(object obj) => Equals(obj as LicenseInfo);

        public static bool operator ==(LicenseInfo left, LicenseInfo right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(LicenseInfo left, LicenseInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    this.Id,
                    this.iRacingMemberId,
                    this.CategoryId,
                    this.Category,
                    this.LicenseLevel,
                    this.SafetyRating,
                    this.Color,
                    this.GroupName,
                    this.GroupId,
                    this.CornersPerIncident,
                    this.IRating,
                    this.TTRating,
                    this.MprNumberOfRaces,
                    this.MprNumberOfTimeTrials
                ).GetHashCode();
        }
    }
}
