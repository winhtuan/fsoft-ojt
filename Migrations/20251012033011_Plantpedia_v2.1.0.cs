using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v210 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comparison_history");

            migrationBuilder.DropTable(
                name: "plant_comparison_item");

            migrationBuilder.DropTable(
                name: "plant_comparison");

            migrationBuilder.DropIndex(
                name: "IX_user_favorite_created_at",
                table: "user_favorite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comparison_history",
                columns: table => new
                {
                    history_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_id_1 = table.Column<string>(type: "char(10)", nullable: false),
                    plant_id_2 = table.Column<string>(type: "char(10)", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    compared_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comparison_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_comparison_history_plant_info_plant_id_1",
                        column: x => x.plant_id_1,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comparison_history_plant_info_plant_id_2",
                        column: x => x.plant_id_2,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comparison_history_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_comparison",
                columns: table => new
                {
                    comparison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_comparison", x => x.comparison_id);
                    table.ForeignKey(
                        name: "FK_plant_comparison_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_comparison_item",
                columns: table => new
                {
                    comparison_id = table.Column<int>(type: "integer", nullable: false),
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_comparison_item", x => new { x.comparison_id, x.plant_id });
                    table.ForeignKey(
                        name: "FK_plant_comparison_item_plant_comparison_comparison_id",
                        column: x => x.comparison_id,
                        principalTable: "plant_comparison",
                        principalColumn: "comparison_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_comparison_item_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_created_at",
                table: "user_favorite",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_history_plant_id_1_plant_id_2",
                table: "comparison_history",
                columns: new[] { "plant_id_1", "plant_id_2" });

            migrationBuilder.CreateIndex(
                name: "IX_comparison_history_plant_id_2",
                table: "comparison_history",
                column: "plant_id_2");

            migrationBuilder.CreateIndex(
                name: "IX_comparison_history_user_id",
                table: "comparison_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comparison_user_id",
                table: "plant_comparison",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comparison_item_plant_id",
                table: "plant_comparison_item",
                column: "plant_id");
        }
    }
}
