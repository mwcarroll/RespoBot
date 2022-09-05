using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RespoBot.Data.Classes
{
    [Table("CarInfos")]
    public class CarInfo
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
