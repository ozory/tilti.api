using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class ValidationSalt : Migration
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
                defaultValue: new DateTime(2024, 1, 31, 19, 52, 17, 724, DateTimeKind.Local).AddTicks(720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 1, 31, 19, 19, 35, 330, DateTimeKind.Local).AddTicks(7710));

            migrationBuilder.AddColumn<string>(
                name: "ValidationSalt",
                schema: "tilt",
                table: "Users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationSalt",
                schema: "tilt",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 31, 19, 19, 35, 330, DateTimeKind.Local).AddTicks(7710),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 1, 31, 19, 52, 17, 724, DateTimeKind.Local).AddTicks(720));
        }
    }
}
