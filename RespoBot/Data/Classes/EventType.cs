using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("EventTypes")]
    public class EventType
    {
        public string Label { get; set; }
        [Key]
        public int Value { get; set; }
    }
}
