using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("Tracks")]
    public class Track
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        public bool AiEnabled { get; set; }
        public bool AwardExempt { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public DateTime Closes { get; set; }
        public string ConfigName { get; set; }
        public int CornersPerLap { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool FreeWithSubscription { get; set; }
        public bool FullyLit { get; set; }
        public int GridStalls { get; set; }
        public bool HasOptPath { get; set; }
        public bool HasShortParadeLap { get; set; }
        public bool HasSvgMap { get; set; }
        public bool IsDirt { get; set; }
        public bool IsOval { get; set; }
        public int LapScoring { get; set; }
        public decimal Latitude { get; set; }
        public string Location { get; set; }
        public decimal Longitude { get; set; }
        public int MaxCars { get; set; }
        public bool NightLighting { get; set; }
        public TimeSpan? NominalLapTime { get; set; }
        public int NumberPitstalls { get; set; }
        public DateTime Opens { get; set; }
        public int PackageId { get; set; }
        public int PitRoadSpeedLimit { get; set; }
        public decimal Price { get; set; }
        public int Priority { get; set; }
        public bool Purchasable { get; set; }
        public int QualifyLaps { get; set; }
        public bool RestartOnLeft { get; set; }
        public bool Retired { get; set; }
        public string SearchFilters { get; set; }
        public string SiteUrl { get; set; }
        public int Sku { get; set; }
        public int SoloLaps { get; set; }
        public bool StartOnLeft { get; set; }
        public bool SupportsGripCompound { get; set; }
        public bool TechTrack { get; set; }
        public string TimeZone { get; set; }
        public float TrackConfigLength { get; set; }
        public string TrackDirpath { get; set; }
        public string Banking { get; set; }
    }
}
