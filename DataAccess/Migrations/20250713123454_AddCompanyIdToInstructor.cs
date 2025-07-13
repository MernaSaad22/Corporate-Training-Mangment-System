using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "Instructors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_CompanyId",
                table: "Instructors",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Compaines_CompanyId",
                table: "Instructors",
                column: "CompanyId",
                principalTable: "Compaines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Compaines_CompanyId",
                table: "Instructors");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_CompanyId",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Instructors");
        }
    }
}
