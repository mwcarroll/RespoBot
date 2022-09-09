using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public bool PaintRules_RestrictCustomPaint { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public string SiteUrl { get; set; }

        public bool Equals(CarInfo other)
        {
            if (other == null) return false;

            return
                object.Equals(this.AiEnabled, other.AiEnabled) &&
                object.Equals(this.AllowNumberColors, other.AllowNumberColors) &&
                object.Equals(this.AllowNumberFont, other.AllowNumberFont) &&
                object.Equals(this.AllowSponsor1, other.AllowSponsor1) &&
                object.Equals(this.AllowSponsor2, other.AllowSponsor2) &&
                object.Equals(this.AllowWheelColor, other.AllowWheelColor) &&
                object.Equals(this.AwardExempt, other.AwardExempt) &&
                object.Equals(this.CarDirectoryPath, other.CarDirectoryPath) &&
                object.Equals(this.CarId, other.CarId) &&
                object.Equals(this.CarName, other.CarName) &&
                object.Equals(this.CarNameAbbreviated, other.CarNameAbbreviated) &&
                object.Equals(this.CarTypes, other.CarTypes) &&
                object.Equals(this.CarWeight, other.CarWeight) &&
                object.Equals(this.Categories, other.Categories) &&
                object.Equals(this.Created, other.Created) &&
                object.Equals(this.FreeWithSubscription, other.FreeWithSubscription) &&
                object.Equals(this.HasHeadlights, other.HasHeadlights) &&
                object.Equals(this.HasMultipleDryTireTypes, other.HasMultipleDryTireTypes) &&
                object.Equals(this.Hp, other.Hp) &&
                object.Equals(this.MaxPowerAdjustPct, other.MaxPowerAdjustPct) &&
                object.Equals(this.MaxWeightPenaltyKg, other.MinPowerAdjustPct) &&
                object.Equals(this.PackageId, other.PackageId) &&
                object.Equals(this.Patterns, other.Patterns) &&
                object.Equals(this.Price, other.Price) &&
                object.Equals(this.Retired, other.Retired) &&
                object.Equals(this.SearchFilters, other.SearchFilters) &&
                object.Equals(this.Sku, other.Sku) &&
                object.Equals(this.PaintRules_RestrictCustomPaint, other.PaintRules_RestrictCustomPaint) &&
                object.Equals(this.CarMake, other.CarMake) &&
                object.Equals(this.CarModel, other.CarModel) &&
                object.Equals(this.SiteUrl, other.SiteUrl);
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
                    this.AiEnabled,
                    this.AllowNumberColors,
                    this.AllowNumberFont,
                    this.AllowSponsor1,
                    this.AllowSponsor2,
                    this.AllowWheelColor,
                    this.AwardExempt,
                    this.CarDirectoryPath,
                    this.CarId,
                    this.CarName,
                    this.CarNameAbbreviated,
                    this.CarTypes,
                    this.CarWeight,
                    this.Categories,
                    this.Created,
                    this.FreeWithSubscription,
                    this.HasHeadlights,
                    this.HasMultipleDryTireTypes,
                    this.Hp,
                    this.MaxPowerAdjustPct,
                    this.MaxWeightPenaltyKg,
                    this.MinPowerAdjustPct,
                    this.PackageId,
                    this.Patterns,
                    this.Price,
                    this.Retired,
                    this.SearchFilters,
                    this.Sku,
                    this.PaintRules_RestrictCustomPaint,
                    this.CarMake,
                    this.CarModel,
                    this.SiteUrl
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
