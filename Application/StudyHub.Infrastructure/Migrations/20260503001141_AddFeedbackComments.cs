using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CanLeaderUpdate",
                table: "Schedules",
                newName: "CanHeadmanUpdate");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Comments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "FeedbackId",
                table: "Comments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FeedbackId",
                table: "Comments",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Feedbacks_FeedbackId",
                table: "Comments",
                column: "FeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Feedbacks_FeedbackId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_FeedbackId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "CanHeadmanUpdate",
                table: "Schedules",
                newName: "CanLeaderUpdate");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
