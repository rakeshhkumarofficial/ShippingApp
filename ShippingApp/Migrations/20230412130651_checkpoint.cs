using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class checkpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Checkpoints",
                columns: table => new
                {
                    checkpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.checkpointId);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    MyProperty = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests", x => x.MyProperty);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Checkpoints");

            migrationBuilder.DropTable(
                name: "tests");
        }
    }
}
