using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v213 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "user_activity",
                columns: new[] { "activity_id", "created_at", "metadata", "ref_id", "type", "user_id" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 2, 8, 0, 0, 0, DateTimeKind.Utc), "{\"keyword\":\"cây xương rồng\"}", null, 3, 1 },
                    { 2L, new DateTime(2025, 1, 3, 9, 30, 0, 0, DateTimeKind.Utc), "{\"content\":\"Cây này đẹp quá!\"}", "cmt_101", 1, 1 },
                    { 3L, new DateTime(2025, 1, 3, 10, 0, 0, 0, DateTimeKind.Utc), "{\"emoji\":\"❤️\"}", "cmt_101", 2, 1 },
                    { 4L, new DateTime(2025, 1, 4, 14, 0, 0, 0, DateTimeKind.Utc), "{\"keyword\":\"hoa lan hồ điệp\"}", null, 3, 1 },
                    { 5L, new DateTime(2025, 1, 2, 11, 0, 0, 0, DateTimeKind.Utc), "{\"keyword\":\"cây trầu bà\"}", null, 3, 2 },
                    { 6L, new DateTime(2025, 1, 3, 16, 15, 0, 0, DateTimeKind.Utc), "{\"content\":\"Lá cây này có độc không?\"}", "cmt_201", 1, 2 },
                    { 7L, new DateTime(2025, 1, 3, 17, 0, 0, 0, DateTimeKind.Utc), "{\"emoji\":\"❤️\"}", "post_301", 2, 2 },
                    { 8L, new DateTime(2025, 1, 4, 10, 0, 0, 0, DateTimeKind.Utc), "{\"keyword\":\"sen đá ruby\"}", null, 3, 2 },
                    { 9L, new DateTime(2025, 1, 5, 9, 30, 0, 0, DateTimeKind.Utc), "{\"content\":\"Cảm ơn bài viết rất bổ ích!\"}", "cmt_202", 1, 2 },
                    { 10L, new DateTime(2025, 1, 5, 9, 45, 0, 0, DateTimeKind.Utc), "{\"emoji\":\"❤️\"}", "cmt_202", 2, 2 },
                    { 11L, new DateTime(2025, 1, 5, 10, 10, 0, 0, DateTimeKind.Utc), "{\"keyword\":\"cây ăn quả dễ trồng\"}", null, 3, 2 },
                    { 12L, new DateTime(2025, 1, 5, 10, 30, 0, 0, DateTimeKind.Utc), "{\"content\":\"Tưới mấy lần/tuần là hợp lý?\"}", "cmt_203", 1, 2 },
                    { 13L, new DateTime(2025, 1, 5, 10, 31, 0, 0, DateTimeKind.Utc), "{\"emoji\":\"❤️\"}", "cmt_203", 2, 2 },
                    { 14L, new DateTime(2025, 1, 6, 8, 0, 0, 0, DateTimeKind.Utc), "{\"keyword\":\"monstera deliciosa\"}", null, 3, 2 },
                    { 15L, new DateTime(2025, 1, 6, 8, 20, 0, 0, DateTimeKind.Utc), "{\"content\":\"Có chịu bóng râm không?\"}", "cmt_204", 1, 2 },
                    { 16L, new DateTime(2025, 1, 6, 8, 25, 0, 0, DateTimeKind.Utc), "{\"emoji\":\"❤️\"}", "post_305", 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "user_activity",
                keyColumn: "activity_id",
                keyValue: 16L);
        }
    }
}
