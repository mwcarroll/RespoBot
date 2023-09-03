using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("TrackedMembers")]
    public class TrackedMember
    {
        [Key]
        // ReSharper disable once InconsistentNaming
        public int IRacingMemberId { get; set; }
        [Key]
        public long DiscordMemberId { get; set; }
        public string Name { get; set; }
        public DateTime? LastCheckedHosted { get; set; }
        public DateTime? LastCheckedOfficial { get; set; }
        public DateTime? LastCheckedLicense { get; set; }
        public DateTime? MemberSince { get; set; }
    }
}
