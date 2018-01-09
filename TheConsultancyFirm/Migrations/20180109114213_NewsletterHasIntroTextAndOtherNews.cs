using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class NewsletterHasIntroTextAndOtherNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "NewsLetters",
                newName: "NewsletterOtherNews");

            migrationBuilder.AddColumn<string>(
                name: "NewsletterIntroText",
                table: "NewsLetters",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewsletterIntroText",
                table: "NewsLetters");

            migrationBuilder.RenameColumn(
                name: "NewsletterOtherNews",
                table: "NewsLetters",
                newName: "Content");
        }
    }
}
