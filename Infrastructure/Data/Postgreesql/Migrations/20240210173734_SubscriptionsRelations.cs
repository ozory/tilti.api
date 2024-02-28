using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Plans_PlanId",
                schema: "tilt",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_PlanId",
                schema: "tilt",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 10, 14, 37, 34, 383, DateTimeKind.Local).AddTicks(2200),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 8, 16, 33, 27, 743, DateTimeKind.Local).AddTicks(2510));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 10, 14, 37, 34, 382, DateTimeKind.Local).AddTicks(8610),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 8, 16, 33, 27, 742, DateTimeKind.Local).AddTicks(9700));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 10, 14, 37, 34, 383, DateTimeKind.Local).AddTicks(820),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 8, 16, 33, 27, 743, DateTimeKind.Local).AddTicks(1370));

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                schema: "tilt",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Plans_PlanId",
                schema: "tilt",
                table: "Subscriptions",
                column: "PlanId",
                principalSchema: "tilt",
                principalTable: "Plans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Plans_PlanId",
                schema: "tilt",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_PlanId",
                schema: "tilt",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Users",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 8, 16, 33, 27, 743, DateTimeKind.Local).AddTicks(2510),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 10, 14, 37, 34, 383, DateTimeKind.Local).AddTicks(2200));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Subscriptions",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 8, 16, 33, 27, 742, DateTimeKind.Local).AddTicks(9700),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 10, 14, 37, 34, 382, DateTimeKind.Local).AddTicks(8610));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Updated",
                schema: "tilt",
                table: "Plans",
                type: "timestamp",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 8, 16, 33, 27, 743, DateTimeKind.Local).AddTicks(1370),
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValue: new DateTime(2024, 2, 10, 14, 37, 34, 383, DateTimeKind.Local).AddTicks(820));

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                schema: "tilt",
                table: "Subscriptions",
                column: "PlanId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Plans_PlanId",
                schema: "tilt",
                table: "Subscriptions",
                column: "PlanId",
                principalSchema: "tilt",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
