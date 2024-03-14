using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class Email : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 736, DateTimeKind.Local).AddTicks(4240),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 951, DateTimeKind.Local).AddTicks(4490));

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

            migrationBuilder.AddColumn<string>(
                name: "PaymentUserIdentifier",
                schema: "tilt",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(1530),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 946, DateTimeKind.Local).AddTicks(8180));

            migrationBuilder.AddColumn<string>(
                name: "PaymentToken",
                schema: "tilt",
                table: "Subscriptions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(8950),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 947, DateTimeKind.Local).AddTicks(5150));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 728, DateTimeKind.Local).AddTicks(2420),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 943, DateTimeKind.Local).AddTicks(7140));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUserIdentifier",
                schema: "tilt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaymentToken",
                schema: "tilt",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 951, DateTimeKind.Local).AddTicks(4490),
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 946, DateTimeKind.Local).AddTicks(8180),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(1530));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 947, DateTimeKind.Local).AddTicks(5150),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 731, DateTimeKind.Local).AddTicks(8950));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Orders",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 12, 12, 14, 34, 943, DateTimeKind.Local).AddTicks(7140),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 3, 13, 17, 44, 35, 728, DateTimeKind.Local).AddTicks(2420));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Document",
                schema: "tilt",
                table: "Users",
                columns: new[] { "Email", "Document" });
        }
    }
}
