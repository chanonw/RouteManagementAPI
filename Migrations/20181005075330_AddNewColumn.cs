using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RouteAPI.Migrations
{
    public partial class AddNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Customer_CustomercusCode",
                table: "Delivery");

            migrationBuilder.RenameColumn(
                name: "CustomercusCode",
                table: "Delivery",
                newName: "cusCode");

            migrationBuilder.RenameIndex(
                name: "IX_Delivery_CustomercusCode",
                table: "Delivery",
                newName: "IX_Delivery_cusCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Customer_cusCode",
                table: "Delivery",
                column: "cusCode",
                principalTable: "Customer",
                principalColumn: "cusCode",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Customer_cusCode",
                table: "Delivery");

            migrationBuilder.RenameColumn(
                name: "cusCode",
                table: "Delivery",
                newName: "CustomercusCode");

            migrationBuilder.RenameIndex(
                name: "IX_Delivery_cusCode",
                table: "Delivery",
                newName: "IX_Delivery_CustomercusCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Customer_CustomercusCode",
                table: "Delivery",
                column: "CustomercusCode",
                principalTable: "Customer",
                principalColumn: "cusCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
