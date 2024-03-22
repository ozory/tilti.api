using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "security");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 7, 9, 23, 26, 300, DateTimeKind.Local).AddTicks(7920),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 1, 31, 19, 52, 17, 724, DateTimeKind.Local).AddTicks(720));

            migrationBuilder.CreateTable(
                name: "RefeshTokens",
                schema: "security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp", nullable: false),
                    RefeshToken = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefeshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefeshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "tilt",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefeshTokens_UserId",
                schema: "security",
                table: "RefeshTokens",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefeshTokens",
                schema: "security");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 31, 19, 52, 17, 724, DateTimeKind.Local).AddTicks(720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 7, 9, 23, 26, 300, DateTimeKind.Local).AddTicks(7920));
        }
    }
}
