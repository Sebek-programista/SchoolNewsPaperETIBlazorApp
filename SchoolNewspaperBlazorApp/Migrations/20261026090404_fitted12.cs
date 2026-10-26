using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolNewspaperBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class fitted12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSubjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Substitutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LessonNumber = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    AbsentTeacherId = table.Column<int>(type: "int", nullable: true),
                    SubstituteTeacherId = table.Column<int>(type: "int", nullable: true),
                    OriginalSubjectId = table.Column<int>(type: "int", nullable: true),
                    SubstituteSubjectId = table.Column<int>(type: "int", nullable: true),
                    OriginalRoomId = table.Column<int>(type: "int", nullable: true),
                    SubstituteRoomId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Substitutions_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutions_SchoolRooms_OriginalRoomId",
                        column: x => x.OriginalRoomId,
                        principalTable: "SchoolRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutions_SchoolRooms_SubstituteRoomId",
                        column: x => x.SubstituteRoomId,
                        principalTable: "SchoolRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutions_SchoolSubjects_OriginalSubjectId",
                        column: x => x.OriginalSubjectId,
                        principalTable: "SchoolSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutions_SchoolSubjects_SubstituteSubjectId",
                        column: x => x.SubstituteSubjectId,
                        principalTable: "SchoolSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutions_Teachers_AbsentTeacherId",
                        column: x => x.AbsentTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Substitutions_Teachers_SubstituteTeacherId",
                        column: x => x.SubstituteTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_Name",
                table: "SchoolClasses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolRooms_Name",
                table: "SchoolRooms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSubjects_Name",
                table: "SchoolSubjects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_AbsentTeacherId",
                table: "Substitutions",
                column: "AbsentTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_Date_LessonNumber",
                table: "Substitutions",
                columns: new[] { "Date", "LessonNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_OriginalRoomId",
                table: "Substitutions",
                column: "OriginalRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_OriginalSubjectId",
                table: "Substitutions",
                column: "OriginalSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_SchoolClassId",
                table: "Substitutions",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_SubstituteRoomId",
                table: "Substitutions",
                column: "SubstituteRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_SubstituteSubjectId",
                table: "Substitutions",
                column: "SubstituteSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitutions_SubstituteTeacherId",
                table: "Substitutions",
                column: "SubstituteTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_FirstName_LastName",
                table: "Teachers",
                columns: new[] { "FirstName", "LastName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Substitutions");

            migrationBuilder.DropTable(
                name: "SchoolClasses");

            migrationBuilder.DropTable(
                name: "SchoolRooms");

            migrationBuilder.DropTable(
                name: "SchoolSubjects");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
