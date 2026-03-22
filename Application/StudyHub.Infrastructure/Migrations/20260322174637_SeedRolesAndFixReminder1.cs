using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndFixReminder1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "MicrosoftId",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("31e31b47-3096-474e-a38e-407207eef6fb"), null, "Student", "STUDENT" },
                    { new Guid("693207f1-8574-4fe7-aaf9-c7b31dd23a6e"), null, "Admin", "ADMIN" },
                    { new Guid("f563e27a-d472-4dea-a632-27efb6fc9d26"), null, "Leader", "LEADER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("31e31b47-3096-474e-a38e-407207eef6fb"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("693207f1-8574-4fe7-aaf9-c7b31dd23a6e"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f563e27a-d472-4dea-a632-27efb6fc9d26"));

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MicrosoftId",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

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
    }
}
