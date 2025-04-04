using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class RateDescriptionColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "tilt",
                table: "Rates",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "tilt",
                table: "Rates",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "tilt",
                table: "Rates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "tilt",
                table: "Rates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 6);
        }
    }
}
