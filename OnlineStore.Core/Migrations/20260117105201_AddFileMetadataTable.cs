using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFileMetadataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "FileMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    UploadedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DownloadCount = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileMetadata_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("3aa0cf4c-4831-4d2e-9c78-421a73e553b7"), "Analytics", "Может просматривать аналитику", "CanViewAnalytics" },
                    { new Guid("58be91cc-97f8-48ed-8bdf-eed4f6f060cf"), "UserManagement", "Может удалять пользователей", "CanDeleteUser" },
                    { new Guid("7610226d-09c7-4434-be5b-e13289e5a5ee"), "ProductManagement", "Может управлять продуктами", "CanManageProducts" },
                    { new Guid("7f891e8c-8ebf-4c86-a355-c10ed543ed86"), "UserManagement", "Может управлять пользователями", "CanManageUsers" },
                    { new Guid("d37e79f8-b15f-4ced-b38f-1fcb3c68fdd0"), "Content", "Может редактировать посты", "CanEditPost" },
                    { new Guid("ece196a2-24bd-4618-8310-d7c78320546d"), "Analytics", "Может просматривать отчеты", "CanViewReports" },
                    { new Guid("f55e1e61-1b9a-4bf3-ae87-59a18ec254f8"), "OrderManagement", "Может управлять заказами", "CanManageOrders" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(730), new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(1430) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(2086) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(2092), new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(2093) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(2098), new DateTime(2026, 1, 17, 10, 51, 59, 846, DateTimeKind.Utc).AddTicks(2099) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 10, 52, 0, 330, DateTimeKind.Utc).AddTicks(9308));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2026, 1, 17, 10, 52, 0, 330, DateTimeKind.Utc).AddTicks(9582));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 10, 52, 0, 173, DateTimeKind.Utc).AddTicks(130), "$2a$11$YQFYN.L0taQ0lOtki9hX/ewkXChFF.9zmOZvoiU6nRrDhtYKNmC9O", new DateTime(2026, 1, 17, 10, 52, 0, 173, DateTimeKind.Utc).AddTicks(136) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 17, 10, 52, 0, 330, DateTimeKind.Utc).AddTicks(6769), "$2a$11$r55oSFY5CPCie3Iz5mV4UeI7os.irv0.dJgqnJ/TskcSCqUNBHhQ2", new DateTime(2026, 1, 17, 10, 52, 0, 330, DateTimeKind.Utc).AddTicks(6772) });

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_UserId",
                table: "FileMetadata",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMetadata");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("3aa0cf4c-4831-4d2e-9c78-421a73e553b7"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("58be91cc-97f8-48ed-8bdf-eed4f6f060cf"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7610226d-09c7-4434-be5b-e13289e5a5ee"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7f891e8c-8ebf-4c86-a355-c10ed543ed86"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d37e79f8-b15f-4ced-b38f-1fcb3c68fdd0"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ece196a2-24bd-4618-8310-d7c78320546d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f55e1e61-1b9a-4bf3-ae87-59a18ec254f8"));

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
    }
}
