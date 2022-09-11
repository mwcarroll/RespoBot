using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;

namespace RespoBot.Data.DbContexts
{
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<DataContext.CarInfo> _carInfos;
        private IDapperRepository<DataContext.EventType> _eventTypes;
        private IDapperRepository<DataContext.Events.Hosted.CarInfo> _hostedEventCarInfos;
        private IDapperRepository<DataContext.Events.HostedEvent> _hostedEvents;
        private IDapperRepository<DataContext.LicenseInfo> _licenseInfos;
        private IDapperRepository<DataContext.Member> _members;
        private IDapperRepository<DataContext.MemberInfo> _memberInfos;
        private IDapperRepository<DataContext.Events.PublicEvent> _publicEvents;
        private IDapperRepository<DataContext.Track> _tracks;

        public IDapperRepository<DataContext.CarInfo> CarInfos => _carInfos ??= new DapperRepository<DataContext.CarInfo>(Connection);
        public IDapperRepository<DataContext.EventType> EventTypes => _eventTypes ??= new DapperRepository<DataContext.EventType>(Connection);
        public IDapperRepository<DataContext.Events.Hosted.CarInfo> HostedEventCarInfos => _hostedEventCarInfos ??= new DapperRepository<DataContext.Events.Hosted.CarInfo>(Connection);
        public IDapperRepository<DataContext.Events.HostedEvent> HostedEvents => _hostedEvents ??= new DapperRepository<DataContext.Events.HostedEvent>(Connection);
        public IDapperRepository<DataContext.LicenseInfo> LicenseInfos => _licenseInfos ??= new DapperRepository<DataContext.LicenseInfo>(Connection);
        public IDapperRepository<DataContext.Member> Members => _members ??= new DapperRepository<DataContext.Member>(Connection);
        public IDapperRepository<DataContext.MemberInfo> MemberInfos => _memberInfos ??= new DapperRepository<DataContext.MemberInfo>(Connection);
        public IDapperRepository<DataContext.Events.PublicEvent> PublicEvents => _publicEvents ??= new DapperRepository<DataContext.Events.PublicEvent>(Connection);
        public IDapperRepository<DataContext.Track> Tracks => _tracks ??= new DapperRepository<DataContext.Track>(Connection);

        public MsSqlDbContext(string connectionString) : base(new SqlConnection(connectionString))
        {

        }
    }
}
