using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewCreateDto, Review>();

            CreateMap<ReviewUpdateDto, Review>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Review, ReviewResponseDto>();

            CreateMap<Review, ReviewListItemDto>()
                .ForMember(dest => dest.CommentPreview, opt => opt.MapFrom(src =>
                    src.Comment.Length > 50 ? src.Comment.Substring(0, 50) + "..." : src.Comment));
        }
    }
}
