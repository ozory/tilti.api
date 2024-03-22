using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class UserIndex : Migration
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
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 951, DateTimeKind.Local).AddTicks(4490),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 44, DateTimeKind.Local).AddTicks(5110));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 946, DateTimeKind.Local).AddTicks(8180),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 39, DateTimeKind.Local).AddTicks(8480));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 947, DateTimeKind.Local).AddTicks(5150),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 40, DateTimeKind.Local).AddTicks(5440));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 943, DateTimeKind.Local).AddTicks(7140),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 36, DateTimeKind.Local).AddTicks(7430));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Document",
                schema: "tilt",
                table: "Users",
                columns: new[] { "Email", "Document" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_Document",
                schema: "tilt",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 44, DateTimeKind.Local).AddTicks(5110),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 951, DateTimeKind.Local).AddTicks(4490));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 39, DateTimeKind.Local).AddTicks(8480),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 946, DateTimeKind.Local).AddTicks(8180));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 40, DateTimeKind.Local).AddTicks(5440),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 947, DateTimeKind.Local).AddTicks(5150));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 3, 56, 36, DateTimeKind.Local).AddTicks(7430),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 943, DateTimeKind.Local).AddTicks(7140));
        }
    }
}
