using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("Members")]
    public class Member
    {
        [Key]
        [Identity]
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("iRacingMemberId")]
        public int iRacingMemberId { get; set; }
        [Column("discordMemberId")]
        public long DiscordMemberId { get; set; }
        public DateTime LastChecked { get; set; }

        [InnerJoin(tableName: "memberInfos", key: "iRacingMemberId", externalKey: "iRacingMemberId")]
        public MemberInfo MemberInfo { get; set; }
    }
}
