using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class Init32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionsEnd",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmissionsStart",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AnnouncementDeadline",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompetitionDate",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompetitionDiscipline",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompetitionLocation",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouncilInitiative",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventLocation",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EventTime",
                table: "Articles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamClass",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExamDate",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamSubject",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolClass",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubstituteDate",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Teacher",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionsEnd",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AdmissionsStart",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AnnouncementDeadline",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CompetitionDate",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CompetitionDiscipline",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CompetitionLocation",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CouncilInitiative",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "EventLocation",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "EventTime",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ExamClass",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ExamDate",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ExamSubject",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SchoolClass",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SubstituteDate",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Teacher",
                table: "Articles");
        }
    }
}
