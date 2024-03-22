using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class ColumnIdUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleDocument",
                schema: "tilt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VehiclePhoto",
                schema: "tilt",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ID",
                schema: "tilt",
                table: "Users",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationCode",
                schema: "tilt",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 27, 18, 57, 19, 18, DateTimeKind.Local).AddTicks(7600),
                oldClrType: typeof(DateTime),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                schema: "tilt",
                table: "Users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "tilt",
                table: "Users",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationCode",
                schema: "tilt",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 1, 27, 18, 57, 19, 18, DateTimeKind.Local).AddTicks(7600));

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                schema: "tilt",
                table: "Users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleDocument",
                schema: "tilt",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehiclePhoto",
                schema: "tilt",
                table: "Users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
