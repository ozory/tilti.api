using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgreesql.Migrations
{
    /// <summary>
    /// Add DriverSubscriptions table for driver subscription model
    /// </summary>
    public partial class AddDriverSubscriptionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "driver_subscriptions",
                schema: "tilt",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    plan_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    payment_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    asaas_payment_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    asaas_payment_link = table.Column<string>(type: "text", nullable: true),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_driver_subscriptions_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_driver_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_driver_subscriptions_plan_id",
                schema: "tilt",
                table: "driver_subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_driver_subscriptions_user_id",
                schema: "tilt",
                table: "driver_subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_driver_subscriptions_user_id_status",
                schema: "tilt",
                table: "driver_subscriptions",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_driver_subscriptions_asaas_payment_id",
                schema: "tilt",
                table: "driver_subscriptions",
                column: "asaas_payment_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "driver_subscriptions",
                schema: "tilt");
        }
    }
}