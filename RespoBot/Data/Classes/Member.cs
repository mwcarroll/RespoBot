using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("Members")]
    public class Member
    {
        [Key]
        public int IRacingMemberId { get; set; }
        public long DiscordMemberId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime? LastCheckedHosted { get; set; }
        public DateTime? LastCheckedOfficial { get; set; }
        public DateTime? LastCheckedLicense { get; set; }
        public DateTime? MemberSince { get; set; }
    }
}
