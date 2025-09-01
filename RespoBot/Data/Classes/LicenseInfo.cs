using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace RespoBot.Data.Classes
{
    [Table("LicenseInfos")]
    public class LicenseInfo : IEquatable<LicenseInfo>
    {
        [Key]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public int IRacingMemberId { get; set; }
        [Key]
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int LicenseLevel { get; set; }
        public decimal SafetyRating { get; set; }
        public string Color { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public decimal CornersPerIncident { get; set; }
        // ReSharper disable once InconsistentNaming
        public int IRating { get; set; }
        public int TtRating { get; set; }
        public int MprNumberOfRaces { get; set; }
        public int MprNumberOfTimeTrials { get; set; }

        public bool Equals(LicenseInfo? other)
        {
            if (other == null) return false;

            return
                Equals(IRacingMemberId, other.IRacingMemberId) &&
                Equals(CategoryId, other.CategoryId) &&
                Equals(Category, other.Category) &&
                Equals(LicenseLevel, other.LicenseLevel) &&
                Equals(SafetyRating, other.SafetyRating) &&
                Equals(Color, other.Color) &&
                Equals(GroupName, other.GroupName) &&
                Equals(GroupId, other.GroupId) &&
                Equals(CornersPerIncident, other.CornersPerIncident) &&
                Equals(IRating, other.IRating) &&
                Equals(TtRating, other.TtRating) &&
                Equals(MprNumberOfRaces, other.MprNumberOfRaces) &&
                Equals(MprNumberOfTimeTrials, other.MprNumberOfTimeTrials);
        }

        public override bool Equals(object? obj) => Equals(obj as LicenseInfo);

        public static bool operator ==(LicenseInfo? left, LicenseInfo? right)
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
                    IRacingMemberId,
                    CategoryId,
                    Category,
                    LicenseLevel,
                    SafetyRating,
                    Color,
                    GroupName,
                    GroupId,
                    CornersPerIncident,
                    IRating,
                    TTRating: TtRating,
                    MprNumberOfRaces,
                    MprNumberOfTimeTrials
                ).GetHashCode();
        }
    }
}
