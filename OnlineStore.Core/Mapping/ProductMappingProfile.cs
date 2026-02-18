using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
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
                .ForMember(dest => dest.Stock, opt => opt.Ignore()) // Не меняем Stock если не передан
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => 
                {
                    // Не обновляем свойства, которые не были переданы (null)
                    if (srcMember == null)
                        return false;
                    
                    // Для int? проверяем, что значение действительно передано (не default)
                    if (srcMember.GetType() == typeof(int?))
                    {
                        var intVal = (int?)srcMember;
                        return intVal.HasValue;
                    }
                    
                    return true;
                }));

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