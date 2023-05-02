using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    tripId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    checkpoint1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    checkpoint2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.tripId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
