using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("LicenseInfos")]
    public class LicenseInfo : IEquatable<LicenseInfo>
    {
        [Key]
        // ReSharper disable once InconsistentNaming
        public int iRacingMemberId { get; set; }
        [Key]
        public int CategoryId { get; set; }
        public string Category { get; set; } = null!;
        public int LicenseLevel { get; set; }
        public decimal SafetyRating { get; set; }
        public string Color { get; set; } = null!;
        public string GroupName { get; set; } = null!;
        public int GroupId { get; set; }
        public decimal CornersPerIncident { get; set; }
        
        // ReSharper disable once InconsistentNaming
        public int? iRating { get; set; }
        public int? TimeTrialRating { get; set; }
        public int MinimumParticipationRequirementNumberOfRaces { get; set; }
        public int MinimumParticipationRequirementNumberOfTimeTrials { get; set; }

        public bool Equals(LicenseInfo? other)
        {
            if (other == null) return false;

            return
                Equals(iRacingMemberId, other.iRacingMemberId) &&
                Equals(CategoryId, other.CategoryId) &&
                Equals(Category, other.Category) &&
                Equals(LicenseLevel, other.LicenseLevel) &&
                Equals(SafetyRating, other.SafetyRating) &&
                Equals(Color, other.Color) &&
                Equals(GroupName, other.GroupName) &&
                Equals(GroupId, other.GroupId) &&
                Equals(CornersPerIncident, other.CornersPerIncident) &&
                Equals(iRating, other.iRating) &&
                Equals(TimeTrialRating, other.TimeTrialRating) &&
                Equals(MinimumParticipationRequirementNumberOfRaces, other.MinimumParticipationRequirementNumberOfRaces) &&
                Equals(MinimumParticipationRequirementNumberOfTimeTrials, other.MinimumParticipationRequirementNumberOfTimeTrials);
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
                    iRacingMemberId,
                    CategoryId,
                    Category,
                    LicenseLevel,
                    SafetyRating,
                    Color,
                    GroupName,
                    GroupId,
                    CornersPerIncident,
                    iRating,
                    TimeTrialRating,
                    MinimumParticipationRequirementNumberOfRaces,
                    MinimumParticipationRequirementNumberOfTimeTrials
                ).GetHashCode();
        }
    }
}
