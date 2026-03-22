using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Schedules_ScheduleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Groups_GroupId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ScheduleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId1",
                table: "Schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_GroupId1",
                table: "Schedules",
                column: "GroupId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Groups_GroupId",
                table: "Schedules",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Groups_GroupId1",
                table: "Schedules",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Groups_GroupId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Groups_GroupId1",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_GroupId1",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Schedules");

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ScheduleId",
                table: "AspNetUsers",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Schedules_ScheduleId",
                table: "AspNetUsers",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Groups_GroupId",
                table: "Schedules",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
