using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class DynamicContentTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Solutions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "NewsItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Downloads",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Cases",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentType = table.Column<int>(nullable: false),
                    IdEn = table.Column<int>(nullable: false),
                    IdNl = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTranslations", x => x.Id);
                });

            migrationBuilder.Sql("UPDATE Solutions SET Language = 'nl';");
            migrationBuilder.Sql("UPDATE NewsItems SET Language = 'nl';");
            migrationBuilder.Sql("UPDATE Cases SET Language = 'nl';");
            migrationBuilder.Sql("UPDATE Downloads SET Language = 'nl';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemTranslations");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Downloads");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Cases");
        }
    }
}
