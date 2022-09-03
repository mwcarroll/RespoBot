using Aydsko.iRacingData;
using Aydsko.iRacingData.Member;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DataContext = RespoBot.Data.Classes;
using RespoBot.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;

namespace RespoBot.Services
{
    public class StatsMassUpdaterService : PeriodicService
    {
        private readonly string _connectionString;

        private readonly IConfiguration Configuration;
        private readonly ILogger<EntryPoint> Logger;

        private readonly IDbContext Db;

        private readonly IMapper Mapper;

        private readonly IDataClient IRacingDataClient;

        public StatsMassUpdaterService(IConfiguration configuration, ILogger<EntryPoint> logger, IDbContext db, IMapper mapper, IDataClient iRacingDataClient, DiscordSocketClient discordClient) :
            base(configuration, logger, discordClient, "StatsMassUpdaterService")
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

                DriverInfo[] currentDriverInfos = (await IRacingDataClient.GetDriverInfoAsync(customerIds: members.Select(x => x.iRacingMemberId).ToArray(), includeLicenses: true)).Data;

                foreach (DataContext.Member member in members)
                {
                    DriverInfo currentDriverInfo = currentDriverInfos.Where(x => x.CustomerId == member.iRacingMemberId).FirstOrDefault();
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
                    LicenseInfo[] currentLicenseInfos = currentDriverInfo.Licenses;

                    DataContext.LicenseInfo[] dataToUpsert = Mapper.Map<LicenseInfo[], DataContext.LicenseInfo[]>(
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
