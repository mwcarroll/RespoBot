using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("memberInfos")]
    public class MemberInfo
    {
        [Key]
        [Column("iRacingMemberId")]
        public int iRacingMemberId { get; set; }
        public string DisplayName { get; set; }
        public DateTime MemberSince { get; set; }
        [LeftJoin(tableName: "licenseInfos", key: "iRacingMemberId", externalKey: "iRacingMemberId")]
        public List<LicenseInfo> LicenseInfo { get; set; }
    }
}
