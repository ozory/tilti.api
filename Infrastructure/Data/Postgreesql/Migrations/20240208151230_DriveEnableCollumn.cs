using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class DriveEnableCollumn : Migration
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
                defaultValue: new DateTime(2024, 2, 8, 12, 12, 30, 344, DateTimeKind.Local).AddTicks(4020),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 7, 9, 23, 26, 300, DateTimeKind.Local).AddTicks(7920));

            migrationBuilder.AddColumn<bool>(
                name: "DriveEnable",
                schema: "tilt",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveEnable",
                schema: "tilt",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 7, 9, 23, 26, 300, DateTimeKind.Local).AddTicks(7920),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 8, 12, 12, 30, 344, DateTimeKind.Local).AddTicks(4020));
        }
    }
}
