using AutoMapper;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Mapping
{
    public class FavoriteProfile : Profile
    {
        public FavoriteProfile()
        {
            // Create DTO -> Entity
            CreateMap<FavoriteCreateDto, FavoriteItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore()) // продукт заполняется в сервисе
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Entity -> Response DTO (включая вложенный Product)
            CreateMap<FavoriteItem, FavoriteResponseDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product)); // Product -> ProductListItemDto

            // Entity -> ListItem DTO (для списков)
            CreateMap<FavoriteItem, FavoriteListItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0m));
        }
    }
}
