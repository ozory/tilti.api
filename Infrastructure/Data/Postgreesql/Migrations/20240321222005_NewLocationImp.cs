using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class NewLocationImp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                schema: "tilt",
                table: "Orders",
                type: "geography(POINT, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography(POINT, 4326)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                schema: "tilt",
                table: "Orders",
                type: "geography(POINT, 4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography(POINT, 4326)");
        }
    }
}
