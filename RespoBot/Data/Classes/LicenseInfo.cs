using MicroOrm.Dapper.Repositories.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RespoBot.Data.Classes
{
    [Table("licenseInfos")]
    public class LicenseInfo
    {
        [Key]
        public int iRacingMemberId { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int LicenseLevel { get; set; }
        public float SafetyRating { get; set; }
        public string Color { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public decimal CornersPerIncident { get; set; }
        public int IRating { get; set; }
        public int TTRating { get; set; }
        public int MprNumberOfRaces { get; set; }
        public int MprNumberOfTimeTrials { get; set; }

    }
}
