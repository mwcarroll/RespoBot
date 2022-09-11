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
                Equals(Id, other.Id) &&
                Equals(Name, other.Name) &&
                Equals(AiEnabled, other.AiEnabled) &&
                Equals(AwardExempt, other.AwardExempt) &&
                Equals(Category, other.Category) &&
                Equals(CategoryId, other.CategoryId) &&
                Equals(Closes, other.Closes) &&
                Equals(ConfigName, other.ConfigName) &&
                Equals(CornersPerLap, other.CornersPerLap) &&
                Equals(Created, other.Created) &&
                Equals(FreeWithSubscription, other.FreeWithSubscription) &&
                Equals(FullyLit, other.FullyLit) &&
                Equals(GridStalls, other.GridStalls) &&
                Equals(HasOptPath, other.HasOptPath) &&
                Equals(HasShortParadeLap, other.HasShortParadeLap) &&
                Equals(HasSvgMap, other.HasSvgMap) &&
                Equals(IsDirt, other.IsDirt) &&
                Equals(IsOval, other.IsOval) &&
                Equals(LapScoring, other.LapScoring) &&
                Equals(Latitude, other.Latitude) &&
                Equals(Location, other.Location) &&
                Equals(Longitude, other.Longitude) &&
                Equals(MaxCars, other.MaxCars) &&
                Equals(NightLighting, other.NightLighting) &&
                Equals(NominalLapTime, other.NominalLapTime) &&
                Equals(NumberPitstalls, other.NumberPitstalls) &&
                Equals(Opens, other.Opens) &&
                Equals(PackageId, other.PackageId) &&
                Equals(PitRoadSpeedLimit, other.PitRoadSpeedLimit) &&
                Equals(Price, other.Price) &&
                Equals(Priority, other.Priority) &&
                Equals(Purchasable, other.Purchasable) &&
                Equals(QualifyLaps, other.QualifyLaps) &&
                Equals(RestartOnLeft, other.RestartOnLeft) &&
                Equals(Retired, other.Retired) &&
                Equals(SearchFilters, other.SearchFilters) &&
                Equals(SiteUrl, other.SiteUrl) &&
                Equals(Sku, other.Sku) &&
                Equals(SoloLaps, other.SoloLaps) &&
                Equals(StartOnLeft, other.StartOnLeft) &&
                Equals(SupportsGripCompound, other.SupportsGripCompound) &&
                Equals(TechTrack, other.TechTrack) &&
                Equals(TimeZone, other.TimeZone) &&
                Equals(TrackConfigLength, other.TrackConfigLength) &&
                Equals(TrackDirpath, other.TrackDirpath) &&
                Equals(Banking, other.Banking);
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
                    Id,
                    Name,
                    AiEnabled,
                    AwardExempt,
                    Category,
                    CategoryId,
                    Closes,
                    ConfigName,
                    CornersPerLap,
                    Created,
                    FreeWithSubscription,
                    FullyLit,
                    GridStalls,
                    HasOptPath,
                    HasShortParadeLap,
                    HasSvgMap,
                    IsDirt,
                    IsOval,
                    LapScoring,
                    Latitude,
                    Location,
                    Longitude,
                    MaxCars,
                    NightLighting,
                    NominalLapTime,
                    NumberPitstalls,
                    Opens,
                    PackageId,
                    PitRoadSpeedLimit,
                    Price,
                    Priority,
                    Purchasable,
                    QualifyLaps,
                    RestartOnLeft,
                    Retired,
                    SearchFilters,
                    SiteUrl,
                    Sku,
                    SoloLaps,
                    StartOnLeft,
                    SupportsGripCompound,
                    TechTrack,
                    TimeZone,
                    TrackConfigLength,
                    TrackDirpath,
                    Banking
                ).GetHashCode();
        }
    }
}
