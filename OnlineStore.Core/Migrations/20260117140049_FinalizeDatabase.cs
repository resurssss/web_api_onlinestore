using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("04b9c298-ab78-4176-9bfb-1d1fa334b0bc"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1dcc34b6-b59b-4eaf-a80c-5a2c4929cb8e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("223ff3dc-bbf9-4ba2-be1c-dec2844d1ce8"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2797e7e3-3f05-4631-8b8d-66c319946e32"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("8da2631b-129e-4497-ad13-b94ee4251b32"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("a6acae5c-ec97-4f0a-b13d-f21271435161"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f9940924-65ac-4323-8319-34b2c326a7a3"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("0c36d640-07c7-47af-8bbb-e491222e234b"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("37018a7f-2817-4cdf-b5ea-17be20a0f60a"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("7f9ae403-4891-4ad4-8fb7-f7f1b22128df"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("99f8f5da-dfd5-451d-a822-e03994ce0f05"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" },
                    { new Guid("9b3b9d85-6ed0-43ca-b2a3-59ddced17d4b"), "UserManagement", "Может управлять пользователями", "CanManageUsers" },
                    { new Guid("df2d311e-a72e-492f-a850-237727d2c237"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("fd3f893c-838d-4a71-882e-eaa6530d11f8"), "Content", "Может редактировать посты", "CanEditPost" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1399), new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1626) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1869), new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1870) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1872), new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1872) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1874), new DateTime(2026, 1, 17, 14, 0, 48, 868, DateTimeKind.Utc).AddTicks(1874) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 14, 0, 49, 304, DateTimeKind.Utc).AddTicks(658));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 14, 0, 49, 304, DateTimeKind.Utc).AddTicks(883));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 0, 49, 167, DateTimeKind.Utc).AddTicks(3420), "$2a$11$jyQuxEUd8uELNRY63qCPjO22sQbRqVZ0eEppJqwpHV7.0ywCi8h2C", new DateTime(2026, 1, 17, 14, 0, 49, 167, DateTimeKind.Utc).AddTicks(3426) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 0, 49, 303, DateTimeKind.Utc).AddTicks(7817), "$2a$11$79XaHRB/O1wopu.dajVZ5e6pyV60pgCVx/mvdO3JLgsBJFPgiN7C6", new DateTime(2026, 1, 17, 14, 0, 49, 303, DateTimeKind.Utc).AddTicks(7821) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("0c36d640-07c7-47af-8bbb-e491222e234b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("37018a7f-2817-4cdf-b5ea-17be20a0f60a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7f9ae403-4891-4ad4-8fb7-f7f1b22128df"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("99f8f5da-dfd5-451d-a822-e03994ce0f05"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("9b3b9d85-6ed0-43ca-b2a3-59ddced17d4b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("df2d311e-a72e-492f-a850-237727d2c237"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("fd3f893c-838d-4a71-882e-eaa6530d11f8"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("04b9c298-ab78-4176-9bfb-1d1fa334b0bc"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("1dcc34b6-b59b-4eaf-a80c-5a2c4929cb8e"), "Content", "Может редактировать посты", "CanEditPost" },
                    { new Guid("223ff3dc-bbf9-4ba2-be1c-dec2844d1ce8"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("2797e7e3-3f05-4631-8b8d-66c319946e32"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("8da2631b-129e-4497-ad13-b94ee4251b32"), "UserManagement", "Может управлять пользователями", "CanManageUsers" },
                    { new Guid("a6acae5c-ec97-4f0a-b13d-f21271435161"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("f9940924-65ac-4323-8319-34b2c326a7a3"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4010), new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4241) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4481), new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4481) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4483), new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4484) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4485), new DateTime(2026, 1, 17, 13, 55, 48, 814, DateTimeKind.Utc).AddTicks(4486) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 13, 55, 49, 181, DateTimeKind.Utc).AddTicks(5005));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 13, 55, 49, 181, DateTimeKind.Utc).AddTicks(5243));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 13, 55, 49, 44, DateTimeKind.Utc).AddTicks(2323), "$2a$11$Kf9Q/iWII/yaaMjaqIj6M.zAcsV/yHSYPuS5.ADQMuxIdMbU5OctG", new DateTime(2026, 1, 17, 13, 55, 49, 44, DateTimeKind.Utc).AddTicks(2327) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 13, 55, 49, 181, DateTimeKind.Utc).AddTicks(2942), "$2a$11$pBLy9f0NctZJ/Zm2YEqVKOeidksDHOxm2AmigHxgDgBRgw4CqQEK.", new DateTime(2026, 1, 17, 13, 55, 49, 181, DateTimeKind.Utc).AddTicks(2945) });
        }
    }
}
