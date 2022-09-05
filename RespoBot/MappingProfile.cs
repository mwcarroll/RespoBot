using AutoMapper;
using System;
using System.Linq;
using DataContext = RespoBot.Data.Classes;
using iRApiCars = Aydsko.iRacingData.Cars;
using iRApiConstants = Aydsko.iRacingData.Constants;
using iRApiMember = Aydsko.iRacingData.Member;
using iRApiSearches = Aydsko.iRacingData.Searches;
using iRApiTracks = Aydsko.iRacingData.Tracks;

namespace RespoBot
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<iRApiCars.CarInfo, DataContext.CarInfo>()
                .ForMember(dest => dest.CarTypes, opt => opt.MapFrom(src => string.Join(',', src.CarTypes.Select(x => x.CarType))))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => string.Join(',', src.Categories)))
                .ForMember(dest => dest.PaintRules_RestrictCustomPaint, opt => opt.MapFrom(src => src.PaintRules != null && src.PaintRules.RestrictCustomPaint));

            CreateMap<iRApiConstants.EventType, DataContext.EventType>();

            CreateMap<iRApiMember.LicenseInfo, DataContext.LicenseInfo>()
                .ForMember(dest => dest.CornersPerIncident, opt => opt.MapFrom(src => Math.Round(src.CornersPerIncident, 2)))
                .ForMember(dest => dest.SafetyRating, opt => opt.MapFrom(src => Math.Round(src.SafetyRating, 2)));

            CreateMap<iRApiSearches.OfficialSearchResultItem, DataContext.Events.PublicEvents>()
                .ForMember(dest => dest.TrackId, opt => opt.MapFrom(src => src.Track.TrackId))
                .ForMember(dest => dest.EventAverageLap, opt => opt.MapFrom(src => src.EventAverageLap != null ? src.EventAverageLap.Value.Ticks : 0))
                .ForMember(dest => dest.EventBestLapTime, opt => opt.MapFrom(src => src.EventBestLapTime != null ? src.EventBestLapTime.Value.Ticks : 0));

            CreateMap<iRApiTracks.Track, DataContext.Track>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TrackId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TrackName))
                .ForMember(dest => dest.Opens, opt => opt.MapFrom(src => new DateTime(src.Opens.Year, src.Opens.Month, src.Opens.Day)))
                .ForMember(dest => dest.Closes, opt => opt.MapFrom(src => new DateTime(src.Closes.Year, src.Closes.Month, src.Closes.Day)));
        }
    }
}
