using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Migrations
{
    public partial class MakeBlockInSlideRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Slide WHERE BlockId IS NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_Slide_Blocks_BlockId",
                table: "Slide");

            migrationBuilder.AlterColumn<int>(
                name: "BlockId",
                table: "Slide",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Slide_Blocks_BlockId",
                table: "Slide",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slide_Blocks_BlockId",
                table: "Slide");

            migrationBuilder.AlterColumn<int>(
                name: "BlockId",
                table: "Slide",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Slide_Blocks_BlockId",
                table: "Slide",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
