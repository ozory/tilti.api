using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class DocumentEmailAdjusts : Migration
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
                defaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 412, DateTimeKind.Local).AddTicks(520),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 182, DateTimeKind.Local).AddTicks(200));

            migrationBuilder.AlterColumn<string>(
                name: "Document",
                schema: "tilt",
                table: "Users",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 407, DateTimeKind.Local).AddTicks(3680),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 176, DateTimeKind.Local).AddTicks(7620));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 408, DateTimeKind.Local).AddTicks(1670),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 177, DateTimeKind.Local).AddTicks(4980));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 404, DateTimeKind.Local).AddTicks(3700),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 173, DateTimeKind.Local).AddTicks(8870));

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
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 182, DateTimeKind.Local).AddTicks(200),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 412, DateTimeKind.Local).AddTicks(520));

            migrationBuilder.AlterColumn<string>(
                name: "Document",
                schema: "tilt",
                table: "Users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 176, DateTimeKind.Local).AddTicks(7620),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 407, DateTimeKind.Local).AddTicks(3680));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 177, DateTimeKind.Local).AddTicks(4980),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 408, DateTimeKind.Local).AddTicks(1670));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 173, DateTimeKind.Local).AddTicks(8870),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 18, 15, 16, 404, DateTimeKind.Local).AddTicks(3700));
        }
    }
}
