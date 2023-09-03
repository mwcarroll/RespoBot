using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;

namespace RespoBot.Data.DbContexts
{
    public interface IDbContext : IDapperDbContext
    {
        IDapperRepository<DataContext.LicenseInfo> LicenseInfos { get; }
        IDapperRepository<DataContext.TrackedMember> Members { get; }
        IDapperRepository<DataContext.SubSessionsOfficial> SubSessionsOfficial { get; }
        IDapperRepository<DataContext.SubSessionResultsOfficial> SubSessionResultsOfficial { get; }
    }
}
