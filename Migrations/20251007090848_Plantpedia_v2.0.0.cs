using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comparison_history",
                columns: table => new
                {
                    history_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    plant_id_1 = table.Column<string>(type: "char(10)", nullable: false),
                    plant_id_2 = table.Column<string>(type: "char(10)", nullable: false),
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
                name: "discussion",
                columns: table => new
                {
                    discussion_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    plant_id = table.Column<string>(type: "char(10)", nullable: true),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false),
                    view_count = table.Column<int>(type: "integer", nullable: false),
                    PlantInfoPlantId = table.Column<string>(type: "char(10)", nullable: true)
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
                name: "plant_comparison",
                columns: table => new
                {
                    comparison_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "user_favorite",
                columns: table => new
                {
                    favorite_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PlantInfoPlantId = table.Column<string>(type: "char(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorite", x => x.favorite_id);
                    table.ForeignKey(
                        name: "FK_user_favorite_plant_info_PlantInfoPlantId",
                        column: x => x.PlantInfoPlantId,
                        principalTable: "plant_info",
                        principalColumn: "plant_id");
                    table.ForeignKey(
                        name: "FK_user_favorite_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_favorite_user_account_user_id",
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
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    parent_comment_id = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "plant_comparison_item",
                columns: table => new
                {
                    comparison_id = table.Column<int>(type: "integer", nullable: false),
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "discussion_reaction",
                columns: table => new
                {
                    reaction_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    discussion_id = table.Column<int>(type: "integer", nullable: true),
                    comment_id = table.Column<int>(type: "integer", nullable: true),
                    reaction_type = table.Column<char>(type: "char(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.InsertData(
                table: "plant_info",
                columns: new[] { "plant_id", "common_name", "created_date", "description", "harvest_date_days", "plant_type_id", "scientific_name", "updated_date" },
                values: new object[,]
                {
                    { "PL041", "Sầu riêng R6", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Giống sầu riêng R6 nổi tiếng ở Việt Nam, cơm vàng, hạt lép, hương vị ngọt béo.", 120, "TYPE02", "Durio zibethinus 'R6'", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL042", "Sầu riêng Dona (Monthong)", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Giống sầu riêng Dona (Monthong Thái Lan), cơm dày, vị ngọt nhẹ, được trồng phổ biến ở miền Nam.", 130, "TYPE02", "Durio zibethinus 'Dona'", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "plant_care",
                columns: new[] { "plant_id", "fertilizer_info", "growth_rate", "humidity_preference", "light_requirement", "soil_recommendation", "watering_needs" },
                values: new object[,]
                {
                    { "PL041", "Bón phân hữu cơ hoai mục đầu mùa mưa, bổ sung NPK 16-16-8 định kỳ. Tránh úng nước vì dễ thối rễ.", 1, null, 0, "Đất đỏ bazan hoặc đất phù sa cổ, tơi xốp, thoát nước tốt, pH 6.0-6.5.", 0 },
                    { "PL042", "Cần tưới đều trong mùa khô, bón NPK kết hợp phân hữu cơ vi sinh, đặc biệt giai đoạn nuôi trái.", 1, null, 0, "Thích hợp đất bazan, thoát nước tốt, có tầng canh tác sâu và giàu hữu cơ.", 0 }
                });

            migrationBuilder.InsertData(
                table: "plant_climate",
                columns: new[] { "climate_id", "plant_id" },
                values: new object[,]
                {
                    { "CLM01", "PL041" },
                    { "CLM01", "PL042" }
                });

            migrationBuilder.InsertData(
                table: "plant_img",
                columns: new[] { "image_id", "caption", "image_url", "plant_id" },
                values: new object[,]
                {
                    { "IMG041", "Sầu riêng R6 - cơm vàng, hạt lép, vị ngọt béo đậm đà", "https://img.lazcdn.com/g/p/f48e393476366397e4444dd9df86534a.jpg_720x720q80.jpg", "PL041" },
                    { "IMG042", "Vườn sầu riêng R6 tại Tây Nguyên - năng suất cao, trái lớn", "https://caygiong.tiendatbanme.com/wp-content/uploads/2024/06/dia_chi_cung_cap_giong_sau_rieng_ri6_monthon_musang_king_black_thorn.jpg", "PL041" },
                    { "IMG043", "Cắt ngang trái sầu riêng R6 - cơm vàng óng, mùi thơm đặc trưng", "https://bizweb.dktcdn.net/100/482/702/products/5-e189baf5-96d2-49af-8341-569e4bb7d9f5.jpg?v=1690703696620", "PL041" },
                    { "IMG044", "Sầu riêng Dona (Monthong) - cơm dày, hạt lép, vị ngọt thanh", "https://nongsanhaugiang.com.vn/images/10012020/93bf714a6b523023951f486f5b902be0.jpg", "PL042" },
                    { "IMG045", "Vườn sầu riêng Dona tại Lâm Đồng - năng suất ổn định", "https://thegioicaygiong.net/wp-content/uploads/2021/12/sau-rieng-dona-5.jpg", "PL042" },
                    { "IMG046", "Cơm sầu riêng Dona vàng nhạt, thơm nhẹ, dẻo và béo", "https://sfarm.vn/wp-content/uploads/2025/05/sau-rieng-thai-dona-la-gi-1.jpg", "PL042" }
                });

            migrationBuilder.InsertData(
                table: "plant_region",
                columns: new[] { "plant_id", "region_id" },
                values: new object[,]
                {
                    { "PL041", "REG01" },
                    { "PL041", "REG03" },
                    { "PL042", "REG01" },
                    { "PL042", "REG03" }
                });

            migrationBuilder.InsertData(
                table: "plant_soil",
                columns: new[] { "plant_id", "soil_type_id" },
                values: new object[,]
                {
                    { "PL041", "SOIL02" },
                    { "PL042", "SOIL02" }
                });

            migrationBuilder.InsertData(
                table: "plant_usage",
                columns: new[] { "plant_id", "usage_id" },
                values: new object[,]
                {
                    { "PL041", "USE01" },
                    { "PL042", "USE01" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_plant_comparison_user_id",
                table: "plant_comparison",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_comparison_item_plant_id",
                table: "plant_comparison_item",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_created_at",
                table: "user_favorite",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_plant_id",
                table: "user_favorite",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_PlantInfoPlantId",
                table: "user_favorite",
                column: "PlantInfoPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_user_id_plant_id",
                table: "user_favorite",
                columns: new[] { "user_id", "plant_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comparison_history");

            migrationBuilder.DropTable(
                name: "discussion_reaction");

            migrationBuilder.DropTable(
                name: "plant_comparison_item");

            migrationBuilder.DropTable(
                name: "user_favorite");

            migrationBuilder.DropTable(
                name: "discussion_comment");

            migrationBuilder.DropTable(
                name: "plant_comparison");

            migrationBuilder.DropTable(
                name: "discussion");

            migrationBuilder.DeleteData(
                table: "plant_care",
                keyColumn: "plant_id",
                keyValue: "PL041");

            migrationBuilder.DeleteData(
                table: "plant_care",
                keyColumn: "plant_id",
                keyValue: "PL042");

            migrationBuilder.DeleteData(
                table: "plant_climate",
                keyColumns: new[] { "climate_id", "plant_id" },
                keyValues: new object[] { "CLM01", "PL041" });

            migrationBuilder.DeleteData(
                table: "plant_climate",
                keyColumns: new[] { "climate_id", "plant_id" },
                keyValues: new object[] { "CLM01", "PL042" });

            migrationBuilder.DeleteData(
                table: "plant_img",
                keyColumn: "image_id",
                keyValue: "IMG041");

            migrationBuilder.DeleteData(
                table: "plant_img",
                keyColumn: "image_id",
                keyValue: "IMG042");

            migrationBuilder.DeleteData(
                table: "plant_img",
                keyColumn: "image_id",
                keyValue: "IMG043");

            migrationBuilder.DeleteData(
                table: "plant_img",
                keyColumn: "image_id",
                keyValue: "IMG044");

            migrationBuilder.DeleteData(
                table: "plant_img",
                keyColumn: "image_id",
                keyValue: "IMG045");

            migrationBuilder.DeleteData(
                table: "plant_img",
                keyColumn: "image_id",
                keyValue: "IMG046");

            migrationBuilder.DeleteData(
                table: "plant_region",
                keyColumns: new[] { "plant_id", "region_id" },
                keyValues: new object[] { "PL041", "REG01" });

            migrationBuilder.DeleteData(
                table: "plant_region",
                keyColumns: new[] { "plant_id", "region_id" },
                keyValues: new object[] { "PL041", "REG03" });

            migrationBuilder.DeleteData(
                table: "plant_region",
                keyColumns: new[] { "plant_id", "region_id" },
                keyValues: new object[] { "PL042", "REG01" });

            migrationBuilder.DeleteData(
                table: "plant_region",
                keyColumns: new[] { "plant_id", "region_id" },
                keyValues: new object[] { "PL042", "REG03" });

            migrationBuilder.DeleteData(
                table: "plant_soil",
                keyColumns: new[] { "plant_id", "soil_type_id" },
                keyValues: new object[] { "PL041", "SOIL02" });

            migrationBuilder.DeleteData(
                table: "plant_soil",
                keyColumns: new[] { "plant_id", "soil_type_id" },
                keyValues: new object[] { "PL042", "SOIL02" });

            migrationBuilder.DeleteData(
                table: "plant_usage",
                keyColumns: new[] { "plant_id", "usage_id" },
                keyValues: new object[] { "PL041", "USE01" });

            migrationBuilder.DeleteData(
                table: "plant_usage",
                keyColumns: new[] { "plant_id", "usage_id" },
                keyValues: new object[] { "PL042", "USE01" });

            migrationBuilder.DeleteData(
                table: "plant_info",
                keyColumn: "plant_id",
                keyValue: "PL041");

            migrationBuilder.DeleteData(
                table: "plant_info",
                keyColumn: "plant_id",
                keyValue: "PL042");
        }
    }
}
