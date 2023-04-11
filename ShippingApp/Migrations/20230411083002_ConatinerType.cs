﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class ConatinerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "containerWeight",
                table: "ContainerTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "containerWeight",
                table: "ContainerTypes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
