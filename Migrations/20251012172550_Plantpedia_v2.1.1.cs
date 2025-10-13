using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v211 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_Email_Used",
                table: "PasswordResets",
                columns: new[] { "Email", "Used" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_ExpiresAtUtc",
                table: "PasswordResets",
                column: "ExpiresAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResets");
        }
    }
}
