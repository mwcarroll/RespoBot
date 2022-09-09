using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;
using System.Data.SqlClient;

namespace RespoBot.Data.DbContexts
{
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<DataContext.CarInfo> _carInfos;
        private IDapperRepository<DataContext.EventType> _eventTypes;
        private IDapperRepository<DataContext.LicenseInfo> _licenseInfos;
        private IDapperRepository<DataContext.Member> _members;
        private IDapperRepository<DataContext.MemberInfo> _memberInfos;
        private IDapperRepository<DataContext.Events.PublicEvents> _publicEvents;
        private IDapperRepository<DataContext.Track> _tracks;

        public IDapperRepository<DataContext.CarInfo> CarInfos => _carInfos ??= new DapperRepository<DataContext.CarInfo>(Connection);
        public IDapperRepository<DataContext.EventType> EventTypes => _eventTypes ??= new DapperRepository<DataContext.EventType>(Connection);
        public IDapperRepository<DataContext.LicenseInfo> LicenseInfos => _licenseInfos ??= new DapperRepository<DataContext.LicenseInfo>(Connection);
        public IDapperRepository<DataContext.Member> Members => _members ??= new DapperRepository<DataContext.Member>(Connection);
        public IDapperRepository<DataContext.MemberInfo> MemberInfos => _memberInfos ??= new DapperRepository<DataContext.MemberInfo>(Connection);
        public IDapperRepository<DataContext.Events.PublicEvents> PublicEvents => _publicEvents ??= new DapperRepository<DataContext.Events.PublicEvents>(Connection);
        public IDapperRepository<DataContext.Track> Tracks => _tracks ??= new DapperRepository<DataContext.Track>(Connection);

        public MsSqlDbContext(string connectionString) : base(new SqlConnection(connectionString))
        {

        }
    }
}
