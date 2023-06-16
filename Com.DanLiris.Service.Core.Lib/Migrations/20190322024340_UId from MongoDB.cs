using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Migrations
{
    public partial class UIdfromMongoDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "StandardTests",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "UId",
            //    table: "Roles",
            //    type: "nvarchar(255)",
            //    maxLength: 255,
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "ProductSPPProperties",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "ProcessType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "UId",
            //    table: "Permissions",
            //    type: "nvarchar(255)",
            //    maxLength: 255,
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "LampStandard",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "FinishType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UId",
                table: "ColorTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "UId",
            //    table: "AccountRoles",
            //    type: "nvarchar(255)",
            //    maxLength: 255,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "UId",
            //    table: "AccountProfiles",
            //    type: "nvarchar(255)",
            //    maxLength: 255,
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UId",
                table: "StandardTests");

            //migrationBuilder.DropColumn(
            //    name: "UId",
            //    table: "Roles");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "ProductSPPProperties");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "ProcessType");

            //migrationBuilder.DropColumn(
            //    name: "UId",
            //    table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "LampStandard");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "FinishType");

            migrationBuilder.DropColumn(
                name: "UId",
                table: "ColorTypes");

            //migrationBuilder.DropColumn(
            //    name: "UId",
            //    table: "AccountRoles");

            //migrationBuilder.DropColumn(
            //    name: "UId",
            //    table: "AccountProfiles");
        }
    }
}
