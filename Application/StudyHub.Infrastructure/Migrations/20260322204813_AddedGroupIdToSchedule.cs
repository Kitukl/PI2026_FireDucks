using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedGroupIdToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("00ee26fe-188e-47cf-b1a4-02fbdbe9b1de"), null, "Admin", "ADMIN" },
                    { new Guid("26b16825-b5b9-44a2-a511-79a88fbb58ed"), null, "Leader", "LEADER" },
                    { new Guid("dfda33d0-7f60-4e50-a329-1f33409b4545"), null, "Student", "STUDENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("00ee26fe-188e-47cf-b1a4-02fbdbe9b1de"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("26b16825-b5b9-44a2-a511-79a88fbb58ed"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("dfda33d0-7f60-4e50-a329-1f33409b4545"));

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
    }
}
