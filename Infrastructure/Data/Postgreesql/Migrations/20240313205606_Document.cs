using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class Document : Migration
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
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 182, DateTimeKind.Local).AddTicks(200),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 736, DateTimeKind.Local).AddTicks(4240));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "tilt",
                table: "Users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 5);

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
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(1530));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 177, DateTimeKind.Local).AddTicks(4980),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(8950));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 173, DateTimeKind.Local).AddTicks(8870),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 728, DateTimeKind.Local).AddTicks(2420));
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
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 736, DateTimeKind.Local).AddTicks(4240),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 182, DateTimeKind.Local).AddTicks(200));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "tilt",
                table: "Users",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250)
                .OldAnnotation("Relational:ColumnOrder", 5);

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
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(1530),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 176, DateTimeKind.Local).AddTicks(7620));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(8950),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 177, DateTimeKind.Local).AddTicks(4980));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 728, DateTimeKind.Local).AddTicks(2420),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 56, 6, 173, DateTimeKind.Local).AddTicks(8870));
        }
    }
}
