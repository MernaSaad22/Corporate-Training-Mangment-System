using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeLessonProgressEmployeeCourseProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeCourseProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    LessonProgress = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignmentProgress = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExamProgress = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCourseProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeCourseProgresses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeCourseProgresses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLessonProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLessonProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLessonProgresses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLessonProgresses_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCourseProgresses_CourseId",
                table: "EmployeeCourseProgresses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCourseProgresses_EmployeeId_CourseId",
                table: "EmployeeCourseProgresses",
                columns: new[] { "EmployeeId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLessonProgresses_EmployeeId_LessonId",
                table: "EmployeeLessonProgresses",
                columns: new[] { "EmployeeId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLessonProgresses_LessonId",
                table: "EmployeeLessonProgresses",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeCourseProgresses");

            migrationBuilder.DropTable(
                name: "EmployeeLessonProgresses");
        }
    }
}
