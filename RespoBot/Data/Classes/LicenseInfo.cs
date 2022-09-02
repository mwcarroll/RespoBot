using MicroOrm.Dapper.Repositories.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
                this.Id.Equals(other.Id) &&
                this.iRacingMemberId.Equals(other.iRacingMemberId) &&
                this.CategoryId.Equals(other.CategoryId) &&
                this.Category.Equals(other.Category) &&
                this.LicenseLevel.Equals(other.LicenseLevel) &&
                this.SafetyRating.Equals(other.SafetyRating) &&
                this.Color.Equals(other.Color) &&
                this.GroupName.Equals(other.GroupName) &&
                this.GroupId.Equals(other.GroupId) &&
                this.CornersPerIncident.Equals(other.CornersPerIncident) &&
                this.IRating.Equals(other.IRating) &&
                this.TTRating.Equals(other.TTRating) &&
                this.MprNumberOfRaces.Equals(other.MprNumberOfRaces) &&
                this.MprNumberOfTimeTrials.Equals(other.MprNumberOfTimeTrials);
        }

    }
}
