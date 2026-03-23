using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixScheduleGroupRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Groups_GroupId1",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_GroupId1",
                table: "Schedules");

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

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Schedules");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("7f7da207-10a3-459f-abb4-552ffa806c22"), null, "Leader", "LEADER" },
                    { new Guid("bfa9c394-49ea-41a2-9ec5-bea6aae73dc8"), null, "Admin", "ADMIN" },
                    { new Guid("fb373214-cbcb-44aa-9a6e-27f94e6710f3"), null, "Student", "STUDENT" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7f7da207-10a3-459f-abb4-552ffa806c22"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bfa9c394-49ea-41a2-9ec5-bea6aae73dc8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fb373214-cbcb-44aa-9a6e-27f94e6710f3"));

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId1",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("00ee26fe-188e-47cf-b1a4-02fbdbe9b1de"), null, "Admin", "ADMIN" },
                    { new Guid("26b16825-b5b9-44a2-a511-79a88fbb58ed"), null, "Leader", "LEADER" },
                    { new Guid("dfda33d0-7f60-4e50-a329-1f33409b4545"), null, "Student", "STUDENT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_GroupId1",
                table: "Schedules",
                column: "GroupId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Groups_GroupId1",
                table: "Schedules",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
