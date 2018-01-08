using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class Addedenabling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Solutions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SharingDescription",
                table: "Solutions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "NewsItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SharingDescription",
                table: "NewsItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Downloads",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Cases",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SharingDescription",
                table: "Cases",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE Solutions SET Enabled = 1;");
            migrationBuilder.Sql("UPDATE NewsItems SET Enabled = 1;");
            migrationBuilder.Sql("UPDATE Cases SET Enabled = 1;");
            migrationBuilder.Sql("UPDATE Downloads SET Enabled = 1;");
            migrationBuilder.Sql("UPDATE AspNetUsers SET Enabled = 1;");
            migrationBuilder.Sql("UPDATE Customers SET Enabled = 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "SharingDescription",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "SharingDescription",
                table: "NewsItems");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Downloads");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "SharingDescription",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "AspNetUsers");
        }
    }
}
