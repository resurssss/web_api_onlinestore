using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Models;
using OnlineStore.Core.Configurations;
using BCrypt.Net;

namespace OnlineStore.Core;

public class OnlineStoreDbContext : DbContext
{
    public OnlineStoreDbContext(DbContextOptions<OnlineStoreDbContext> options) : base(options)
    {
    }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserSession> UserSessions { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<UserClaim> UserClaims { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Coupon> Coupons { get; set; }
    public virtual DbSet<FavoriteItem> FavoriteItems { get; set; }
    public virtual DbSet<SecurityAuditLog> SecurityAuditLogs { get; set; }
    public virtual DbSet<FileMetadata> FileMetadata { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Применение конфигураций Fluent API
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new CouponConfiguration());
            modelBuilder.ApplyConfiguration(new FavoriteItemConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new SecurityAuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new FileMetadataConfiguration());
            
            // Seed данные
            SeedData(modelBuilder);
            
            // Конфигурация ProductImage
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.FileId);
                entity.Property(e => e.Order).HasDefaultValue(0);
                entity.Property(e => e.IsMain).HasDefaultValue(false);
                
                // Связи
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(d => d.File)
                    .WithMany()
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Установка каскадного удаления для Product -> ProductImage
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            base.OnModelCreating(modelBuilder);
        }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed ролей
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Администратор", Description = "Полный доступ ко всем функциям", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { Id = 2, Name = "Пользователь", Description = "Базовый пользователь", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { Id = 3, Name = "Модератор", Description = "Модерация контента", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { Id = 4, Name = "ПремиумПользователь", Description = "Расширенные функции", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        
        // Seed permissions
        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = Guid.NewGuid(), Name = "CanEditPost", Description = "Может редактировать посты", Category = "Content" },
            new Permission { Id = Guid.NewGuid(), Name = "CanDeleteUser", Description = "Может удалять пользователей", Category = "UserManagement" },
            new Permission { Id = Guid.NewGuid(), Name = "CanViewReports", Description = "Может просматривать отчеты", Category = "Analytics" },
            new Permission { Id = Guid.NewGuid(), Name = "CanManageUsers", Description = "Может управлять пользователями", Category = "UserManagement" },
            new Permission { Id = Guid.NewGuid(), Name = "CanManageProducts", Description = "Может управлять продуктами", Category = "ProductManagement" },
            new Permission { Id = Guid.NewGuid(), Name = "CanViewAnalytics", Description = "Может просматривать аналитику", Category = "Analytics" },
            new Permission { Id = Guid.NewGuid(), Name = "CanManageOrders", Description = "Может управлять заказами", Category = "OrderManagement" }
        );
        
        // Seed тестовых пользователей
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FirstName = "Admin",
                LastName = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
        
        // Seed UserRole связи для тестовых пользователей
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = 1, RoleId = 1, AssignedAt = DateTime.UtcNow }, // Админ - Администратор
            new UserRole { UserId = 2, RoleId = 2, AssignedAt = DateTime.UtcNow }  // Пользователь - Пользователь
        );
    }
}