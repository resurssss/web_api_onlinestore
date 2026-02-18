using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;
using System.Linq;

namespace OnlineStore.Infrastructure.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ProductCreateDto -> Product
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteItems, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // ProductUpdateDto -> Product
            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteItems, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Product -> ProductResponseDto
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.Stock > 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                    src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));

            // Product -> ProductListItemDto
            CreateMap<Product, ProductListItemDto>()
                .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.Stock > 0));
        }
    }
}
