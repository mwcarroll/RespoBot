using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;

namespace RespoBot.Data.DbContexts
{
    public interface IDbContext : IDapperDbContext
    {
        IDapperRepository<DataContext.LicenseInfo> LicenseInfos { get; }
        IDapperRepository<DataContext.Member> Members { get; }
        IDapperRepository<DataContext.SubSession> SubSessions { get; }
    }
}
