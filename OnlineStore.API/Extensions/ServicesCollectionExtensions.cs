using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineStore.Core.DTOs;
using OnlineStore.Core;
using OnlineStore.Services.Services;
using FluentValidation;
using OnlineStore.Core.Validators;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Repositories;
using OnlineStore.Core.Services;

namespace OnlineStore.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Регистрация репозиториев как Scoped
            services.AddScoped<IProductRepository, ProductRepository>();
            
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Scoped — создаются на каждый HTTP-запрос
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<FileService>();
            services.AddScoped<OnlineStore.Core.Services.FileValidationService>();
            services.AddScoped<OnlineStore.Core.Services.FileStorageService>();
            
            return services;
        }

        public static IServiceCollection AddJwtService(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            // Регистрация валидаторов как Transient
            services.AddTransient<IValidator<CartItemCreateDto>, CartItemCreateValidator>();
            services.AddTransient<IValidator<CartItemUpdateDto>, CartItemUpdateValidator>();
            services.AddTransient<IValidator<CouponCreateDto>, CouponCreateValidator>();
            services.AddTransient<IValidator<CouponUpdateDto>, CouponUpdateValidator>();
            services.AddTransient<IValidator<FavoriteCreateDto>, FavoriteValidator>();
            services.AddTransient<IValidator<ProductCreateDto>, ProductCreateValidator>();
            services.AddTransient<IValidator<ProductUpdateDto>, ProductUpdateValidator>();
            services.AddTransient<IValidator<ReviewCreateDto>, ReviewValidator>();
            services.AddTransient<IValidator<ReviewUpdateDto>, ReviewUpdateValidator>();
            services.AddTransient<IValidator<RegisterDTO>, RegisterValidator>();
            services.AddTransient<IValidator<ForgotPasswordDTO>, ForgotPasswordDTOValidator>();
            services.AddTransient<IValidator<ResetPasswordDTO>, ResetPasswordDTOValidator>();
            services.AddTransient<IValidator<ChangePasswordDTO>, ChangePasswordDTOValidator>();
            services.AddTransient<IValidator<ConfirmEmailDTO>, ConfirmEmailDTOValidator>();
            
            return services;
        }
    }
}
