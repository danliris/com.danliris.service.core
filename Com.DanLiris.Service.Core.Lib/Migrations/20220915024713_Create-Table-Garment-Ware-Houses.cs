using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Core.Lib.Migrations
{
    public partial class CreateTableGarmentWareHouses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentWareHouses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Address = table.Column<string>(maxLength: 3000, nullable: true),
                    Attention = table.Column<string>(maxLength: 50, nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    FaxNumber = table.Column<string>(maxLength: 50, nullable: true),
                    NPWP = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 50, nullable: true),
                    _CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _CreatedUtc = table.Column<DateTime>(nullable: false),
                    _DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _DeletedUtc = table.Column<DateTime>(nullable: false),
                    _IsDeleted = table.Column<bool>(nullable: false),
                    _LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    _LastModifiedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentWareHouses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentWareHouses");
        }
    }
}
