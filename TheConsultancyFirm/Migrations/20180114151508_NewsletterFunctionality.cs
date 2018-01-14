using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class NewsletterFunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NewsLetters",
                newName: "NewsletterSubscription");

            migrationBuilder.RenameIndex(
                name: "PK_NewsLetters",
                newName: "PK_NewsletterSubscription",
                table: "NewsletterSubscription");

            migrationBuilder.RenameIndex(
                name: "IX_NewsLetters_Email",
                newName: "IX_NewsletterSubscription_Email",
                table: "NewsletterSubscription");

            migrationBuilder.CreateTable(
                name: "NewsLetters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewsletterIntroText = table.Column<string>(nullable: false),
                    NewsletterOtherNews = table.Column<string>(nullable: true),
                    SentAt = table.Column<DateTime>(nullable: false),
                    Subject = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLetters", x => x.Id);
                });

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Solutions",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SharingDescription",
                table: "Solutions",
                maxLength: 140,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsLetters");

            migrationBuilder.RenameTable(
                name: "NewsletterSubscription",
                newName: "NewsLetters");

            migrationBuilder.AlterColumn<string>(
                name: "Summary",
                table: "Solutions",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "SharingDescription",
                table: "Solutions",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 140,
                oldNullable: true);
        }
    }
}
