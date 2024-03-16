using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTimeOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 16, 20, 14, 33, 4, DateTimeKind.Local).AddTicks(1220),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 590, DateTimeKind.Local).AddTicks(5720));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 585, DateTimeKind.Local).AddTicks(7560));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 586, DateTimeKind.Local).AddTicks(4800));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 582, DateTimeKind.Local).AddTicks(3860));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 590, DateTimeKind.Local).AddTicks(5720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 16, 20, 14, 33, 4, DateTimeKind.Local).AddTicks(1220));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 585, DateTimeKind.Local).AddTicks(7560),
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 586, DateTimeKind.Local).AddTicks(4800),
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 16, 20, 3, 29, 582, DateTimeKind.Local).AddTicks(3860),
                oldClrType: typeof(DateTime),
                oldType: "timestamp");
        }
    }
}
