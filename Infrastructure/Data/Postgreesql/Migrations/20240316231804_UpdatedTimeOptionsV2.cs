using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTimeOptionsV2 : Migration
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
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 16, 20, 14, 33, 4, DateTimeKind.Local).AddTicks(1220));
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
                defaultValue: new DateTime(2024, 3, 16, 20, 14, 33, 4, DateTimeKind.Local).AddTicks(1220),
                oldClrType: typeof(DateTime),
                oldType: "timestamp");
        }
    }
}
