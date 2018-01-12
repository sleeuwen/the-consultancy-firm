using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class othernewsNullableSolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewsletterOtherNews",
                table: "NewsLetters",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NewsletterOtherNews",
                table: "NewsLetters",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
