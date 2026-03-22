using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStructure9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ad1a703f-f3bf-44aa-9ded-4bc37584d009"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b85d1dd6-f6e9-476c-b6da-ddb422e6e61a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("cbd95168-83ed-4d5d-b575-4761e104d38c"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { new Guid("ad1a703f-f3bf-44aa-9ded-4bc37584d009"), null, "Admin", "ADMIN" },
                    { new Guid("b85d1dd6-f6e9-476c-b6da-ddb422e6e61a"), null, "Leader", "LEADER" },
                    { new Guid("cbd95168-83ed-4d5d-b575-4761e104d38c"), null, "Student", "STUDENT" }
                });
        }
    }
}
