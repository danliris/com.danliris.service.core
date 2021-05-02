using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Migrations
{
    public partial class FixProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Composition",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Construction",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Design",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Lot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WovenType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YarnType1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "YarnType2",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Composition",
                table: "Products",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Construction",
                table: "Products",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Design",
                table: "Products",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grade",
                table: "Products",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lot",
                table: "Products",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Width",
                table: "Products",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WovenType",
                table: "Products",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YarnType1",
                table: "Products",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YarnType2",
                table: "Products",
                maxLength: 256,
                nullable: true);
        }
    }
}
