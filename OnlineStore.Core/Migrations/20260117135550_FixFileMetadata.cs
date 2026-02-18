using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixFileMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("3bedd090-196a-4e02-bd30-592ffddfd7a9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("4ae7c66e-0578-4857-b755-e285aa52428b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("52c8702a-2964-4293-9b8f-1c74d4853d7a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("59d8c01f-02be-4621-88a2-d57c53fef675"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("96467d09-39f5-41ce-ae8c-eaa54652001a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("cd1b8cf6-6943-4a8c-a6bb-ee66477687c2"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d8bf0cca-ac30-4d73-b769-bda5e15ddf20"));

            migrationBuilder.AlterColumn<int>(
                name: "UploadedBy",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "CameraModel",
                table: "FileMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateTaken",
                table: "FileMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "FileMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Orientation",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "FileMetadata",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_FileMetadata_FileId",
                        column: x => x.FileId,
                        principalTable: "FileMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_FileId",
                table: "ProductImages",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

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

            migrationBuilder.DropColumn(
                name: "CameraModel",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "DateTaken",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "FileMetadata");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadedBy",
                table: "FileMetadata",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FileMetadata",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("3bedd090-196a-4e02-bd30-592ffddfd7a9"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("4ae7c66e-0578-4857-b755-e285aa52428b"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" },
                    { new Guid("52c8702a-2964-4293-9b8f-1c74d4853d7a"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("59d8c01f-02be-4621-88a2-d57c53fef675"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("96467d09-39f5-41ce-ae8c-eaa54652001a"), "OrderManagement", "Может управлять заказами", "CanManageOrders" },
                    { new Guid("cd1b8cf6-6943-4a8c-a6bb-ee66477687c2"), "UserManagement", "Может управлять пользователями", "CanManageUsers" },
                    { new Guid("d8bf0cca-ac30-4d73-b769-bda5e15ddf20"), "Content", "Может редактировать посты", "CanEditPost" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4413), new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4694) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4955), new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4955) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4957), new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4958) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4960), new DateTime(2026, 1, 17, 11, 3, 54, 928, DateTimeKind.Utc).AddTicks(4960) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 11, 3, 55, 411, DateTimeKind.Utc).AddTicks(7644));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 11, 3, 55, 411, DateTimeKind.Utc).AddTicks(7922));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 11, 3, 55, 250, DateTimeKind.Utc).AddTicks(9138), "$2a$11$BVXl4WYzX/OHB.PeQ43yKObR96Gg.CTZWdZ.CKejhJ3gARlZJGgIa", new DateTime(2026, 1, 17, 11, 3, 55, 250, DateTimeKind.Utc).AddTicks(9144) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 11, 3, 55, 411, DateTimeKind.Utc).AddTicks(5299), "$2a$11$Y6hCd0DOlZO.FTt0jgwr8e0jEF73Gu6vutzNF4KQs1bsdNwp7a7EW", new DateTime(2026, 1, 17, 11, 3, 55, 411, DateTimeKind.Utc).AddTicks(5303) });
        }
    }
}
