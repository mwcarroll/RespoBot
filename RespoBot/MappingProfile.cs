using System;

namespace RespoBot
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<iRApi.Member.LicenseInfo, DataContext.LicenseInfo>()
                .ForMember(dest => dest.CornersPerIncident, 
                    opt => opt.MapFrom(src => Math.Round(src.CornersPerIncident, 2)))
                .ForMember(dest => dest.SafetyRating, 
                    opt => opt.MapFrom(src => Math.Round(src.SafetyRating, 2)));

            CreateMap<iRApi.Series.Schedule, DataContext.Schedule>()
                .ForMember(dest => dest.TrackId, 
                    opt => opt.MapFrom(src => src.Track.TrackId));

            CreateMap<iRApi.Searches.OfficialSearchResultItem, DataContext.SubSessionsOfficial>()
                .ForMember(dest => dest.TrackName,
                    opt => opt.MapFrom(src => src.Track.TrackName))
                .ForMember(dest => dest.StrengthOfField,
                    opt => opt.MapFrom(src => src.EventStrengthOfField));

            CreateMap<iRApi.Results.Result, DataContext.SubSessionResultsOfficial>()
                .ForMember(dest => dest.IRatingChange,
                    opt => opt.MapFrom(src => src.NewIRating - src.OldIRating))
                .ForMember(dest => dest.SafetyRatingChange,
                    opt => opt.MapFrom(src => Math.Round(src.NewSafetyRating - src.OldSafetyRating, 2)));
        }
    }
}
