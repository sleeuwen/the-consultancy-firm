using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class UpdatedNewsletterToNewsletterSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewsLetters_Email",
                table: "NewsLetters");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NewsLetters");

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

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NewsLetters",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NewsLetters_Email",
                table: "NewsLetters",
                column: "Email",
                unique: true);
        }
    }
}
