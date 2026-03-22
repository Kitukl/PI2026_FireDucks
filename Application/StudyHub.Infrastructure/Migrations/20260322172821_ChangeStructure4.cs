using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStructure4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Reminder_ReminderId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminder",
                table: "Reminder");

            migrationBuilder.RenameTable(
                name: "Reminder",
                newName: "Reminders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Reminders_ReminderId",
                table: "AspNetUsers",
                column: "ReminderId",
                principalTable: "Reminders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Reminders_ReminderId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders");

            migrationBuilder.RenameTable(
                name: "Reminders",
                newName: "Reminder");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminder",
                table: "Reminder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Reminder_ReminderId",
                table: "AspNetUsers",
                column: "ReminderId",
                principalTable: "Reminder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
