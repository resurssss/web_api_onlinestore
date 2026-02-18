using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 46, 26, 297, DateTimeKind.Utc).AddTicks(3666), new DateTime(2025, 12, 24, 5, 46, 26, 297, DateTimeKind.Utc).AddTicks(3949) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 46, 26, 297, DateTimeKind.Utc).AddTicks(4455), new DateTime(2025, 12, 24, 5, 46, 26, 297, DateTimeKind.Utc).AddTicks(4456) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 46, 26, 297, DateTimeKind.Utc).AddTicks(4459), new DateTime(2025, 12, 24, 5, 46, 26, 297, DateTimeKind.Utc).AddTicks(4460) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedAt",
                value: new DateTime(2025, 12, 24, 5, 46, 26, 298, DateTimeKind.Utc).AddTicks(5832));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "AssignedAt",
                value: new DateTime(2025, 12, 24, 5, 46, 26, 298, DateTimeKind.Utc).AddTicks(6100));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 46, 26, 298, DateTimeKind.Utc).AddTicks(4449), new DateTime(2025, 12, 24, 5, 46, 26, 298, DateTimeKind.Utc).AddTicks(4450) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 24, 5, 46, 26, 298, DateTimeKind.Utc).AddTicks(4457), new DateTime(2025, 12, 24, 5, 46, 26, 298, DateTimeKind.Utc).AddTicks(4458) });
        }
    }
}
