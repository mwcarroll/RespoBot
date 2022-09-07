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
                this.AiEnabled.Equals(other.AiEnabled) &&
                this.AllowNumberColors.Equals(other.AllowNumberColors) &&
                this.AllowNumberFont.Equals(other.AllowNumberFont) &&
                this.AllowSponsor1.Equals(other.AllowSponsor1) &&
                this.AllowSponsor2.Equals(other.AllowSponsor2) &&
                this.AllowWheelColor.Equals(other.AllowWheelColor) &&
                this.AwardExempt.Equals(other.AwardExempt) &&
                this.CarDirectoryPath.Equals(other.CarDirectoryPath) &&
                this.CarId.Equals(other.CarId) &&
                this.CarName.Equals(other.CarName) &&
                this.CarNameAbbreviated.Equals(other.CarNameAbbreviated) &&
                this.CarTypes.Equals(other.CarTypes) &&
                this.CarWeight.Equals(other.CarWeight) &&
                this.Categories.Equals(other.Categories) &&
                this.Created.Equals(other.Created) &&
                this.FreeWithSubscription.Equals(other.FreeWithSubscription) &&
                this.HasHeadlights.Equals(other.HasHeadlights) &&
                this.HasMultipleDryTireTypes.Equals(other.HasMultipleDryTireTypes) &&
                this.Hp.Equals(other.Hp) &&
                this.MaxPowerAdjustPct.Equals(other.MaxPowerAdjustPct) &&
                this.MaxWeightPenaltyKg.Equals(other.MaxWeightPenaltyKg) &&
                this.MinPowerAdjustPct.Equals(other.MinPowerAdjustPct) &&
                this.PackageId.Equals(other.PackageId) &&
                this.Patterns.Equals(other.Patterns) &&
                this.Price.Equals(other.Price) &&
                this.Retired.Equals(other.Price) &&
                this.SearchFilters.Equals(other.SearchFilters) &&
                this.Sku.Equals(other.Sku) &&
                this.PaintRules_RestrictCustomPaint.Equals(other.PaintRules_RestrictCustomPaint) &&
                this.CarMake.Equals(other.CarMake) &&
                this.CarModel.Equals(other.CarModel) &&
                this.SiteUrl.Equals(other.SiteUrl);
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
