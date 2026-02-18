using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1d94a1d1-9df1-497e-a6d3-e397a7a7689d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1e69fd1d-6e09-4950-a4c4-440ba777f884"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2ff5d0d0-e84d-44e8-9c57-76444992f511"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("5db41f46-5d0a-4809-b3bc-20db2c55a283"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("828c6c2e-4b8f-469b-ae6a-867872b4d7b0"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("98386d97-da94-4f9f-af11-7148f5ad1797"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("b2521f4d-c325-4b00-b3de-afcd55f6e836"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("56d6ae76-d8d8-47a9-bf0c-2927e3cb36dd"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("713f1fc6-9f61-481c-904f-2e4228385a75"), "UserManagement", "Может управлять пользователями", "CanManageUsers" },
                    { new Guid("91d6f477-75c5-45c4-904c-ebcec04da23b"), "Content", "Может редактировать посты", "CanEditPost" },
                    { new Guid("9563b1eb-e6d5-4c3a-8b5a-887cb87ce337"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("974f3adb-6376-467d-9f56-44f0d4f7fe62"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("abb0d353-496c-41f6-b2c2-bcc3125ade72"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("cdb7237d-2ad6-4454-9962-d9db1ceb8720"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1525), new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1762) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1980), new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1980) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1982), new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1983) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1984), new DateTime(2026, 1, 12, 12, 5, 34, 322, DateTimeKind.Utc).AddTicks(1985) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 12, 12, 5, 34, 793, DateTimeKind.Utc).AddTicks(1485));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 12, 12, 5, 34, 793, DateTimeKind.Utc).AddTicks(1740));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 12, 5, 34, 653, DateTimeKind.Utc).AddTicks(5332), "$2a$11$.V/DPQ51TBBWV9nLkraaUep4r9Tj2GE6VpTdmHrDeT/ljpBvv81jm", new DateTime(2026, 1, 12, 12, 5, 34, 653, DateTimeKind.Utc).AddTicks(5336) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 12, 5, 34, 792, DateTimeKind.Utc).AddTicks(9277), "$2a$11$M/tD5NzpGP25OlCca9w8HeZ7J5NdTYq68wwNXI/YwFAKH9ZrQiBy6", new DateTime(2026, 1, 12, 12, 5, 34, 792, DateTimeKind.Utc).AddTicks(9280) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("56d6ae76-d8d8-47a9-bf0c-2927e3cb36dd"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("713f1fc6-9f61-481c-904f-2e4228385a75"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("91d6f477-75c5-45c4-904c-ebcec04da23b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("9563b1eb-e6d5-4c3a-8b5a-887cb87ce337"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("974f3adb-6376-467d-9f56-44f0d4f7fe62"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("abb0d353-496c-41f6-b2c2-bcc3125ade72"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("cdb7237d-2ad6-4454-9962-d9db1ceb8720"));

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("1d94a1d1-9df1-497e-a6d3-e397a7a7689d"), "UserManagement", "Может управлять пользователями", "CanManageUsers" },
                    { new Guid("1e69fd1d-6e09-4950-a4c4-440ba777f884"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("2ff5d0d0-e84d-44e8-9c57-76444992f511"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("5db41f46-5d0a-4809-b3bc-20db2c55a283"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" },
                    { new Guid("828c6c2e-4b8f-469b-ae6a-867872b4d7b0"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("98386d97-da94-4f9f-af11-7148f5ad1797"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("b2521f4d-c325-4b00-b3de-afcd55f6e836"), "Content", "Может редактировать посты", "CanEditPost" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(7767), new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8089) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8449), new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8450) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8453), new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8453) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8455), new DateTime(2026, 1, 8, 12, 56, 11, 651, DateTimeKind.Utc).AddTicks(8456) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 8, 12, 56, 11, 653, DateTimeKind.Utc).AddTicks(3065));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 8, 12, 56, 11, 653, DateTimeKind.Utc).AddTicks(3359));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 8, 12, 56, 11, 653, DateTimeKind.Utc).AddTicks(1128), "AQAAAAEAACcQAAAAEJ1b3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz", new DateTime(2026, 1, 8, 12, 56, 11, 653, DateTimeKind.Utc).AddTicks(1446) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 8, 12, 56, 11, 653, DateTimeKind.Utc).AddTicks(1747), "AQAAAAEAACcQAAAAEJ1b3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz3vKz", new DateTime(2026, 1, 8, 12, 56, 11, 653, DateTimeKind.Utc).AddTicks(1747) });
        }
    }
}
