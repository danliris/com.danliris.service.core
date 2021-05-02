using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Migrations
{
    public partial class COACodeUnitDivisionCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "COACode",
                table: "Units",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "COACode",
                table: "Divisions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportDebtCOA",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalDebtCOA",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchasingCOA",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StockCOA",
                table: "Categories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "COACode",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "COACode",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "ImportDebtCOA",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LocalDebtCOA",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PurchasingCOA",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "StockCOA",
                table: "Categories");
        }
    }
}
