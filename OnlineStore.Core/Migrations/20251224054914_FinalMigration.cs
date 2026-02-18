using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class FinalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 49, 13, 873, DateTimeKind.Utc).AddTicks(262), new DateTime(2025, 12, 24, 5, 49, 13, 873, DateTimeKind.Utc).AddTicks(626) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 49, 13, 873, DateTimeKind.Utc).AddTicks(960), new DateTime(2025, 12, 24, 5, 49, 13, 873, DateTimeKind.Utc).AddTicks(961) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 49, 13, 873, DateTimeKind.Utc).AddTicks(964), new DateTime(2025, 12, 24, 5, 49, 13, 873, DateTimeKind.Utc).AddTicks(965) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2025, 12, 24, 5, 49, 13, 874, DateTimeKind.Utc).AddTicks(4634));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2025, 12, 24, 5, 49, 13, 874, DateTimeKind.Utc).AddTicks(4984));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 49, 13, 874, DateTimeKind.Utc).AddTicks(2861), new DateTime(2025, 12, 24, 5, 49, 13, 874, DateTimeKind.Utc).AddTicks(2862) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 49, 13, 874, DateTimeKind.Utc).AddTicks(2872), new DateTime(2025, 12, 24, 5, 49, 13, 874, DateTimeKind.Utc).AddTicks(2873) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 47, 13, 978, DateTimeKind.Utc).AddTicks(723), new DateTime(2025, 12, 24, 5, 47, 13, 978, DateTimeKind.Utc).AddTicks(1008) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 47, 13, 978, DateTimeKind.Utc).AddTicks(1271), new DateTime(2025, 12, 24, 5, 47, 13, 978, DateTimeKind.Utc).AddTicks(1272) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 47, 13, 978, DateTimeKind.Utc).AddTicks(1274), new DateTime(2025, 12, 24, 5, 47, 13, 978, DateTimeKind.Utc).AddTicks(1275) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2025, 12, 24, 5, 47, 13, 979, DateTimeKind.Utc).AddTicks(1512));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2025, 12, 24, 5, 47, 13, 979, DateTimeKind.Utc).AddTicks(1786));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 47, 13, 979, DateTimeKind.Utc).AddTicks(181), new DateTime(2025, 12, 24, 5, 47, 13, 979, DateTimeKind.Utc).AddTicks(182) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 47, 13, 979, DateTimeKind.Utc).AddTicks(191), new DateTime(2025, 12, 24, 5, 47, 13, 979, DateTimeKind.Utc).AddTicks(191) });
        }
    }
}
