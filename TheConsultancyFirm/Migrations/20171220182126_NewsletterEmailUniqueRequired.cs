using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class NewsletterEmailUniqueRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NewsLetters",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsLetters_Email",
                table: "NewsLetters",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewsLetters_Email",
                table: "NewsLetters");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NewsLetters",
                nullable: true,
                oldClrType: typeof(string));}
    }
}
