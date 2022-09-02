using AutoMapper;
using DataContext = RespoBot.Data.Classes;
using Aydsko.iRacingData.Member;
using System;
using System.Security.Policy;

namespace RespoBot
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LicenseInfo, DataContext.LicenseInfo>()
                .ForMember(dest => dest.CornersPerIncident, opt => opt.MapFrom(src => Math.Round(src.CornersPerIncident, 2)))
                .ForMember(dest => dest.SafetyRating, opt => opt.MapFrom(src => Math.Round(src.SafetyRating, 2)));
        }
    }
}
