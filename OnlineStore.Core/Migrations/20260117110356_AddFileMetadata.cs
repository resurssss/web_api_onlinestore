using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFileMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
