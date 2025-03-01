using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class RateDescriptionTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "tilt",
                table: "Rates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                schema: "tilt",
                table: "Rates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "tilt",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "Tags",
                schema: "tilt",
                table: "Rates");
        }
    }
}
