using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace RespoBot.Data.Classes
{
    [Table("MemberInfos")]
    public class MemberInfo
    {
        [Key]
        [Column("iRacingMemberId")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public int IRacingMemberId { get; set; }
        public string DisplayName { get; set; }
        public DateTime MemberSince { get; set; }
        [LeftJoin(tableName: "licenseInfos", key: "iRacingMemberId", externalKey: "iRacingMemberId")]
        public List<LicenseInfo> LicenseInfo { get; set; }
    }
}
