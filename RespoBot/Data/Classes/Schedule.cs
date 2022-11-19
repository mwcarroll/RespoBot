using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
    [Table("Schedules")]
    public class Schedule : IEquatable<Schedule>
    {
        public int SeasonId { get; set; }
        public int RaceWeekIndex { get; set; }
        public int SeriesId { get; set; }
        public int TrackId { get; set; }

        public bool Equals(Schedule other)
        {
            if (other == null) return false;

            return
                Equals(SeasonId, other.SeasonId) &&
                Equals(RaceWeekIndex, other.RaceWeekIndex) &&
                Equals(SeriesId, other.SeriesId) &&
                Equals(TrackId, other.TrackId);
        }

        public override bool Equals(object obj) => Equals(obj as Schedule);

        public static bool operator ==(Schedule left, Schedule right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Schedule left, Schedule right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (
                    SeasonId,
                    RaceWeekIndex,
                    SeriesId,
                    TrackId
                ).GetHashCode();
        }
    }
}
