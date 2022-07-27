using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Migrations
{
    public partial class AddColumnApprovalCCROOnGarmentSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalCC",
                table: "GarmentSections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalRO",
                table: "GarmentSections",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalCC",
                table: "GarmentSections");

            migrationBuilder.DropColumn(
                name: "ApprovalRO",
                table: "GarmentSections");
        }
    }
}
