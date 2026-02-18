using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Infrastructure.Mapping
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponResponseDto>()
                .ForMember(dest => dest.TimesUsed, opt => opt.MapFrom(src => src.TimesUsed))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActiveCoupon()))
                .ReverseMap();

            CreateMap<CouponCreateDto, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TimesUsed, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<CouponUpdateDto, Coupon>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Coupon, CouponListItemDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActiveCoupon()));
        }
    }
}
