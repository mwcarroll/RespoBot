using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("Tracks")]
    public class Track : IEquatable<Track>
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

        public bool Equals(Track other)
        {
            if (other == null) return false;

            return
                this.Id.Equals(other.Id) &&
                this.Name.Equals(other.Name) &&
                this.AiEnabled.Equals(other.AiEnabled) &&
                this.AwardExempt.Equals(other.AwardExempt) &&
                this.Category.Equals(other.Category) &&
                this.CategoryId.Equals(other.CategoryId) &&
                this.Closes.Equals(other.Closes) &&
                this.ConfigName.Equals(other.ConfigName) &&
                this.CornersPerLap.Equals(other.CornersPerLap) &&
                this.Created.Equals(other.Created) &&
                this.FreeWithSubscription.Equals(other.FreeWithSubscription) &&
                this.FullyLit.Equals(other.FullyLit) &&
                this.GridStalls.Equals(other.GridStalls) &&
                this.HasOptPath.Equals(other.HasOptPath) &&
                this.HasShortParadeLap.Equals(other.HasShortParadeLap) &&
                this.HasSvgMap.Equals(other.HasSvgMap) &&
                this.IsDirt.Equals(other.IsDirt) &&
                this.IsOval.Equals(other.IsOval) &&
                this.LapScoring.Equals(other.LapScoring) &&
                this.Latitude.Equals(other.Latitude) &&
                this.Location.Equals(other.Location) &&
                this.Longitude.Equals(other.Longitude) &&
                this.MaxCars.Equals(other.MaxCars) &&
                this.NightLighting.Equals(other.NightLighting) &&
                this.NominalLapTime.Equals(other.NominalLapTime) &&
                this.NumberPitstalls.Equals(other.NumberPitstalls) &&
                this.Opens.Equals(other.Opens) &&
                this.PackageId.Equals(other.PackageId) &&
                this.PitRoadSpeedLimit.Equals(other.PitRoadSpeedLimit) &&
                this.Price.Equals(other.Price) &&
                this.Priority.Equals(other.Priority) &&
                this.Purchasable.Equals(other.Purchasable) &&
                this.QualifyLaps.Equals(other.QualifyLaps) &&
                this.RestartOnLeft.Equals(other.RestartOnLeft) &&
                this.Retired.Equals(other.Retired) &&
                this.SearchFilters.Equals(other.SearchFilters) &&
                this.SiteUrl.Equals(other.SiteUrl) &&
                this.Sku.Equals(other.Sku) &&
                this.SoloLaps.Equals(other.SoloLaps) &&
                this.StartOnLeft.Equals(other.StartOnLeft) &&
                this.SupportsGripCompound.Equals(other.SupportsGripCompound) &&
                this.TechTrack.Equals(other.TechTrack) &&
                this.TimeZone.Equals(other.TimeZone) &&
                this.TrackConfigLength.Equals(other.TrackConfigLength) &&
                this.TrackDirpath.Equals(other.TrackDirpath) &&
                this.Banking.Equals(other.Banking);
        }
    }
}
