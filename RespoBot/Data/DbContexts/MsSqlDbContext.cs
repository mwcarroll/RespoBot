using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;

namespace RespoBot.Data.DbContexts
{
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<DataContext.LicenseInfo> _licenseInfos;
        private IDapperRepository<DataContext.Member> _members;
        private IDapperRepository<DataContext.SubSession> _subSessions;

        public IDapperRepository<DataContext.LicenseInfo> LicenseInfos => _licenseInfos ??= new DapperRepository<DataContext.LicenseInfo>(Connection);
        public IDapperRepository<DataContext.Member> Members => _members ??= new DapperRepository<DataContext.Member>(Connection);
        public IDapperRepository<DataContext.SubSession> SubSessions => _subSessions ??= new DapperRepository<DataContext.SubSession>(Connection);

        public MsSqlDbContext(string connectionString) : base(new SqlConnection(connectionString))
        {

        }
    }
}
