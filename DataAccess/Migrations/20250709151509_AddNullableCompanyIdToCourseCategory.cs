using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableCompanyIdToCourseCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "CourseCategories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_CompanyId",
                table: "CourseCategories",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCategories_Compaines_CompanyId",
                table: "CourseCategories",
                column: "CompanyId",
                principalTable: "Compaines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseCategories_Compaines_CompanyId",
                table: "CourseCategories");

            migrationBuilder.DropIndex(
                name: "IX_CourseCategories_CompanyId",
                table: "CourseCategories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CourseCategories");
        }
    }
}
