using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class RateOrderReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                schema: "tilt",
                table: "Rates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_OrderId",
                schema: "tilt",
                table: "Rates",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Orders_OrderId",
                schema: "tilt",
                table: "Rates",
                column: "OrderId",
                principalSchema: "tilt",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Orders_OrderId",
                schema: "tilt",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_OrderId",
                schema: "tilt",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "tilt",
                table: "Rates");
        }
    }
}
