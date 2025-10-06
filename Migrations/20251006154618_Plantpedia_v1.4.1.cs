using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class Plantpedia_v141 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "user_login_data",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "role",
                table: "user_login_data",
                type: "integer",
                maxLength: 25,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "user_account",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "avatar_url", "last_name" },
                values: new object[] { "https://www.ibm.com/content/dam/adobe-cms/instana/media_logo/AWS-EC2.png/_jcr_content/renditions/cq5dam.web.1280.1280.png", "Nguyen Minh A" });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "user_id", "avatar_url", "date_of_birth", "gender", "last_name" },
                values: new object[] { 2, "https://tse3.mm.bing.net/th/id/OIP.JMspq1z3Vm2m00ioNzUtEgHaHa?cb=12&rs=1&pid=ImgDetMain&o=7&rm=3", new DateTime(2004, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), 'M', "Nguyen Minh B" });

            migrationBuilder.UpdateData(
                table: "user_login_data",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "email", "role", "username" },
                values: new object[] { "winhtuan.dev@gmail.com", 2, "minha" });

            migrationBuilder.InsertData(
                table: "user_login_data",
                columns: new[] { "user_id", "created_at", "email", "last_login_at", "password_hash", "password_salt", "role", "username" },
                values: new object[] { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "winhtuan@gmail.com", null, "dD8tZsGrCCpE6ZJgyiv7s85HAfs6MI0L8ccPVZ6gOXQ=", "5W8Ubef8XcxAeznr0uPnWA==", 0, "minhb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_login_data",
                keyColumn: "user_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "user_account",
                keyColumn: "user_id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "email",
                table: "user_login_data");

            migrationBuilder.DropColumn(
                name: "role",
                table: "user_login_data");

            migrationBuilder.UpdateData(
                table: "user_account",
                keyColumn: "user_id",
                keyValue: 1,
                columns: new[] { "avatar_url", "last_name" },
                values: new object[] { "https://zando-ai.com/wp-content/uploads/2025/01/hero-img.png", "Admin" });

            migrationBuilder.UpdateData(
                table: "user_login_data",
                keyColumn: "user_id",
                keyValue: 1,
                column: "username",
                value: "admin");
        }
    }
}
