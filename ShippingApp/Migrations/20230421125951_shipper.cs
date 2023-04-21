using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class shipper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shippers",
                columns: table => new
                {
                    mapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    shipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    productType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    containerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    shipmentWeight = table.Column<float>(type: "real", nullable: false),
                    isAccepted = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    driverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    checkpoint1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    checkpoint2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shippers", x => x.mapId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shippers");
        }
    }
}
