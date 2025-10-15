using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v212 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "user_account",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "user_account",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "user_account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "user_account",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_activity",
                columns: table => new
                {
                    activity_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    ref_id = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_activity", x => x.activity_id);
                    table.ForeignKey(
                        name: "FK_user_activity_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "user_account",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "created_at", "deleted_at", "status", "updated_at" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null });

            migrationBuilder.UpdateData(
                table: "user_account",
                keyColumn: "user_id",
                keyValue: 2,
                columns: new[] { "created_at", "deleted_at", "status", "updated_at" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_user_login_data_email",
                table: "user_login_data",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_login_data_username",
                table: "user_login_data",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_account_status",
                table: "user_account",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_user_activity_user_id_type_created_at",
                table: "user_activity",
                columns: new[] { "user_id", "type", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_activity");

            migrationBuilder.DropIndex(
                name: "IX_user_login_data_email",
                table: "user_login_data");

            migrationBuilder.DropIndex(
                name: "IX_user_login_data_username",
                table: "user_login_data");

            migrationBuilder.DropIndex(
                name: "IX_user_account_status",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "status",
                table: "user_account");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "user_account");
        }
    }
}
