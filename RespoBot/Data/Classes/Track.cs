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
                object.Equals(this.Id, other.Id) &&
                object.Equals(this.Name, other.Name) &&
                object.Equals(this.AiEnabled, other.AiEnabled) &&
                object.Equals(this.AwardExempt, other.AwardExempt) &&
                object.Equals(this.Category, other.Category) &&
                object.Equals(this.CategoryId, other.CategoryId) &&
                object.Equals(this.Closes, other.Closes) &&
                object.Equals(this.ConfigName, other.ConfigName) &&
                object.Equals(this.CornersPerLap, other.CornersPerLap) &&
                object.Equals(this.Created, other.Created) &&
                object.Equals(this.FreeWithSubscription, other.FreeWithSubscription) &&
                object.Equals(this.FullyLit, other.FullyLit) &&
                object.Equals(this.GridStalls, other.GridStalls) &&
                object.Equals(this.HasOptPath, other.HasOptPath) &&
                object.Equals(this.HasShortParadeLap, other.HasShortParadeLap) &&
                object.Equals(this.HasSvgMap, other.HasSvgMap) &&
                object.Equals(this.IsDirt, other.IsDirt) &&
                object.Equals(this.IsOval, other.IsOval) &&
                object.Equals(this.LapScoring, other.LapScoring) &&
                object.Equals(this.Latitude, other.Latitude) &&
                object.Equals(this.Location, other.Location) &&
                object.Equals(this.Longitude, other.Longitude) &&
                object.Equals(this.MaxCars, other.MaxCars) &&
                object.Equals(this.NightLighting, other.NightLighting) &&
                object.Equals(this.NominalLapTime, other.NominalLapTime) &&
                object.Equals(this.NumberPitstalls, other.NumberPitstalls) &&
                object.Equals(this.Opens, other.Opens) &&
                object.Equals(this.PackageId, other.PackageId) &&
                object.Equals(this.PitRoadSpeedLimit, other.PitRoadSpeedLimit) &&
                object.Equals(this.Price, other.Price) &&
                object.Equals(this.Priority, other.Priority) &&
                object.Equals(this.Purchasable, other.Purchasable) &&
                object.Equals(this.QualifyLaps, other.QualifyLaps) &&
                object.Equals(this.RestartOnLeft, other.RestartOnLeft) &&
                object.Equals(this.Retired, other.Retired) &&
                object.Equals(this.SearchFilters, other.SearchFilters) &&
                object.Equals(this.SiteUrl, other.SiteUrl) &&
                object.Equals(this.Sku, other.Sku) &&
                object.Equals(this.SoloLaps, other.SoloLaps) &&
                object.Equals(this.StartOnLeft, other.StartOnLeft) &&
                object.Equals(this.SupportsGripCompound, other.SupportsGripCompound) &&
                object.Equals(this.TechTrack, other.TechTrack) &&
                object.Equals(this.TimeZone, other.TimeZone) &&
                object.Equals(this.TrackConfigLength, other.TrackConfigLength) &&
                object.Equals(this.TrackDirpath, other.TrackDirpath) &&
                object.Equals(this.Banking, other.Banking);
        }

        public override bool Equals(object obj) => Equals(obj as Track);

        public static bool operator ==(Track left, Track right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Track left, Track right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    this.Id,
                    this.Name,
                    this.AiEnabled,
                    this.AwardExempt,
                    this.Category,
                    this.CategoryId,
                    this.Closes,
                    this.ConfigName,
                    this.CornersPerLap,
                    this.Created,
                    this.FreeWithSubscription,
                    this.FullyLit,
                    this.GridStalls,
                    this.HasOptPath,
                    this.HasShortParadeLap,
                    this.HasSvgMap,
                    this.IsDirt,
                    this.IsOval,
                    this.LapScoring,
                    this.Latitude,
                    this.Location,
                    this.Longitude,
                    this.MaxCars,
                    this.NightLighting,
                    this.NominalLapTime,
                    this.NumberPitstalls,
                    this.Opens,
                    this.PackageId,
                    this.PitRoadSpeedLimit,
                    this.Price,
                    this.Priority,
                    this.Purchasable,
                    this.QualifyLaps,
                    this.RestartOnLeft,
                    this.Retired,
                    this.SearchFilters,
                    this.SiteUrl,
                    this.Sku,
                    this.SoloLaps,
                    this.StartOnLeft,
                    this.SupportsGripCompound,
                    this.TechTrack,
                    this.TimeZone,
                    this.TrackConfigLength,
                    this.TrackDirpath,
                    this.Banking
                ).GetHashCode();
        }
    }
}
