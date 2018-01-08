using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class AddBlockProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Slide",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Blocks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkPath",
                table: "Blocks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkText",
                table: "Blocks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Slide");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "LinkPath",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "LinkText",
                table: "Blocks");
        }
    }
}
