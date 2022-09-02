using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;
using RespoBot.Data.Classes;
using System.Data.SqlClient;

namespace RespoBot.Data.DbContexts
{
    public class MsSqlDbContext : DapperDbContext, IDbContext
    {
        private IDapperRepository<LicenseInfo> _licenseInfos;
        private IDapperRepository<Member> _members;
        private IDapperRepository<MemberInfo> _memberInfos;
        
        public IDapperRepository<LicenseInfo> LicenseInfos => _licenseInfos ??= new DapperRepository<LicenseInfo>(Connection);
        public IDapperRepository<Member> Members => _members ??= new DapperRepository<Member>(Connection);
        public IDapperRepository<MemberInfo> MemberInfos => _memberInfos ??= new DapperRepository<MemberInfo>(Connection);

        public MsSqlDbContext(string connectionString) : base(new SqlConnection(connectionString))
        {

        }
    }
}
