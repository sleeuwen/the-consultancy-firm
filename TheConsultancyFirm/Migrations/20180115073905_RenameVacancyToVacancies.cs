using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class RenameVacancyToVacancies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacancy",
                table: "Vacancy");

            migrationBuilder.RenameTable(
                name: "Vacancy",
                newName: "Vacancies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacancies",
                table: "Vacancies",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Vacancies",
                table: "Vacancies");

            migrationBuilder.RenameTable(
                name: "Vacancies",
                newName: "Vacancy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vacancy",
                table: "Vacancy",
                column: "Id");
        }
    }
}
