﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToolTemp.WPF.Migrations
{
    /// <inheritdoc />
    public partial class fixdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    devid = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ActiveId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.devid);
                });

            migrationBuilder.CreateTable(
                name: "dv_BusDataTemp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Factory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Line = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressMachine = table.Column<int>(type: "int", nullable: false),
                    Port = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Temp = table.Column<double>(type: "float", nullable: false),
                    Baudrate = table.Column<int>(type: "int", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsWarning = table.Column<bool>(type: "bit", nullable: false),
                    Sensor_Typeid = table.Column<int>(type: "int", nullable: false),
                    Sensor_kind = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    Sensor_ant = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dv_BusDataTemp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dv_Factory_Configs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Line = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dv_Factory_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dv_style",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Min = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Devid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Standard_temp = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Compensate_Vaild = table.Column<decimal>(type: "decimal(18,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dv_style", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "dv_BusDataTemp");

            migrationBuilder.DropTable(
                name: "dv_Factory_Configs");

            migrationBuilder.DropTable(
                name: "dv_style");
        }
    }
}
