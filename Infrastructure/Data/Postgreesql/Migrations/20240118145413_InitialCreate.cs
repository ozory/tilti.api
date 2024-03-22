using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tilt");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "tilt",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Password = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Document = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    ValidationCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Photo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    VehicleDocument = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VehiclePlate = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    VehicleModel = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    VehicleYear = table.Column<short>(type: "smallint", nullable: true),
                    VehiclePhoto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    VerificationCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "tilt");
        }
    }
}
