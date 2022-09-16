using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("SubSessions")]
    public class SubSession
    {
        [Key]
        public int IRacingMemberId { get; set; }
        [Key]
        public int SubSessionId { get; set; }
        [Column("Official")]
        public bool IsOfficial { get; set; }
        [Column("HostedOrLeague")]
        public bool IsHostedOrLeague { get; set; }
        public string SeriesName { get; set; }
        public string ClassName { get; set; }
        public string TrackName { get; set; }
        public int NumberOfDrivers { get; set; }
        public int CarNumber { get; set; }
        public int StrengthOfField { get; set; }
        public int QualifyPosition { get; set; }
        public int FinishPosition { get; set; }
        public int? NumberOfDriversInClass { get; set; }
        public int? CarNumberInClass { get; set; }
        public int? StrengthOfFieldClass { get; set; }
        public int? QualifyPositionInClass { get; set; }
        public int? FinishPositionInClass { get; set; }
        public int? ChampionshipPoints { get; set; }
        public int? IRatingNew { get; set; }
        public int? IRatingChange { get; set; }
        public decimal? SafetyRatingNew { get; set; }
        public decimal? SafetyRatingChange { get; set; }
        public int IncidentPoints { get; set; }
    }
}
