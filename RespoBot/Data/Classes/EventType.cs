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
                Equals(Label, other.Label) &&
                Equals(Value, other.Value);
        }

        public override bool Equals(object obj) => Equals(obj as EventType);

        public static bool operator ==(EventType left, EventType right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EventType left, EventType right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    Label,
                    Value
                ).GetHashCode();
        }
    }
}
