using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("SubSessionResults_Official")]
    public class SubSessionResultsOfficial
    {
        [Key]
        public int SubSessionId { get; set; }
        [Key]
        // ReSharper disable once InconsistentNaming
        public int IRacingMemberId { get; set; }
        public string ClassName { get; set; }
        public int CarNumber { get; set; }
        public int QualifyPosition { get; set; }
        public int FinishPosition { get; set; }
        public int? NumberOfDriversInClass { get; set; }
        public int? CarNumberInClass { get; set; }
        public int? StrengthOfFieldClass { get; set; }
        public int? QualifyPositionInClass { get; set; }
        public int? FinishPositionInClass { get; set; }
        public int? ChampionshipPoints { get; set; }
        // ReSharper disable once InconsistentNaming
        public int? IRatingNew { get; set; }
        // ReSharper disable once InconsistentNaming
        public int? IRatingChange { get; set; }
        public decimal? SafetyRatingNew { get; set; }
        public decimal? SafetyRatingChange { get; set; }
        public int IncidentPoints { get; set; }
    }
}
