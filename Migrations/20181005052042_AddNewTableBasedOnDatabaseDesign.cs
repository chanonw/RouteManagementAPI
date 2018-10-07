using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace RouteAPI.Migrations
{
    public partial class AddNewTableBasedOnDatabaseDesign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Cars",
                table: "Cars");

            migrationBuilder.RenameTable(
                name: "Cars",
                newName: "Car");

            migrationBuilder.AddColumn<Guid>(
                name: "zoneId",
                table: "Car",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Car",
                table: "Car",
                column: "carCode");

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    warehouseId = table.Column<Guid>(type: "NVARCHAR(50)", nullable: false),
                    gps = table.Column<string>(nullable: true),
                    warehouseName = table.Column<string>(type: "NVARCHAR(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.warehouseId);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                columns: table => new
                {
                    zoneId = table.Column<Guid>(type: "NVARCHAR(50)", nullable: false),
                    warehouseId = table.Column<Guid>(nullable: true),
                    zoneName = table.Column<string>(type: "NVARCHAR(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.zoneId);
                    table.ForeignKey(
                        name: "FK_Zone_Warehouse_warehouseId",
                        column: x => x.warehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "warehouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    cusCode = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    building = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    city = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    cusCond = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    cusType = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    day = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    depBottle = table.Column<int>(nullable: false),
                    district = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    firstName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    gps = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    houseNo = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    lastName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    portalCode = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    road = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    soi = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    status = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    subDistrict = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    title = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    zoneId = table.Column<Guid>(type: "NVARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.cusCode);
                    table.ForeignKey(
                        name: "FK_Customer_Zone_zoneId",
                        column: x => x.zoneId,
                        principalTable: "Zone",
                        principalColumn: "zoneId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    deliveryId = table.Column<Guid>(type: "NVARCHAR(50)", nullable: false),
                    CustomercusCode = table.Column<string>(nullable: true),
                    carCode = table.Column<string>(nullable: true),
                    quantity = table.Column<int>(nullable: false),
                    status = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    transDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery", x => x.deliveryId);
                    table.ForeignKey(
                        name: "FK_Delivery_Customer_CustomercusCode",
                        column: x => x.CustomercusCode,
                        principalTable: "Customer",
                        principalColumn: "cusCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delivery_Car_carCode",
                        column: x => x.carCode,
                        principalTable: "Car",
                        principalColumn: "carCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Car_zoneId",
                table: "Car",
                column: "zoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_zoneId",
                table: "Customer",
                column: "zoneId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_CustomercusCode",
                table: "Delivery",
                column: "CustomercusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_carCode",
                table: "Delivery",
                column: "carCode");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_warehouseId",
                table: "Zone",
                column: "warehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_Zone_zoneId",
                table: "Car",
                column: "zoneId",
                principalTable: "Zone",
                principalColumn: "zoneId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_Zone_zoneId",
                table: "Car");

            migrationBuilder.DropTable(
                name: "Delivery");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Zone");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Car",
                table: "Car");

            migrationBuilder.DropIndex(
                name: "IX_Car_zoneId",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "zoneId",
                table: "Car");

            migrationBuilder.RenameTable(
                name: "Car",
                newName: "Cars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cars",
                table: "Cars",
                column: "carCode");
        }
    }
}
