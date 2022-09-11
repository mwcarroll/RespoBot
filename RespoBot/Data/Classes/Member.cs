using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using MicroOrm.Dapper.Repositories.Attributes;
using MicroOrm.Dapper.Repositories.Attributes.Joins;

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
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public int IRacingMemberId { get; set; }
        [Column("discordMemberId")]
        public long DiscordMemberId { get; set; }
        public DateTime? LastCheckedHosted { get; set; }
        public DateTime? LastCheckedPublic { get; set; }

        [InnerJoin(tableName: "memberInfos", key: "IRacingMemberId", externalKey: "IRacingMemberId")]
        public MemberInfo MemberInfo { get; set; }
    }
}
