using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v201 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "discussion_reaction");

            migrationBuilder.DropTable(
                name: "discussion_comment");

            migrationBuilder.DropTable(
                name: "discussion");

            migrationBuilder.CreateTable(
                name: "plant_comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    parent_comment_id = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_comment", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_plant_comment_plant_comment_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "plant_comment",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK_plant_comment_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_comment_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_comment_reaction",
                columns: table => new
                {
                    reaction_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    reaction_type = table.Column<char>(type: "char(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_comment_reaction", x => x.reaction_id);
                    table.ForeignKey(
                        name: "FK_plant_comment_reaction_plant_comment_comment_id",
                        column: x => x.comment_id,
                        principalTable: "plant_comment",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_comment_reaction_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_plant_comment_parent_comment_id",
                table: "plant_comment",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comment_plant_id",
                table: "plant_comment",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comment_user_id",
                table: "plant_comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comment_reaction_comment_id",
                table: "plant_comment_reaction",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comment_reaction_user_id_comment_id",
                table: "plant_comment_reaction",
                columns: new[] { "user_id", "comment_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plant_comment_reaction");

            migrationBuilder.DropTable(
                name: "plant_comment");

            migrationBuilder.CreateTable(
                name: "discussion",
                columns: table => new
                {
                    discussion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_id = table.Column<string>(type: "char(10)", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false),
                    PlantInfoPlantId = table.Column<string>(type: "char(10)", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    view_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussion", x => x.discussion_id);
                    table.ForeignKey(
                        name: "FK_discussion_plant_info_PlantInfoPlantId",
                        column: x => x.PlantInfoPlantId,
                        principalTable: "plant_info",
                        principalColumn: "plant_id");
                    table.ForeignKey(
                        name: "FK_discussion_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_discussion_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discussion_comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discussion_id = table.Column<int>(type: "integer", nullable: false),
                    parent_comment_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussion_comment", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_discussion_comment_discussion_comment_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "discussion_comment",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussion_comment_discussion_discussion_id",
                        column: x => x.discussion_id,
                        principalTable: "discussion",
                        principalColumn: "discussion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussion_comment_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discussion_reaction",
                columns: table => new
                {
                    reaction_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment_id = table.Column<int>(type: "integer", nullable: true),
                    discussion_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reaction_type = table.Column<char>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussion_reaction", x => x.reaction_id);
                    table.ForeignKey(
                        name: "FK_discussion_reaction_discussion_comment_comment_id",
                        column: x => x.comment_id,
                        principalTable: "discussion_comment",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussion_reaction_discussion_discussion_id",
                        column: x => x.discussion_id,
                        principalTable: "discussion",
                        principalColumn: "discussion_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussion_reaction_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_discussion_created_at",
                table: "discussion",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_plant_id",
                table: "discussion",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_PlantInfoPlantId",
                table: "discussion",
                column: "PlantInfoPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_user_id",
                table: "discussion",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_comment_created_at",
                table: "discussion_comment",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_comment_discussion_id",
                table: "discussion_comment",
                column: "discussion_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_comment_parent_comment_id",
                table: "discussion_comment",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_comment_user_id",
                table: "discussion_comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_reaction_comment_id",
                table: "discussion_reaction",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_reaction_discussion_id",
                table: "discussion_reaction",
                column: "discussion_id");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_reaction_user_id_comment_id",
                table: "discussion_reaction",
                columns: new[] { "user_id", "comment_id" },
                unique: true,
                filter: "comment_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_discussion_reaction_user_id_discussion_id",
                table: "discussion_reaction",
                columns: new[] { "user_id", "discussion_id" },
                unique: true,
                filter: "discussion_id IS NOT NULL");
        }
    }
}
