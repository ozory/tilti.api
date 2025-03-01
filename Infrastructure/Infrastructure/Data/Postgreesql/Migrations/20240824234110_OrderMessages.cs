using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class OrderMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_DriverId",
                schema: "tilt",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UserId",
                schema: "tilt",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "tilt",
                table: "Messages",
                newName: "TargetUserId");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                schema: "tilt",
                table: "Messages",
                newName: "SourceUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_UserId",
                schema: "tilt",
                table: "Messages",
                newName: "IX_Messages_TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_DriverId",
                schema: "tilt",
                table: "Messages",
                newName: "IX_Messages_SourceUserId");

            migrationBuilder.AlterColumn<long>(
                name: "TargetUserId",
                schema: "tilt",
                table: "Messages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<long>(
                name: "SourceUserId",
                schema: "tilt",
                table: "Messages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SourceUserId",
                schema: "tilt",
                table: "Messages",
                column: "SourceUserId",
                principalSchema: "tilt",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_TargetUserId",
                schema: "tilt",
                table: "Messages",
                column: "TargetUserId",
                principalSchema: "tilt",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SourceUserId",
                schema: "tilt",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_TargetUserId",
                schema: "tilt",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                schema: "tilt",
                table: "Messages",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "SourceUserId",
                schema: "tilt",
                table: "Messages",
                newName: "DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_TargetUserId",
                schema: "tilt",
                table: "Messages",
                newName: "IX_Messages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SourceUserId",
                schema: "tilt",
                table: "Messages",
                newName: "IX_Messages_DriverId");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                schema: "tilt",
                table: "Messages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 2)
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<long>(
                name: "DriverId",
                schema: "tilt",
                table: "Messages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 3)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_DriverId",
                schema: "tilt",
                table: "Messages",
                column: "DriverId",
                principalSchema: "tilt",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UserId",
                schema: "tilt",
                table: "Messages",
                column: "UserId",
                principalSchema: "tilt",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
