using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class CancelRasonsAndMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelDescription",
                schema: "tilt",
                table: "Orders",
                type: "varchar(500)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AddColumn<string>(
                name: "CancelRasons",
                schema: "tilt",
                table: "Orders",
                type: "varchar(500)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "tilt",
                table: "Orders",
                type: "varchar(500)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 12);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleTime",
                schema: "tilt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "tilt",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "tilt",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "tilt",
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_Users_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "tilt",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "tilt",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DriverId",
                schema: "tilt",
                table: "Messages",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_OrderId",
                schema: "tilt",
                table: "Messages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                schema: "tilt",
                table: "Messages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages",
                schema: "tilt");

            migrationBuilder.DropColumn(
                name: "CancelDescription",
                schema: "tilt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CancelRasons",
                schema: "tilt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "tilt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ScheduleTime",
                schema: "tilt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "tilt",
                table: "Orders");
        }
    }
}
