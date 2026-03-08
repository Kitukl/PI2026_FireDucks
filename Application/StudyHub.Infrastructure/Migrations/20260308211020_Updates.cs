using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LecturerLessons_Lecturers_LecturersId",
                table: "LecturerLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturerLessons_Lessons_LessonsId",
                table: "LecturerLessons");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSchedules_Lessons_LessonsId",
                table: "LessonSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSchedules_Schedules_SchedulesId",
                table: "LessonSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatistics_Statistics_StatisticsId",
                table: "TaskStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatistics_Tasks_TasksId",
                table: "TaskStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStatistics_AspNetUsers_UsersId",
                table: "UserStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStatistics_Statistics_StatisticsId",
                table: "UserStatistics");

            migrationBuilder.DropTable(
                name: "LessonToSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserStatistics",
                table: "UserStatistics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskStatistics",
                table: "TaskStatistics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonSchedules",
                table: "LessonSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LecturerLessons",
                table: "LecturerLessons");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ReminderOffset",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "UserStatistics",
                newName: "StatisticUser");

            migrationBuilder.RenameTable(
                name: "TaskStatistics",
                newName: "StatisticTask");

            migrationBuilder.RenameTable(
                name: "LessonSchedules",
                newName: "LessonSchedule");

            migrationBuilder.RenameTable(
                name: "LecturerLessons",
                newName: "LecturerLesson");

            migrationBuilder.RenameIndex(
                name: "IX_UserStatistics_UsersId",
                table: "StatisticUser",
                newName: "IX_StatisticUser_UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskStatistics_TasksId",
                table: "StatisticTask",
                newName: "IX_StatisticTask_TasksId");

            migrationBuilder.RenameIndex(
                name: "IX_LessonSchedules_SchedulesId",
                table: "LessonSchedule",
                newName: "IX_LessonSchedule_SchedulesId");

            migrationBuilder.RenameIndex(
                name: "IX_LecturerLessons_LessonsId",
                table: "LecturerLesson",
                newName: "IX_LecturerLesson_LessonsId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Statistics",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<long>(
                name: "UpdateInterval",
                table: "Schedules",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "LessonsSlots",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "LessonsSlots",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<Guid>(
                name: "LessonsSlotId",
                table: "Lessons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Feedbacks",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "FeedbackType",
                table: "Feedbacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MicrosoftId",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ReminderId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatisticUser",
                table: "StatisticUser",
                columns: new[] { "StatisticsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatisticTask",
                table: "StatisticTask",
                columns: new[] { "StatisticsId", "TasksId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonSchedule",
                table: "LessonSchedule",
                columns: new[] { "LessonsId", "SchedulesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LecturerLesson",
                table: "LecturerLesson",
                columns: new[] { "LecturersId", "LessonsId" });

            migrationBuilder.CreateTable(
                name: "Reminder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReminderOffset = table.Column<long>(type: "bigint", nullable: false),
                    TimeType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminder", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LessonsSlotId",
                table: "Lessons",
                column: "LessonsSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ReminderId",
                table: "AspNetUsers",
                column: "ReminderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Reminder_ReminderId",
                table: "AspNetUsers",
                column: "ReminderId",
                principalTable: "Reminder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturerLesson_Lecturers_LecturersId",
                table: "LecturerLesson",
                column: "LecturersId",
                principalTable: "Lecturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturerLesson_Lessons_LessonsId",
                table: "LecturerLesson",
                column: "LessonsId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LessonsSlots_LessonsSlotId",
                table: "Lessons",
                column: "LessonsSlotId",
                principalTable: "LessonsSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSchedule_Lessons_LessonsId",
                table: "LessonSchedule",
                column: "LessonsId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSchedule_Schedules_SchedulesId",
                table: "LessonSchedule",
                column: "SchedulesId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatisticTask_Statistics_StatisticsId",
                table: "StatisticTask",
                column: "StatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatisticTask_Tasks_TasksId",
                table: "StatisticTask",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatisticUser_AspNetUsers_UsersId",
                table: "StatisticUser",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StatisticUser_Statistics_StatisticsId",
                table: "StatisticUser",
                column: "StatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Reminder_ReminderId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturerLesson_Lecturers_LecturersId",
                table: "LecturerLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturerLesson_Lessons_LessonsId",
                table: "LecturerLesson");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LessonsSlots_LessonsSlotId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSchedule_Lessons_LessonsId",
                table: "LessonSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_LessonSchedule_Schedules_SchedulesId",
                table: "LessonSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_StatisticTask_Statistics_StatisticsId",
                table: "StatisticTask");

            migrationBuilder.DropForeignKey(
                name: "FK_StatisticTask_Tasks_TasksId",
                table: "StatisticTask");

            migrationBuilder.DropForeignKey(
                name: "FK_StatisticUser_AspNetUsers_UsersId",
                table: "StatisticUser");

            migrationBuilder.DropForeignKey(
                name: "FK_StatisticUser_Statistics_StatisticsId",
                table: "StatisticUser");

            migrationBuilder.DropTable(
                name: "Reminder");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_LessonsSlotId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ReminderId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatisticUser",
                table: "StatisticUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatisticTask",
                table: "StatisticTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LessonSchedule",
                table: "LessonSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LecturerLesson",
                table: "LecturerLesson");

            migrationBuilder.DropColumn(
                name: "LessonsSlotId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "FeedbackType",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ReminderId",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "StatisticUser",
                newName: "UserStatistics");

            migrationBuilder.RenameTable(
                name: "StatisticTask",
                newName: "TaskStatistics");

            migrationBuilder.RenameTable(
                name: "LessonSchedule",
                newName: "LessonSchedules");

            migrationBuilder.RenameTable(
                name: "LecturerLesson",
                newName: "LecturerLessons");

            migrationBuilder.RenameIndex(
                name: "IX_StatisticUser_UsersId",
                table: "UserStatistics",
                newName: "IX_UserStatistics_UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_StatisticTask_TasksId",
                table: "TaskStatistics",
                newName: "IX_TaskStatistics_TasksId");

            migrationBuilder.RenameIndex(
                name: "IX_LessonSchedule_SchedulesId",
                table: "LessonSchedules",
                newName: "IX_LessonSchedules_SchedulesId");

            migrationBuilder.RenameIndex(
                name: "IX_LecturerLesson_LessonsId",
                table: "LecturerLessons",
                newName: "IX_LecturerLessons_LessonsId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Statistics",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "UpdateInterval",
                table: "Schedules",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "LessonsSlots",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "LessonsSlots",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Feedbacks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Feedbacks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Feedbacks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Feedbacks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "MicrosoftId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReminderOffset",
                table: "AspNetUsers",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserStatistics",
                table: "UserStatistics",
                columns: new[] { "StatisticsId", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskStatistics",
                table: "TaskStatistics",
                columns: new[] { "StatisticsId", "TasksId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LessonSchedules",
                table: "LessonSchedules",
                columns: new[] { "LessonsId", "SchedulesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LecturerLessons",
                table: "LecturerLessons",
                columns: new[] { "LecturersId", "LessonsId" });

            migrationBuilder.CreateTable(
                name: "LessonToSlot",
                columns: table => new
                {
                    LessonsId = table.Column<Guid>(type: "uuid", nullable: false),
                    LessonsSlotsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonToSlot", x => new { x.LessonsId, x.LessonsSlotsId });
                    table.ForeignKey(
                        name: "FK_LessonToSlot_LessonsSlots_LessonsSlotsId",
                        column: x => x.LessonsSlotsId,
                        principalTable: "LessonsSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonToSlot_Lessons_LessonsId",
                        column: x => x.LessonsId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonToSlot_LessonsSlotsId",
                table: "LessonToSlot",
                column: "LessonsSlotsId");

            migrationBuilder.AddForeignKey(
                name: "FK_LecturerLessons_Lecturers_LecturersId",
                table: "LecturerLessons",
                column: "LecturersId",
                principalTable: "Lecturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturerLessons_Lessons_LessonsId",
                table: "LecturerLessons",
                column: "LessonsId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSchedules_Lessons_LessonsId",
                table: "LessonSchedules",
                column: "LessonsId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LessonSchedules_Schedules_SchedulesId",
                table: "LessonSchedules",
                column: "SchedulesId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatistics_Statistics_StatisticsId",
                table: "TaskStatistics",
                column: "StatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatistics_Tasks_TasksId",
                table: "TaskStatistics",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStatistics_AspNetUsers_UsersId",
                table: "UserStatistics",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserStatistics_Statistics_StatisticsId",
                table: "UserStatistics",
                column: "StatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
