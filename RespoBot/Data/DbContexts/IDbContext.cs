using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;
using RespoBot.Data.Classes;

namespace RespoBot.Data.DbContexts
{
    public interface IDbContext : IDapperDbContext
    {
        IDapperRepository<LicenseInfo> LicenseInfos { get; }
        IDapperRepository<Member> Members { get; }
        IDapperRepository<MemberInfo> MemberInfos { get; }

    }
}
