using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndFixReminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("23bbc9f1-2d54-4a0d-92a4-aa8a3447f298"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3269eca6-0005-40ca-9bd6-004dfd7c97b0"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b980fdd9-f47a-42a4-916d-e78763d3e324"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("506f89fd-27cf-40a5-b29f-3de520591cac"), null, "Admin", "ADMIN" },
                    { new Guid("6aae3aaa-3ac5-47f7-b9bb-7185a9f0ffb6"), null, "Student", "STUDENT" },
                    { new Guid("fbbb817a-99a1-4c10-8362-766bd0608cd0"), null, "Leader", "LEADER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("506f89fd-27cf-40a5-b29f-3de520591cac"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6aae3aaa-3ac5-47f7-b9bb-7185a9f0ffb6"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fbbb817a-99a1-4c10-8362-766bd0608cd0"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("23bbc9f1-2d54-4a0d-92a4-aa8a3447f298"), null, "Student", "STUDENT" },
                    { new Guid("3269eca6-0005-40ca-9bd6-004dfd7c97b0"), null, "Leader", "LEADER" },
                    { new Guid("b980fdd9-f47a-42a4-916d-e78763d3e324"), null, "Admin", "ADMIN" }
                });
        }
    }
}
