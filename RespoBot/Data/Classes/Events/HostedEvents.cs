using MicroOrm.Dapper.Repositories.Attributes.Joins;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RespoBot.Data.Classes.Events
{
    [Table("HostedEvents")]
    public class HostedEvents : EventBase
    {
        public int PrivateSessionId { get; set; }
        public string SessionName { get; set; }
        public int LeagueId { get; set; }
        public int LeagueSeasonId { get; set; }
        public DateTime Created { get; set; }
        public int PracticeLength { get; set; }
        public int QualifyLength { get; set; }
        public int QualifyLaps { get; set; }
        public int RaceLength { get; set; }
        public int RaceLaps { get; set; }
        public bool HeatRace { get; set; }
        public Host Host { get; set; }
        [InnerJoin(tableName: "HostedEvents_CarInfos", key: "PrivateSessionId", externalKey: "PrivateSessionId")]
        public List<CarInfo> Cars { get; set; }
    }

    public class Host
    {
        [Column("Host_CustomerId")]
        public int CustomerId { get; set; }
        [Column("Host_DisplayName")]
        public string DisplayName { get; set; }
    }

    [Table("HostedEvents_CarInfos")]
    public class CarInfo
    {
        [Column("Id")]
        public int CarId { get; set; }
        [Column("Name")]
        public string CarName { get; set; }
        [Column("ClassId")]
        public int CarClassId { get; set; }
        [Column("ClassName")]
        public string CarClassName { get; set; }
        [Column("ClassShortName")]
        public string CarClassShortName { get; set; }
        [Column("NameAbbreviated")]
        public string CarNameAbbreviated { get; set; }
    }
}
