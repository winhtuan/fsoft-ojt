using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v202 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "reaction_type",
                table: "plant_comment_reaction",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(char),
                oldType: "char(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<char>(
                name: "reaction_type",
                table: "plant_comment_reaction",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
