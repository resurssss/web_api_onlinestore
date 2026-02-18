using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using System.Linq;
//преобразование данных

namespace OnlineStore.Services.MappingProfiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<CartItem, CartItemResponseDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

            CreateMap<Cart, CartResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items ?? Enumerable.Empty<CartItem>()))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.AppliedCoupon, opt => opt.MapFrom(src => src.AppliedCoupon));
            
            CreateMap<Coupon, CouponResponseDto>();
        }
    }
}
