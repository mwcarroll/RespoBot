using System;
using System.Linq;

namespace RespoBot
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<iRApi.Member.LicenseInfo, DataContext.LicenseInfo>()
                .ForMember(dest => dest.CornersPerIncident, opt => opt.MapFrom(src => Math.Round(src.CornersPerIncident, 2)))
                .ForMember(dest => dest.SafetyRating, opt => opt.MapFrom(src => Math.Round(src.SafetyRating, 2)));
        }
    }
}
