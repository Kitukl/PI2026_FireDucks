using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSubjectsAndGroupPMI31 : Migration
    {
        private static readonly Guid ProgrammingSubjectId = new("d6579cbf-4f31-4a70-9f93-a447f2684ca1");
        private static readonly Guid EnglishSubjectId = new("f58f1d21-fe3a-4cb3-820a-9f0bb49d11c4");
        private static readonly Guid MathSubjectId = new("25461ea5-a563-4eba-a421-f6a0a0e6664d");
        private static readonly Guid Pmi31GroupId = new("0f0ee991-8f37-48f9-a0a2-bf31fcdf33f0");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                INSERT INTO "Subjects" ("Id", "Name")
                SELECT '{ProgrammingSubjectId}', 'Programming'
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Subjects" WHERE "Name" = 'Programming'
                );

                INSERT INTO "Subjects" ("Id", "Name")
                SELECT '{EnglishSubjectId}', 'English'
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Subjects" WHERE "Name" = 'English'
                );

                INSERT INTO "Subjects" ("Id", "Name")
                SELECT '{MathSubjectId}', 'Math'
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Subjects" WHERE "Name" = 'Math'
                );

                INSERT INTO "Groups" ("Id", "Name")
                SELECT '{Pmi31GroupId}', 'PMI-31'
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Groups" WHERE "Name" = 'PMI-31'
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Groups"
                WHERE "Name" = 'PMI-31';

                DELETE FROM "Subjects"
                WHERE "Name" IN ('Programming', 'English', 'Math');
                """);
        }
    }
}
