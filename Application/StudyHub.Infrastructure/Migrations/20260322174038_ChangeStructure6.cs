using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStructure6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3a49c9fe-24f3-4d9d-852f-767d3c580104"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d6a02956-023f-4b41-a30c-d47450b1eccb"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ebc12079-0632-4553-80a3-5e61f33cceb0"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { new Guid("3a49c9fe-24f3-4d9d-852f-767d3c580104"), null, "Leader", "LEADER" },
                    { new Guid("d6a02956-023f-4b41-a30c-d47450b1eccb"), null, "Student", "STUDENT" },
                    { new Guid("ebc12079-0632-4553-80a3-5e61f33cceb0"), null, "Admin", "ADMIN" }
                });
        }
    }
}
