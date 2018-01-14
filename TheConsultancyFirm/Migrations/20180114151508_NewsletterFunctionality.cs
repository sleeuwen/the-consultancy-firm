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
            migrationBuilder.DropIndex(
                name: "IX_NewsLetters_Email",
                table: "NewsLetters");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "NewsLetters",
                newName: "Subject");

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

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "NewsLetters",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "NewsletterIntroText",
                table: "NewsLetters",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewsletterOtherNews",
                table: "NewsLetters",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "NewsLetters",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "NewsletterSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubscription", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterSubscription_Email",
                table: "NewsletterSubscription",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsletterSubscription");

            migrationBuilder.DropColumn(
                name: "NewsletterIntroText",
                table: "NewsLetters");

            migrationBuilder.DropColumn(
                name: "NewsletterOtherNews",
                table: "NewsLetters");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "NewsLetters");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "NewsLetters",
                newName: "Email");

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

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "NewsLetters",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_NewsLetters_Email",
                table: "NewsLetters",
                column: "Email",
                unique: true);
        }
    }
}
