using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class Hilo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "EntityFrameworkHiLoSequence",
                schema: "tilt",
                incrementBy: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 214, DateTimeKind.Local).AddTicks(5210),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(9350));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "tilt",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 214, DateTimeKind.Local).AddTicks(1590),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(6120));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 214, DateTimeKind.Local).AddTicks(3480),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(8050));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 211, DateTimeKind.Local).AddTicks(7410),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 782, DateTimeKind.Local).AddTicks(2100));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "EntityFrameworkHiLoSequence",
                schema: "tilt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(9350),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 214, DateTimeKind.Local).AddTicks(5210));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "tilt",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(6120),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 214, DateTimeKind.Local).AddTicks(1590));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 784, DateTimeKind.Local).AddTicks(8050),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 214, DateTimeKind.Local).AddTicks(3480));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 7, 20, 45, 45, 782, DateTimeKind.Local).AddTicks(2100),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 11, 11, 25, 19, 211, DateTimeKind.Local).AddTicks(7410));
        }
    }
}
