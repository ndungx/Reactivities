using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDTO>();
            CreateMap<UserActivity, AttendeeDTO>()
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src => src.AppUser.UserName)
                )
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.MapFrom(src => src.AppUser.DisplayName)
                )
                .ForMember(
                    dest => dest.Image,
                    opt => opt.MapFrom(
                        src => src.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url
                    )
                )
                .ForMember(
                    dest => dest.Following,
                    opt => opt.MapFrom<FollowingResolver>()
                );
        }
    }
}