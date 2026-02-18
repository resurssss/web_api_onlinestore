using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class RecreateFileMetadataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMetadata_Users_UserId",
                table: "FileMetadata");

            migrationBuilder.DropIndex(
                name: "IX_FileMetadata_UserId",
                table: "FileMetadata");

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

            migrationBuilder.DropColumn(
                name: "UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_UploadedBy",
                table: "FileMetadata",
                column: "UploadedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMetadata_Users_UploadedBy",
                table: "FileMetadata",
                column: "UploadedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMetadata_Users_UploadedBy",
                table: "FileMetadata");

            migrationBuilder.DropIndex(
                name: "IX_FileMetadata_UploadedBy",
                table: "FileMetadata");

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

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_UserId",
                table: "FileMetadata",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMetadata_Users_UserId",
                table: "FileMetadata",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
