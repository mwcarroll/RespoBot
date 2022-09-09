using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using iRApi = Aydsko.iRacingData;

using Discord.WebSocket;

using DataContext = RespoBot.Data.Classes;
using RespoBot.Data.DbContexts;

namespace RespoBot.Services.PeriodicServices
{
    public class StatsMassUpdaterService : PeriodicDiscordService
    {
        private readonly string _connectionString;

        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly IDbContext Db;

        private readonly IMapper Mapper;

        private readonly iRApi.IDataClient IRacingDataClient;

        public StatsMassUpdaterService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, iRApi.IDataClient iRacingDataClient, DiscordSocketClient discordClient) :
            base(configuration, logger, discordClient, nameof(StatsMassUpdaterService))
        {
            Configuration = configuration;
            Logger = logger;

            Db = db;

            Mapper = mapper;

            IRacingDataClient = iRacingDataClient;

            _connectionString = Configuration.GetConnectionString("Default");
        }

        public override async void Run()
        {
            Logger.LogInformation("iRacing Stats - Mass Updater Fired");

            try
            {
                SqlConnection sqlConnection = new(_connectionString);

                IEnumerable<DataContext.Member> members = Db.Members.FindAll();
                IEnumerable<DataContext.MemberInfo> memberInfos = Db.MemberInfos.FindAll();
                IEnumerable<DataContext.LicenseInfo> licenseInfos = Db.LicenseInfos.FindAll();

                iRApi.Member.DriverInfo[] currentDriverInfos = (await IRacingDataClient.GetDriverInfoAsync(customerIds: members.Select(x => x.iRacingMemberId).ToArray(), includeLicenses: true)).Data;

                foreach (DataContext.Member member in members)
                {
                    iRApi.Member.DriverInfo currentDriverInfo = currentDriverInfos.Where(x => x.CustomerId == member.iRacingMemberId).FirstOrDefault();
                    DataContext.MemberInfo cachedMemberInfo = memberInfos.FirstOrDefault(x => x.iRacingMemberId == member.iRacingMemberId);

                    if (cachedMemberInfo == null)
                    {
                        Db.MemberInfos.Insert(new DataContext.MemberInfo()
                        {
                            iRacingMemberId = member.iRacingMemberId,
                            DisplayName = currentDriverInfo.DisplayName,
                            MemberSince = DateTime.Parse(currentDriverInfo.MemberSince)
                        });
                    }
                    else if (
                        !cachedMemberInfo.DisplayName.Equals(currentDriverInfo.DisplayName) ||
                        !cachedMemberInfo.MemberSince.Equals(DateTime.Parse(currentDriverInfo.MemberSince))
                    )
                    {
                        cachedMemberInfo.DisplayName = currentDriverInfo.DisplayName;
                        cachedMemberInfo.MemberSince = DateTime.Parse(currentDriverInfo.MemberSince);

                        Db.MemberInfos.Update(cachedMemberInfo);
                    }

                    DataContext.LicenseInfo[] cachedLicenseInfo = licenseInfos.Where(x => x.iRacingMemberId == member.iRacingMemberId).ToArray();
                    iRApi.Member.LicenseInfo[] currentLicenseInfos = currentDriverInfo.Licenses;

                    DataContext.LicenseInfo[] dataToUpsert = Mapper.Map<DataContext.LicenseInfo[]>(
                        currentLicenseInfos,
                        opts => opts.AfterMap((src, dest) =>
                        {
                            for (int i = 0; i < dest.Length; i++)
                            {
                                dest[i].iRacingMemberId = member.iRacingMemberId;
                                dest[i].Id = cachedLicenseInfo.Where(x => x.iRacingMemberId == member.iRacingMemberId && x.CategoryId == i + 1).Select(x => x.Id).FirstOrDefault();
                            }
                        })
                    );

                    if (cachedLicenseInfo == null || cachedLicenseInfo.Length == 0)
                        Db.LicenseInfos.BulkInsert(dataToUpsert);
                    else if (!cachedLicenseInfo.SequenceEqual(dataToUpsert))
                        Db.LicenseInfos.BulkUpdate(dataToUpsert);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
    }
}
