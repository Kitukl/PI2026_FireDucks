using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("9a9aa371-da25-46cd-90d3-dded994118a8"), null, "Leader", "LEADER" },
                    { new Guid("c195766b-5b2d-4f5a-94de-f3d67114099b"), null, "Admin", "ADMIN" },
                    { new Guid("e94dc167-9d67-4d56-9d0c-92a193f5cc0f"), null, "Student", "STUDENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9a9aa371-da25-46cd-90d3-dded994118a8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c195766b-5b2d-4f5a-94de-f3d67114099b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("e94dc167-9d67-4d56-9d0c-92a193f5cc0f"));
        }
    }
}
