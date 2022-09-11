using MicroOrm.Dapper.Repositories;
using MicroOrm.Dapper.Repositories.DbContext;

namespace RespoBot.Data.DbContexts
{
    public interface IDbContext : IDapperDbContext
    {
        IDapperRepository<DataContext.CarInfo> CarInfos { get; }
        IDapperRepository<DataContext.EventType> EventTypes { get; }
        IDapperRepository<DataContext.Events.Hosted.CarInfo> HostedEventCarInfos { get; }
        IDapperRepository<DataContext.Events.HostedEvent> HostedEvents { get; }
        IDapperRepository<DataContext.LicenseInfo> LicenseInfos { get; }
        IDapperRepository<DataContext.Member> Members { get; }
        IDapperRepository<DataContext.MemberInfo> MemberInfos { get; }
        IDapperRepository<DataContext.Events.OfficialEvent> OfficialEvents { get; }
        IDapperRepository<DataContext.Track> Tracks { get; }
    }
}
