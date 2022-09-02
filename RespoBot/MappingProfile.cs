using AutoMapper;
using DataContext = RespoBot.Data.Classes;
using Aydsko.iRacingData.Member;

namespace RespoBot
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LicenseInfo, DataContext.LicenseInfo>();
        }
    }
}
