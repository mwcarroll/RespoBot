using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("CarInfos")]
    public class CarInfo : IEquatable<CarInfo>
    {
        public bool AiEnabled { get; set; }
        public bool AllowNumberColors { get; set; }
        public bool AllowNumberFont { get; set; }
        public bool AllowSponsor1 { get; set; }
        public bool AllowSponsor2 { get; set; }
        public bool AllowWheelColor { get; set; }
        public bool AwardExempt { get; set; }
        public string CarDirectoryPath { get; set; }
        [Key]
        public int CarId { get; set; }
        public string CarName { get; set; }
        public string CarNameAbbreviated { get; set; }
        public string CarTypes { get; set; }
        public int CarWeight { get; set; }
        public string Categories { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool FreeWithSubscription { get; set; }
        public bool HasHeadlights { get; set; }
        public bool HasMultipleDryTireTypes { get; set; }
        public int Hp { get; set; }
        public int MaxPowerAdjustPct { get; set; }
        public int MaxWeightPenaltyKg { get; set; }
        public int MinPowerAdjustPct { get; set; }
        public int PackageId { get; set; }
        public int Patterns { get; set; }
        public float Price { get; set; }
        public bool Retired { get; set; }
        public string SearchFilters { get; set; }
        public int Sku { get; set; }
        [Column("PaintRules_RestrictCustomPaint")]
        public bool PaintRulesRestrictCustomPaint { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public string SiteUrl { get; set; }

        public bool Equals(CarInfo other)
        {
            if (other == null) return false;

            return
                Equals(AiEnabled, other.AiEnabled) &&
                Equals(AllowNumberColors, other.AllowNumberColors) &&
                Equals(AllowNumberFont, other.AllowNumberFont) &&
                Equals(AllowSponsor1, other.AllowSponsor1) &&
                Equals(AllowSponsor2, other.AllowSponsor2) &&
                Equals(AllowWheelColor, other.AllowWheelColor) &&
                Equals(AwardExempt, other.AwardExempt) &&
                Equals(CarDirectoryPath, other.CarDirectoryPath) &&
                Equals(CarId, other.CarId) &&
                Equals(CarName, other.CarName) &&
                Equals(CarNameAbbreviated, other.CarNameAbbreviated) &&
                Equals(CarTypes, other.CarTypes) &&
                Equals(CarWeight, other.CarWeight) &&
                Equals(Categories, other.Categories) &&
                Equals(Created, other.Created) &&
                Equals(FreeWithSubscription, other.FreeWithSubscription) &&
                Equals(HasHeadlights, other.HasHeadlights) &&
                Equals(HasMultipleDryTireTypes, other.HasMultipleDryTireTypes) &&
                Equals(Hp, other.Hp) &&
                Equals(MaxPowerAdjustPct, other.MaxPowerAdjustPct) &&
                Equals(MaxWeightPenaltyKg, other.MinPowerAdjustPct) &&
                Equals(PackageId, other.PackageId) &&
                Equals(Patterns, other.Patterns) &&
                Equals(Price, other.Price) &&
                Equals(Retired, other.Retired) &&
                Equals(SearchFilters, other.SearchFilters) &&
                Equals(Sku, other.Sku) &&
                Equals(PaintRulesRestrictCustomPaint, other.PaintRulesRestrictCustomPaint) &&
                Equals(CarMake, other.CarMake) &&
                Equals(CarModel, other.CarModel) &&
                Equals(SiteUrl, other.SiteUrl);
        }

        public override bool Equals(object obj) => Equals(obj as CarInfo);

        public static bool operator==(CarInfo left, CarInfo right)
        {
            if(left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator!=(CarInfo left, CarInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    AiEnabled,
                    AllowNumberColors,
                    AllowNumberFont,
                    AllowSponsor1,
                    AllowSponsor2,
                    AllowWheelColor,
                    AwardExempt,
                    CarDirectoryPath,
                    CarId,
                    CarName,
                    CarNameAbbreviated,
                    CarTypes,
                    CarWeight,
                    Categories,
                    Created,
                    FreeWithSubscription,
                    HasHeadlights,
                    HasMultipleDryTireTypes,
                    Hp,
                    MaxPowerAdjustPct,
                    MaxWeightPenaltyKg,
                    MinPowerAdjustPct,
                    PackageId,
                    Patterns,
                    Price,
                    Retired,
                    SearchFilters,
                    Sku,
                    PaintRulesRestrictCustomPaint,
                    CarMake,
                    CarModel,
                    SiteUrl
                ).GetHashCode();
        }
    }

    public class CarTypes
    {
        public string CarType { get; set; }
    }

    public class PaintRules
    {
        public bool RestrictCustomPaint { get; set; }
    }
}
