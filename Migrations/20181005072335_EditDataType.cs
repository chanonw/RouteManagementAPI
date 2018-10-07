using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RouteAPI.Migrations
{
    public partial class EditDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Zone_zoneId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_zoneId",
                table: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "zoneId",
                table: "Customer",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "NVARCHAR(50)");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_zoneId",
                table: "Customer",
                column: "zoneId",
                unique: true,
                filter: "[zoneId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Zone_zoneId",
                table: "Customer",
                column: "zoneId",
                principalTable: "Zone",
                principalColumn: "zoneId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Zone_zoneId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_zoneId",
                table: "Customer");

            migrationBuilder.AlterColumn<Guid>(
                name: "zoneId",
                table: "Customer",
                type: "NVARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_zoneId",
                table: "Customer",
                column: "zoneId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Zone_zoneId",
                table: "Customer",
                column: "zoneId",
                principalTable: "Zone",
                principalColumn: "zoneId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
