using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class CreateDriverTransfersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "Sequence-DriverTransfers",
                schema: "tilt",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "DriverTransfers",
                schema: "tilt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AsaasTransferId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverTransfers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "tilt",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverTransfers_Users_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "tilt",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransfers_DriverId",
                schema: "tilt",
                table: "DriverTransfers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverTransfers_OrderId",
                schema: "tilt",
                table: "DriverTransfers",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverTransfers",
                schema: "tilt");

            migrationBuilder.DropSequence(
                name: "Sequence-DriverTransfers",
                schema: "tilt");
        }
    }
}
