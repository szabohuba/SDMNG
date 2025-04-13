using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDMNG.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRouteStopRelationForDatabaseCorreactio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stops_TransportRoutes_TransportRouteId",
                table: "Stops");

            migrationBuilder.DropIndex(
                name: "IX_Stops_TransportRouteId",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "EndLocation",
                table: "TransportRoutes");

            migrationBuilder.DropColumn(
                name: "StartLocation",
                table: "TransportRoutes");

            migrationBuilder.DropColumn(
                name: "TransportRouteId",
                table: "Stops");

            migrationBuilder.CreateTable(
                name: "RouteStops",
                columns: table => new
                {
                    RouteStopId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoutStopName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransportRouteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StopId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteStops", x => x.RouteStopId);
                    table.ForeignKey(
                        name: "FK_RouteStops_Stops_StopId",
                        column: x => x.StopId,
                        principalTable: "Stops",
                        principalColumn: "StopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteStops_TransportRoutes_TransportRouteId",
                        column: x => x.TransportRouteId,
                        principalTable: "TransportRoutes",
                        principalColumn: "TransportRoutesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_StopId",
                table: "RouteStops",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_TransportRouteId_SequenceNumber",
                table: "RouteStops",
                columns: new[] { "TransportRouteId", "SequenceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_TransportRouteId_StopId",
                table: "RouteStops",
                columns: new[] { "TransportRouteId", "StopId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteStops");

            migrationBuilder.AddColumn<string>(
                name: "EndLocation",
                table: "TransportRoutes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartLocation",
                table: "TransportRoutes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransportRouteId",
                table: "Stops",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_TransportRouteId",
                table: "Stops",
                column: "TransportRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stops_TransportRoutes_TransportRouteId",
                table: "Stops",
                column: "TransportRouteId",
                principalTable: "TransportRoutes",
                principalColumn: "TransportRoutesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
