using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes
{
	[Table("SubSessions_Official")]
	public class SubSessionsOfficial
	{
		[Key]
		public int SubSessionId { get; set; }

		public string SeriesName { get; set; } = null!;
		public string TrackName { get; set; } = null!;
		public int NumberOfDrivers { get; set; }
		public int StrengthOfField { get; set; }
	}
}