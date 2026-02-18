using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToFileMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1bfa9b94-472f-457e-bb3e-64e1c53551f2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("363253f2-45d6-4f7d-8ba3-33fa2830b135"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("48b42c9b-07e0-47da-81d6-f26ad50be585"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("b28924a8-96d0-4f6f-9a4c-4458b1853d6c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c3fb4458-f700-4ab8-9abd-8fa438434b86"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ce7bc9eb-525e-4917-a0be-3fd5eae086ae"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("da76217e-ad8b-4351-838f-a03fc557efce"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FileMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("3b82c831-4cb7-4e76-a9ad-666f33d07ef2"), "Content", "Может редактировать посты", "CanEditPost" },
                    { new Guid("48a47005-8d29-461e-b53f-508632739ebb"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" },
                    { new Guid("8e359098-40dd-4755-8554-12be15d12c85"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("8fb467d7-e217-4276-9928-9a4cf40a1d69"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("9ffc44ac-5dad-402b-a03c-aacf3f0e82e0"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("a3c4ccd0-3037-4e0e-9da1-fbceb75c7de1"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("d59c6730-0fab-4245-a447-710ffe5e30ee"), "UserManagement", "Может управлять пользователями", "CanManageUsers" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4361), new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4636) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4900), new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4900) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4903), new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4903) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4905), new DateTime(2026, 1, 18, 22, 59, 28, 24, DateTimeKind.Utc).AddTicks(4906) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 18, 22, 59, 28, 483, DateTimeKind.Utc).AddTicks(4276));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 18, 22, 59, 28, 483, DateTimeKind.Utc).AddTicks(4554));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 18, 22, 59, 28, 322, DateTimeKind.Utc).AddTicks(4151), "$2a$11$uCykiS9puWb9Lj8orS9AquePoYoqTg9TVlDmgn0Y1bliwxJhrOk/m", new DateTime(2026, 1, 18, 22, 59, 28, 322, DateTimeKind.Utc).AddTicks(4156) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 18, 22, 59, 28, 483, DateTimeKind.Utc).AddTicks(1463), "$2a$11$2Y07TqOdSdFEnm1nd3mFoOzM1nBy9YP8M1G12WpNun1vnLBbBCixe", new DateTime(2026, 1, 18, 22, 59, 28, 483, DateTimeKind.Utc).AddTicks(1468) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("3b82c831-4cb7-4e76-a9ad-666f33d07ef2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("48a47005-8d29-461e-b53f-508632739ebb"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("8e359098-40dd-4755-8554-12be15d12c85"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("8fb467d7-e217-4276-9928-9a4cf40a1d69"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("9ffc44ac-5dad-402b-a03c-aacf3f0e82e0"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("a3c4ccd0-3037-4e0e-9da1-fbceb75c7de1"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d59c6730-0fab-4245-a447-710ffe5e30ee"));

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FileMetadata");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("1bfa9b94-472f-457e-bb3e-64e1c53551f2"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" },
                    { new Guid("363253f2-45d6-4f7d-8ba3-33fa2830b135"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("48b42c9b-07e0-47da-81d6-f26ad50be585"), "Content", "Может редактировать посты", "CanEditPost" },
                    { new Guid("b28924a8-96d0-4f6f-9a4c-4458b1853d6c"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("c3fb4458-f700-4ab8-9abd-8fa438434b86"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("ce7bc9eb-525e-4917-a0be-3fd5eae086ae"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("da76217e-ad8b-4351-838f-a03fc557efce"), "UserManagement", "Может управлять пользователями", "CanManageUsers" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(376), new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(597) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(800), new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(801) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(802), new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(803) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(805), new DateTime(2026, 1, 17, 14, 29, 29, 191, DateTimeKind.Utc).AddTicks(805) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 14, 29, 29, 668, DateTimeKind.Utc).AddTicks(5487));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 14, 29, 29, 668, DateTimeKind.Utc).AddTicks(5723));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 29, 29, 533, DateTimeKind.Utc).AddTicks(1654), "$2a$11$OJd4JhSnMsr1AhPirrwdaeR4k3WC73Lw2DZMYDrWt35qPZPpWbb4.", new DateTime(2026, 1, 17, 14, 29, 29, 533, DateTimeKind.Utc).AddTicks(1658) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 14, 29, 29, 668, DateTimeKind.Utc).AddTicks(2950), "$2a$11$jyj9ouOwR1y6n5ewwSYUjO7VoLhi5jF/Ik7FWN6kUh42VhORV7gpi", new DateTime(2026, 1, 17, 14, 29, 29, 668, DateTimeKind.Utc).AddTicks(2953) });
        }
    }
}
