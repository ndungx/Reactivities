using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Comments
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Comment, CommentDTO>()
                .ForMember(
                    dest => dest.Username, 
                    opt => opt.MapFrom(src => src.Author.UserName)
                )
                .ForMember(
                    dest => dest.DisplayName, 
                    opt => opt.MapFrom(src => src.Author.DisplayName)
                )
                .ForMember(
                    dest => dest.Image, 
                    opt => opt.MapFrom(
                        src => src.Author.Photos.FirstOrDefault(x => x.IsMain).Url
                    )
                );
        }
    }
}