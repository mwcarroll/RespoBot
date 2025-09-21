using System.Data.SqlClient;
using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;

namespace RespoBot.Data.DbContexts
{
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<DataContext.LicenseInfo> _licenseInfos;
        private IDapperRepository<DataContext.TrackedMember> _members;
        private IDapperRepository<DataContext.SubSessionsOfficial> _subSessionsOfficial;
        private IDapperRepository<DataContext.SubSessionResultsOfficial> _subSessionResultsOfficial;

        public IDapperRepository<DataContext.LicenseInfo> LicenseInfos => _licenseInfos ??= new DapperRepository<DataContext.LicenseInfo>(Connection);
        public IDapperRepository<DataContext.TrackedMember> TrackedMembers => _members ??= new DapperRepository<DataContext.TrackedMember>(Connection);
        public IDapperRepository<DataContext.SubSessionsOfficial> SubSessionsOfficial => _subSessionsOfficial ??= new DapperRepository<DataContext.SubSessionsOfficial>(Connection);
        public IDapperRepository<DataContext.SubSessionResultsOfficial> SubSessionResultsOfficial => _subSessionResultsOfficial ??= new DapperRepository<DataContext.SubSessionResultsOfficial>(Connection);

        public MsSqlDbContext(string connectionString) : base(new SqlConnection(connectionString))
        {

        }
    }
}
