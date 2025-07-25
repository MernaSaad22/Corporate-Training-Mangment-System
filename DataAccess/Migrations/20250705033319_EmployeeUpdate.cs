﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Compaines_CompanyId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Compaines_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "Compaines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Compaines_CompanyId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Compaines_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "Compaines",
                principalColumn: "Id");
        }
    }
}
