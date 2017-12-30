using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class DbFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update current NULL records with valid values for the migration to be successful
            migrationBuilder.Sql("UPDATE NewsItems SET Title = '' WHERE Title IS NULL;");
            migrationBuilder.Sql("UPDATE Downloads SET Title = '' WHERE Title IS NULL;");
            migrationBuilder.Sql("UPDATE Downloads SET Description = '' WHERE Description IS NULL;");
            migrationBuilder.Sql("UPDATE Cases SET Title = '' WHERE Title IS NULL;");

            // Add dummy customer to set it on cases with a NULL CustomerId
            migrationBuilder.Sql("INSERT INTO Customers (Name, Link) VALUES ('Unspecified', '');");
            migrationBuilder.Sql("UPDATE Cases SET CustomerId = (SELECT Id FROM Customers Where Name = 'Unspecified') WHERE CustomerId IS NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Downloads_DownloadId",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Customers_CustomerId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Blocks_DownloadId",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "DownloadId",
                table: "Blocks");

            migrationBuilder.RenameColumn(
                name: "Readed",
                table: "Contacts",
                newName: "Read");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Solutions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "NewsItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Downloads",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Downloads",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Downloads",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Cases",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Cases",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Cases",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Customers_CustomerId",
                table: "Cases",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Customers_CustomerId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Downloads");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Cases");

            migrationBuilder.RenameColumn(
                name: "Read",
                table: "Contacts",
                newName: "Readed");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Solutions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "NewsItems",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Downloads",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Downloads",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Cases",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Cases",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "DownloadId",
                table: "Blocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_DownloadId",
                table: "Blocks",
                column: "DownloadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Downloads_DownloadId",
                table: "Blocks",
                column: "DownloadId",
                principalTable: "Downloads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Customers_CustomerId",
                table: "Cases",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
