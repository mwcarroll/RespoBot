using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("EventTypes")]
    public class EventType : IEquatable<EventType>
    {
        public string Label { get; set; }
        [Key]
        public int Value { get; set; }


        public bool Equals(EventType other)
        {
            if (other == null) return false;

            return
                this.Label.Equals(other.Label) &&
                this.Value.Equals(other.Value);
        }
    }
}
