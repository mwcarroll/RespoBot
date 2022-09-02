using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

namespace RespoBot.Data.Classes
{
    [Table("members")]
    public class Member
    {
        [Key]
        [Identity, IgnoreUpdate]
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("iRacingMemberId")]
        public int iRacingMemberId { get; set; }
        [Column("discordMemberId")]
        public long DiscordMemberId { get; set; }

        [InnerJoin(tableName: "memberInfos", key: "iRacingMemberId", externalKey: "iRacingMemberId")]
        public MemberInfo MemberInfo { get; set; }
    }
}
