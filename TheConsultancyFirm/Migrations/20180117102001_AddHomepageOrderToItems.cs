using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class AddHomepageOrderToItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomepageOrder",
                table: "Solutions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomepageOrder",
                table: "NewsItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomepageOrder",
                table: "Cases",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HomepageCarousel",
                table: "Blocks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomepageOrder",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "HomepageOrder",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "HomepageOrder",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "HomepageCarousel",
                table: "Blocks");
        }
    }
}
