using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FPT_Plantpedia_Razor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "climate",
                columns: table => new
                {
                    climate_id = table.Column<string>(type: "char(10)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    temperature_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    rainfall_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    humidity_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_climate", x => x.climate_id);
                });

            migrationBuilder.CreateTable(
                name: "plant_type",
                columns: table => new
                {
                    plant_type_id = table.Column<string>(type: "char(10)", nullable: false),
                    type_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_type", x => x.plant_type_id);
                });

            migrationBuilder.CreateTable(
                name: "region",
                columns: table => new
                {
                    region_id = table.Column<string>(type: "char(10)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region", x => x.region_id);
                });

            migrationBuilder.CreateTable(
                name: "soil_type",
                columns: table => new
                {
                    soil_type_id = table.Column<string>(type: "char(10)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_soil_type", x => x.soil_type_id);
                });

            migrationBuilder.CreateTable(
                name: "usage",
                columns: table => new
                {
                    usage_id = table.Column<string>(type: "char(10)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usage", x => x.usage_id);
                });

            migrationBuilder.CreateTable(
                name: "user_account",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    last_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    gender = table.Column<char>(type: "char(1)", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "plant_info",
                columns: table => new
                {
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    scientific_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    common_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    plant_type_id = table.Column<string>(type: "char(10)", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    harvest_date_days = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_info", x => x.plant_id);
                    table.ForeignKey(
                        name: "FK_plant_info_plant_type_plant_type_id",
                        column: x => x.plant_type_id,
                        principalTable: "plant_type",
                        principalColumn: "plant_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login_data",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_salt = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login_data", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_login_data_user_account_user_id",
                        column: x => x.user_id,
                        principalTable: "user_account",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_care",
                columns: table => new
                {
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    light_requirement = table.Column<int>(type: "integer", nullable: false),
                    watering_needs = table.Column<int>(type: "integer", nullable: false),
                    humidity_preference = table.Column<int>(type: "integer", nullable: true),
                    growth_rate = table.Column<int>(type: "integer", nullable: true),
                    soil_recommendation = table.Column<string>(type: "text", nullable: false),
                    fertilizer_info = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_care", x => x.plant_id);
                    table.ForeignKey(
                        name: "FK_plant_care_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_climate",
                columns: table => new
                {
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    climate_id = table.Column<string>(type: "char(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_climate", x => new { x.plant_id, x.climate_id });
                    table.ForeignKey(
                        name: "FK_plant_climate_climate_climate_id",
                        column: x => x.climate_id,
                        principalTable: "climate",
                        principalColumn: "climate_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_climate_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_img",
                columns: table => new
                {
                    image_id = table.Column<string>(type: "char(10)", nullable: false),
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    caption = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_img", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_plant_img_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_region",
                columns: table => new
                {
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    region_id = table.Column<string>(type: "char(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_region", x => new { x.plant_id, x.region_id });
                    table.ForeignKey(
                        name: "FK_plant_region_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_region_region_region_id",
                        column: x => x.region_id,
                        principalTable: "region",
                        principalColumn: "region_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_soil",
                columns: table => new
                {
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    soil_type_id = table.Column<string>(type: "char(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_soil", x => new { x.plant_id, x.soil_type_id });
                    table.ForeignKey(
                        name: "FK_plant_soil_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_soil_soil_type_soil_type_id",
                        column: x => x.soil_type_id,
                        principalTable: "soil_type",
                        principalColumn: "soil_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_usage",
                columns: table => new
                {
                    plant_id = table.Column<string>(type: "char(10)", nullable: false),
                    usage_id = table.Column<string>(type: "char(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_usage", x => new { x.plant_id, x.usage_id });
                    table.ForeignKey(
                        name: "FK_plant_usage_plant_info_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plant_info",
                        principalColumn: "plant_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_usage_usage_usage_id",
                        column: x => x.usage_id,
                        principalTable: "usage",
                        principalColumn: "usage_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "climate",
                columns: new[] { "climate_id", "humidity_range", "name", "rainfall_range", "temperature_range" },
                values: new object[,]
                {
                    { "CLM01", "70% - 90%", "Nhiệt đới gió mùa", "1500mm - 2500mm/năm", "20°C - 35°C" },
                    { "CLM02", "60% - 80%", "Ôn đới núi cao", "1000mm - 2000mm/năm", "5°C - 20°C" },
                    { "CLM03", "75% - 95%", "Khí hậu ven biển", "1200mm - 2200mm/năm", "22°C - 32°C" },
                    { "CLM04", "40% - 60%", "Khí hậu khô hạn", "500mm - 1000mm/năm", "25°C - 38°C" },
                    { "CLM05", "65% - 85%", "Khí hậu mát mẻ", "1000mm - 1800mm/năm", "15°C - 25°C" }
                });

            migrationBuilder.InsertData(
                table: "plant_type",
                columns: new[] { "plant_type_id", "description", "type_name" },
                values: new object[,]
                {
                    { "TYPE01", "Cây có thân cứng, sống lâu năm, thường dùng làm bóng mát hoặc lấy gỗ.", "Cây thân gỗ" },
                    { "TYPE02", "Cây cho quả dùng làm thực phẩm, phổ biến trong nông nghiệp Việt Nam.", "Cây ăn quả" },
                    { "TYPE03", "Cây trồng để khai thác nguyên liệu như cao su, trà, cà phê.", "Cây công nghiệp" },
                    { "TYPE04", "Cây cung cấp thực phẩm chính như lúa, ngô, khoai.", "Cây lương thực" },
                    { "TYPE05", "Cây trồng để trang trí, làm đẹp không gian sống và công cộng.", "Cây cảnh" },
                    { "TYPE06", "Bao gồm các loại cây trồng ngắn ngày để thu hoạch lá, củ, quả dùng làm thực phẩm hoặc gia vị.", "Cây rau màu & gia vị" }
                });

            migrationBuilder.InsertData(
                table: "region",
                columns: new[] { "region_id", "name", "note" },
                values: new object[,]
                {
                    { "REG01", "Đồng bằng sông Cửu Long", "Vùng đất thấp, nhiều kênh rạch, khí hậu nhiệt đới gió mùa, thích hợp trồng lúa và cây ăn quả." },
                    { "REG02", "Đồng bằng sông Hồng", "Vùng nông nghiệp trọng điểm phía Bắc, đất phù sa màu mỡ, khí hậu cận nhiệt đới ẩm." },
                    { "REG03", "Tây Nguyên", "Vùng cao nguyên đất đỏ bazan, khí hậu ôn đới núi cao, thích hợp cây công nghiệp như cà phê, cao su." },
                    { "REG04", "Duyên hải Nam Trung Bộ", "Vùng ven biển khô hạn, đất cát pha, phù hợp cây chịu hạn và cây cảnh." },
                    { "REG05", "Trung du và miền núi Bắc Bộ", "Vùng đồi núi, khí hậu mát mẻ, đa dạng sinh học, thích hợp cây thuốc và cây rừng." }
                });

            migrationBuilder.InsertData(
                table: "soil_type",
                columns: new[] { "soil_type_id", "name", "note" },
                values: new object[,]
                {
                    { "SOIL01", "Đất phù sa", "Giàu dinh dưỡng, thích hợp trồng lúa và cây ăn quả." },
                    { "SOIL02", "Đất đỏ bazan", "Thường thấy ở Tây Nguyên, tốt cho cây công nghiệp." },
                    { "SOIL03", "Đất cát ven biển", "Thích hợp trồng rau màu, cây chịu hạn." },
                    { "SOIL04", "Đất mặn", "Cần cải tạo, có thể trồng cây chịu mặn như đước." },
                    { "SOIL05", "Đất đồi núi", "Thường dùng trồng cây lâu năm, cây rừng." }
                });

            migrationBuilder.InsertData(
                table: "usage",
                columns: new[] { "usage_id", "name", "note" },
                values: new object[,]
                {
                    { "USE01", "Thực phẩm", "Cây cung cấp quả, lá, củ hoặc hạt để ăn." },
                    { "USE02", "Dược liệu", "Cây có giá trị chữa bệnh trong y học cổ truyền." },
                    { "USE03", "Cảnh quan", "Cây dùng để trang trí, tạo không gian xanh." },
                    { "USE04", "Gỗ", "Cây thân gỗ dùng trong xây dựng, nội thất." },
                    { "USE05", "Nguyên liệu công nghiệp", "Cây dùng để sản xuất cao su, trà, cà phê, v.v." }
                });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "user_id", "avatar_url", "date_of_birth", "gender", "last_name" },
                values: new object[] { 1, "https://zando-ai.com/wp-content/uploads/2025/01/hero-img.png", new DateTime(2004, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), 'M', "Admin" });

            migrationBuilder.InsertData(
                table: "plant_info",
                columns: new[] { "plant_id", "common_name", "created_date", "description", "harvest_date_days", "plant_type_id", "scientific_name", "updated_date" },
                values: new object[,]
                {
                    { "PL001", "Bồ Đề", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây linh thiêng, lá hình tim, thường trồng ở chùa.", 1095, "TYPE01", "Ficus religiosa", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL002", "Bằng Lăng tím", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây thân gỗ, hoa tím rực rỡ, thường thấy ở công viên.", 1095, "TYPE01", "Lagerstroemia speciosa", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL003", "Ngọc Lan", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây hoa trắng thơm, thường trồng làm cảnh.", 1095, "TYPE01", "Michelia alba", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL004", "Phượng Vĩ", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây hoa đỏ rực, biểu tượng mùa hè học trò.", 1095, "TYPE01", "Delonix regia", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL005", "Xoài", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây ăn quả phổ biến, quả ngọt, giàu vitamin.", 730, "TYPE02", "Mangifera indica", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL006", "Ổi", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây ăn quả, lá và quả có giá trị dược liệu.", 365, "TYPE02", "Psidium guajava", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL007", "Dừa", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây nhiệt đới, cung cấp nước và dầu dừa.", 1460, "TYPE02", "Cocos nucifera", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL008", "Mít", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây ăn quả lớn, quả to, mùi thơm đặc trưng.", 730, "TYPE02", "Artocarpus heterophyllus", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL009", "Cà phê", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây công nghiệp, trồng nhiều ở Tây Nguyên.", 1095, "TYPE03", "Coffea robusta", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL010", "Cao su", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Cây công nghiệp, dùng để lấy mủ cao su.", 1825, "TYPE03", "Hevea brasiliensis", new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL011", "Trà", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Cây công nghiệp, dùng để chế biến trà.", 730, "TYPE03", "Camellia sinensis", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL012", "Hồ tiêu", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Cây gia vị, xuất khẩu mạnh của Việt Nam.", 1460, "TYPE03", "Piper nigrum", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL013", "Lúa", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Cây lương thực chính của Việt Nam.", 120, "TYPE04", "Oryza sativa", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL014", "Ngô", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Cây lương thực, dùng làm thức ăn và nguyên liệu.", 90, "TYPE04", "Zea mays", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL015", "Khoai lang", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Củ ngọt, dễ trồng, phổ biến ở nông thôn.", 120, "TYPE04", "Ipomoea batatas", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL016", "Sắn", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Cây lương thực, dùng làm bột và thức ăn chăn nuôi.", 210, "TYPE04", "Manihot esculenta", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL017", "Mai vàng", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Hoa Tết miền Nam, biểu tượng may mắn.", 730, "TYPE05", "Ochna integerrima", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL018", "Đào", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Hoa Tết miền Bắc, tượng trưng cho mùa xuân.", 730, "TYPE05", "Prunus persica", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL019", "Cúc vạn thọ", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Hoa trang trí, dễ trồng, màu vàng rực rỡ.", 90, "TYPE05", "Tagetes erecta", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL020", "Lan Hồ Điệp", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Hoa sang trọng, thường dùng làm quà tặng.", 365, "TYPE05", "Phalaenopsis spp.", new DateTime(2025, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL021", "Sưa đỏ", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây gỗ quý hiếm, có giá trị kinh tế rất cao, được bảo vệ.", 3650, "TYPE01", "Dalbergia tonkinensis", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL022", "Xà cừ", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây bóng mát phổ biến ở đô thị, gỗ dùng trong xây dựng và đồ mộc.", 2555, "TYPE01", "Khaya senegalensis", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL023", "Vải", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây ăn quả đặc sản miền Bắc, quả ngọt, mọng nước.", 1095, "TYPE02", "Litchi chinensis", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL024", "Sầu riêng", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Được mệnh danh là 'vua của các loại trái cây', mùi vị đặc trưng.", 1825, "TYPE02", "Durio zibethinus", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL025", "Thanh long", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây họ xương rồng, quả có vị ngọt mát, tốt cho sức khỏe.", 365, "TYPE02", "Hylocereus undatus", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL026", "Điều", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây công nghiệp trồng để lấy hạt, một sản phẩm xuất khẩu giá trị.", 1095, "TYPE03", "Anacardium occidentale", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL027", "Mía", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Nguyên liệu chính để sản xuất đường, nước giải khát phổ biến.", 300, "TYPE03", "Saccharum officinarum", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL028", "Đậu tương (Đậu nành)", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây họ đậu giàu protein, dùng làm thực phẩm và thức ăn chăn nuôi.", 100, "TYPE04", "Glycine max", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL029", "Lạc (Đậu phộng)", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây lương thực lấy củ, dùng để ép dầu hoặc làm thực phẩm.", 120, "TYPE04", "Arachis hypogaea", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL030", "Lưỡi hổ", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Cây cảnh nội thất phổ biến, có khả năng lọc không khí tốt, rất dễ chăm sóc.", 365, "TYPE05", "Dracaena trifasciata", new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL031", "Trầu bà", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Cây dây leo trong nhà, dễ trồng, có thể trồng đất hoặc thủy canh.", 180, "TYPE05", "Epipremnum aureum", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL032", "Hoa hồng", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Biểu tượng của tình yêu, có nhiều màu sắc và chủng loại khác nhau.", 120, "TYPE05", "Rosa chinensis", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL033", "Hoa súng", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Cây thủy sinh, hoa nở trên mặt nước, thường trồng trong ao, hồ.", 180, "TYPE05", "Nymphaea", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL034", "Rau muống", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Loại rau ăn lá phổ biến nhất trong bữa ăn của người Việt.", 30, "TYPE06", "Ipomoea aquatica", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL035", "Cà chua", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Cây ăn quả, được dùng như một loại rau trong ẩm thực.", 75, "TYPE06", "Solanum lycopersicum", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL036", "Dưa chuột", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Quả dùng để ăn sống, làm salad hoặc chế biến món ăn.", 50, "TYPE06", "Cucumis sativus", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL037", "Hành lá", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gia vị không thể thiếu trong nhiều món ăn, dễ trồng tại nhà.", 45, "TYPE06", "Allium fistulosum", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL038", "Ớt", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Quả có vị cay, dùng làm gia vị hoặc nguyên liệu cho các món ăn.", 90, "TYPE06", "Capsicum frutescens", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL039", "Xà lách", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Rau ăn sống phổ biến, thành phần chính của các món salad.", 60, "TYPE06", "Lactuca sativa", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "PL040", "Sả", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Cây gia vị có mùi thơm đặc trưng, dùng trong nấu ăn và y học.", 120, "TYPE06", "Cymbopogon citratus", new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "user_login_data",
                columns: new[] { "user_id", "created_at", "last_login_at", "password_hash", "password_salt", "username" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "dD8tZsGrCCpE6ZJgyiv7s85HAfs6MI0L8ccPVZ6gOXQ=", "5W8Ubef8XcxAeznr0uPnWA==", "admin" });

            migrationBuilder.InsertData(
                table: "plant_care",
                columns: new[] { "plant_id", "fertilizer_info", "growth_rate", "humidity_preference", "light_requirement", "soil_recommendation", "watering_needs" },
                values: new object[,]
                {
                    { "PL001", "Bón phân hữu cơ định kỳ hàng năm khi cây còn non. Cây trưởng thành không yêu cầu bón phân thường xuyên.", 1, null, 0, "Đất thoát nước tốt, giàu mùn.", 1 },
                    { "PL002", "Bón phân NPK có hàm lượng Kali cao trước mùa ra hoa để kích thích hoa nở rộ.", 0, null, 0, "Ưa đất tơi xốp, thoát nước tốt và giàu dinh dưỡng.", 1 },
                    { "PL003", "Bón phân hữu cơ hoai mục vào đầu mùa mưa. Bổ sung NPK định kỳ 2-3 tháng/lần.", 1, null, 0, "Đất chua nhẹ, giàu mùn và thoát nước tốt.", 1 },
                    { "PL004", "Không yêu cầu bón phân thường xuyên. Có thể bón bổ sung phân hữu cơ hàng năm để cây phát triển tốt hơn.", 0, null, 0, "Chịu được nhiều loại đất, kể cả đất nghèo dinh dưỡng, nhưng cần thoát nước tốt.", 2 },
                    { "PL005", "Bón phân theo giai đoạn: kiến thiết cơ bản, ra hoa, nuôi quả. Tăng cường Kali trước khi thu hoạch để quả ngọt hơn.", 1, null, 0, "Đất thịt nhẹ, phù sa, thoát nước tốt, pH từ 5.5-7.0.", 1 },
                    { "PL006", "Bón phân NPK và phân hữu cơ định kỳ sau mỗi vụ thu hoạch để cây phục hồi và cho quả vụ tiếp theo.", 0, null, 0, "Thích nghi rộng, nhưng tốt nhất là đất tơi xốp, giàu hữu cơ.", 1 },
                    { "PL007", "Cần nhiều Kali và Clo. Bón phân NPK có tỷ lệ Kali cao và bổ sung muối (NaCl) định kỳ.", 1, null, 0, "Ưa đất cát pha, đất phù sa ven biển có độ mặn nhẹ.", 0 },
                    { "PL008", "Bón nhiều phân hữu cơ và NPK. Tăng cường Kali khi quả bắt đầu lớn để tăng độ ngọt và phẩm chất.", 1, null, 0, "Đất thịt pha sét, thoát nước tốt, tầng canh tác dày.", 1 },
                    { "PL009", "Yêu cầu dinh dưỡng cao, bón phân cân đối NPK theo chu kỳ sinh trưởng, đặc biệt là sau thu hoạch và trước khi ra hoa.", 1, null, 1, "Đất bazan tơi xốp, giàu hữu cơ, thoát nước tốt.", 0 },
                    { "PL010", "Bón phân tập trung vào giai đoạn kiến thiết cơ bản (5-6 năm đầu) để cây nhanh đạt đường kính đủ tiêu chuẩn khai thác mủ.", 1, null, 0, "Đất xám, đất đỏ bazan, pH từ 4.5-6.5.", 0 },
                    { "PL011", "Cần nhiều Đạm (N) để phát triển búp non. Bón phân NPK giàu đạm sau mỗi lứa hái.", 0, null, 0, "Đất chua (pH 4.5-5.5), tơi xốp, giàu hữu cơ.", 0 },
                    { "PL012", "Kết hợp phân hữu cơ và phân vô cơ. Bón định kỳ, tăng cường Kali giai đoạn nuôi hạt để hạt chắc.", 1, null, 1, "Đất đỏ bazan hoặc đất xám, tơi xốp, thoát nước cực tốt để tránh bệnh thối rễ.", 1 },
                    { "PL013", "Yêu cầu bón phân theo từng giai đoạn: bón lót, bón thúc đẻ nhánh và bón đón đòng.", 0, null, 0, "Đất phù sa, đất sét hoặc đất thịt có khả năng giữ nước tốt.", 0 },
                    { "PL014", "Cần nhiều dinh dưỡng, đặc biệt là Đạm. Bón lót và bón thúc ở các giai đoạn cây 3-5 lá, xoáy nõn, trỗ cờ.", 0, null, 0, "Đất tơi xốp, giàu dinh dưỡng, thoát nước tốt.", 1 },
                    { "PL015", "Cần nhiều Kali để củ to và ngọt. Bón lót và bón thúc tập trung vào giai đoạn dây bắt đầu phát triển và hình thành củ.", 0, null, 0, "Đất cát pha, đất thịt nhẹ, tơi xốp để củ dễ phát triển.", 1 },
                    { "PL016", "Phản ứng tốt với phân bón, đặc biệt là Kali. Bón lót và bón thúc 1-2 lần trong 3 tháng đầu.", 0, null, 0, "Chịu được đất nghèo dinh dưỡng, đất dốc, nhưng cần thoát nước.", 2 },
                    { "PL017", "Bón phân định kỳ. Cuối năm cần tuốt lá và bón phân có hàm lượng Lân và Kali cao để kích thích ra hoa đúng dịp Tết.", 1, null, 0, "Đất thịt pha, tơi xốp, thoát nước tốt, giàu hữu cơ.", 1 },
                    { "PL018", "Tương tự như mai, cần tuốt lá và xử lý phân bón, nước để cây ra hoa đúng dịp Tết Nguyên Đán.", 1, null, 0, "Đất thịt nhẹ, đất phù sa, thoát nước tốt.", 1 },
                    { "PL019", "Bón phân NPK định kỳ 10-15 ngày/lần. Bấm ngọn để cây ra nhiều nhánh và cho nhiều hoa hơn.", 0, null, 0, "Đất tơi xốp, giàu dinh dưỡng, thoát nước tốt.", 1 },
                    { "PL020", "Sử dụng phân bón chuyên dụng cho lan, pha loãng. Bón định kỳ 1-2 tuần/lần, ngưng bón khi cây đang ra hoa.", 2, 0, 2, "Không trồng trong đất. Trồng trong giá thể chuyên dụng cho lan như vỏ thông, than củi, dớn.", 1 },
                    { "PL021", "Bón lót phân hữu cơ khi trồng. Bón thúc NPK định kỳ trong vài năm đầu để cây phát triển nhanh.", 2, null, 0, "Đất tơi xốp, giàu dinh dưỡng và thoát nước tốt.", 1 },
                    { "PL022", "Không yêu cầu bón phân thường xuyên, sức sống rất mạnh mẽ.", 0, null, 0, "Thích nghi tốt với nhiều loại đất, kể cả đất khô cằn.", 2 },
                    { "PL023", "Bón phân theo giai đoạn, tăng cường Lân và Kali để thúc đẩy ra hoa và đậu quả.", 1, null, 0, "Đất phù sa, giàu hữu cơ, thoát nước tốt.", 1 },
                    { "PL024", "Yêu cầu dinh dưỡng rất cao, cần bón phân hữu cơ và NPK thường xuyên, đặc biệt là Kali trong giai đoạn nuôi trái.", 1, 0, 0, "Đất thịt giàu hữu cơ, thoát nước cực tốt để tránh bệnh thối rễ.", 0 },
                    { "PL025", "Bón phân hữu cơ và NPK định kỳ, đặc biệt khi cây đang ra hoa và nuôi quả.", 0, null, 0, "Đất cát pha, tơi xốp, thoát nước tốt. Cần có trụ để leo.", 2 },
                    { "PL026", "Chịu được đất nghèo dinh dưỡng. Bón phân NPK vào đầu và cuối mùa mưa để tăng năng suất.", 1, null, 0, "Đất xám bạc màu, đất cát, thoát nước tốt.", 2 },
                    { "PL027", "Cần nhiều Đạm và Kali. Bón thúc nhiều lần trong giai đoạn vươn lóng.", 0, null, 0, "Đất phù sa, đất sét pha, giữ ẩm tốt.", 0 },
                    { "PL028", "Cây họ đậu có khả năng tự cố định đạm. Cần bón lót Lân và Kali, bón ít đạm.", 0, null, 0, "Đất thịt nhẹ, tơi xốp, thoát nước tốt.", 1 },
                    { "PL029", "Cần nhiều Lân, Kali và Vôi (Canxi) để củ chắc. Bón lót và bón thúc khi cây ra hoa rộ.", 0, null, 0, "Đất cát pha tơi xốp để củ dễ phát triển.", 1 },
                    { "PL030", "Hầu như không cần bón phân. Có thể bón phân loãng 1-2 lần vào mùa xuân.", 2, null, 1, "Hỗn hợp đất thoát nước tốt như đất cho xương rồng.", 2 },
                    { "PL031", "Bón phân cân bằng dạng lỏng mỗi tháng một lần vào mùa sinh trưởng.", 0, null, 1, "Đất trồng trong nhà thông thường, thoát nước tốt.", 1 },
                    { "PL032", "Là cây ưa phân. Bón phân hữu cơ và NPK định kỳ 2-3 tuần/lần để cây ra hoa liên tục.", 0, null, 0, "Đất giàu hữu cơ, tơi xốp và thoát nước tốt.", 1 },
                    { "PL033", "Sử dụng phân bón viên nén chuyên dụng cho cây thủy sinh, nhét vào gốc 1-2 tháng/lần.", 1, null, 0, "Đất bùn, đất sét nặng trong ao hồ hoặc chậu không có lỗ thoát nước.", 0 },
                    { "PL034", "Cần nhiều đạm để phát triển lá. Bón phân ure hoặc phân hữu cơ giàu đạm sau mỗi lần thu hoạch.", 0, null, 0, "Đất bùn hoặc đất thịt giữ nước tốt.", 0 },
                    { "PL035", "Bón lót và bón thúc định kỳ. Cần nhiều Canxi để tránh bệnh thối đít quả.", 0, null, 0, "Đất tơi xốp, giàu hữu cơ, thoát nước tốt.", 1 },
                    { "PL036", "Ưa phân hữu cơ. Bón thúc định kỳ bằng phân NPK khi cây bắt đầu ra quả.", 0, null, 0, "Đất tơi xốp, giàu dinh dưỡng. Cần làm giàn để leo.", 0 },
                    { "PL037", "Bón phân hữu cơ hoặc NPK giàu đạm sau mỗi lần cắt lá để cây nhanh ra lá mới.", 0, null, 0, "Đất tơi xốp, giàu mùn.", 1 },
                    { "PL038", "Bón NPK cân bằng. Tăng cường Kali khi cây ra hoa, đậu quả để quả chắc và cay hơn.", 1, null, 0, "Đất tơi xốp, thoát nước tốt.", 1 },
                    { "PL039", "Chủ yếu cần đạm để phát triển lá. Bón phân pha loãng định kỳ.", 0, null, 0, "Đất tơi xốp, giàu dinh dưỡng, giữ ẩm tốt.", 1 },
                    { "PL040", "Không kén phân bón. Bón một ít phân NPK hoặc phân hữu cơ vào mùa mưa để cây đẻ nhánh nhiều.", 0, null, 0, "Chịu được nhiều loại đất nhưng cần thoát nước tốt.", 1 }
                });

            migrationBuilder.InsertData(
                table: "plant_climate",
                columns: new[] { "climate_id", "plant_id" },
                values: new object[,]
                {
                    { "CLM01", "PL001" },
                    { "CLM01", "PL002" },
                    { "CLM03", "PL003" },
                    { "CLM01", "PL004" },
                    { "CLM01", "PL005" },
                    { "CLM01", "PL006" },
                    { "CLM03", "PL007" },
                    { "CLM01", "PL008" },
                    { "CLM02", "PL009" },
                    { "CLM02", "PL010" },
                    { "CLM02", "PL011" },
                    { "CLM01", "PL012" },
                    { "CLM01", "PL013" },
                    { "CLM04", "PL014" },
                    { "CLM04", "PL015" },
                    { "CLM04", "PL016" },
                    { "CLM01", "PL017" },
                    { "CLM05", "PL018" },
                    { "CLM01", "PL019" },
                    { "CLM05", "PL020" },
                    { "CLM01", "PL021" },
                    { "CLM01", "PL022" },
                    { "CLM01", "PL023" },
                    { "CLM01", "PL024" },
                    { "CLM04", "PL025" },
                    { "CLM04", "PL026" },
                    { "CLM01", "PL027" },
                    { "CLM01", "PL028" },
                    { "CLM04", "PL029" },
                    { "CLM01", "PL030" },
                    { "CLM01", "PL031" },
                    { "CLM05", "PL032" },
                    { "CLM01", "PL033" },
                    { "CLM01", "PL034" },
                    { "CLM01", "PL035" },
                    { "CLM01", "PL036" },
                    { "CLM01", "PL037" },
                    { "CLM01", "PL038" },
                    { "CLM05", "PL039" },
                    { "CLM01", "PL040" }
                });

            migrationBuilder.InsertData(
                table: "plant_img",
                columns: new[] { "image_id", "caption", "image_url", "plant_id" },
                values: new object[,]
                {
                    { "IMG001", "Cây Bồ Đề cổ thụ", "https://www.thuocdantoc.org/wp-content/uploads/2019/10/cay-bo-de.jpg", "PL001" },
                    { "IMG002", "Hoa Bằng Lăng tím", "https://bancuanhanong.com/img/images/products/mshong-bang-lang-giong0.jpg", "PL002" },
                    { "IMG003", "Hoa Ngọc Lan trắng", "https://misshoa.com/wp-content/uploads/2020/06/hinh-anh-hoa-ngoc-lan.jpg", "PL003" },
                    { "IMG004", "Hoa Phượng Vĩ đỏ", "https://baoquangbinh.vn/dataimages/202406/original/images786085_images786071_anh_hoa_phuong_vi_do_013046367.jpg", "PL004" },
                    { "IMG005", "Quả Xoài chín", "https://resource.kinhtedothi.vn/2024/06/26/1-1716882547.jpg", "PL005" },
                    { "IMG006", "Quả Ổi xanh", "https://cdn.youmed.vn/tin-tuc/wp-content/uploads/2020/09/oi5.png", "PL006" },
                    { "IMG007", "Cây Dừa ven biển", "https://khachsandayroi.com/wp-content/uploads/2020/04/1564978606054-trong-dua-bo-bien-3.jpg", "PL007" },
                    { "IMG008", "Quả Mít vàng", "https://static.kinhtedouong.vn/w640/images/upload/huongtra/08182025/qua-mit-1.jpg", "PL008" },
                    { "IMG009", "Cây Cà phê", "https://vinbarista.com/uploads/news/tim-hieu-cay-ca-phe-a-z-nguon-goc-dac-diem-sinh-hoc-phan-loai-202408131653.jpg", "PL009" },
                    { "IMG010", "Cây Cao su xanh mướt", "https://caosu.vn/files/assets/cs8.webp", "PL010" },
                    { "IMG011", "Lá Trà tươi", "https://suckhoehangngay.mediacdn.vn/154880486097817600/2020/11/10/cach-nau-la-tra-xanh-tuoi-de-uong-giam-can-16049983939621864329843.jpg", "PL011" },
                    { "IMG012", "Hồ Tiêu", "https://vnras.com/wp-content/uploads/2023/11/hat-tieu-2.jpg", "PL012" },
                    { "IMG013", "Ruộng Lúa xanh mướt", "https://media-cdn-v2.laodong.vn/storage/newsportal/2024/8/15/1380610/Vanhomc6.jpg", "PL013" },
                    { "IMG014", "Cây Ngô và bắp vàng", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQqV8SlyIe-Ufow8IL8qQ_KdEPhj3s8K6tcpQ&s", "PL014" },
                    { "IMG015", "Củ Khoai lang tím", "https://images2.thanhnien.vn/zoom/686_429/528068263637045248/2024/6/16/khoai-lang-tim-17185341801521870433944-1-0-384-612-crop-1718534203791803949402.jpg", "PL015" },
                    { "IMG016", "Củ Sắn trắng", "https://images2.thanhnien.vn/528068263637045248/2024/1/10/cu-san-khung-2-1704871816254294431810.jpg", "PL016" },
                    { "IMG017", "Hoa Mai vàng ngày Tết", "https://cdn2.fptshop.com.vn/unsafe/800x0/hinh_anh_hoa_mai_tet_1_d8a15be3ea.png", "PL017" },
                    { "IMG018", "Hoa Đào hồng rực rỡ", "https://media-cdn-v2.laodong.vn/Storage/NewsPortal/2023/1/31/1142999/Hoa-Anh-Dao-4.JPG", "PL018" },
                    { "IMG019", "Hoa Cúc vạn thọ vàng", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQfh5HbPKm23X2BbSWsI5QoWBZV1fAySEtvVA&s", "PL019" },
                    { "IMG020", "Hoa Lan Hồ Điệp sang trọng", "https://lanhodiep.vn/wp-content/uploads/2021/10/Lan-ho-diep-mau-cam-mang-ve-dep-tao-nha-sang-trong.jpg", "PL020" },
                    { "IMG021", "Gỗ cây Sưa đỏ quý hiếm", "https://i.etsystatic.com/18026585/r/il/552977/1859839778/il_794xN.1859839778_vvh3.jpg", "PL021" },
                    { "IMG022", "Cây Xà cừ tỏa bóng mát", "https://thegioicaycongtrinh.com/wp-content/uploads/2022/07/cay-xa-cu-cong-trinh-gia-re-2.jpg", "PL022" },
                    { "IMG023", "Chùm Vải thiều chín mọng", "https://bacgiang.gov.vn/documents/20181/8745446/1592886137045_trai+nghiem+vai+3.jpg/5c7481ce-fb26-43c1-ba0b-8c5ba9601054?t=1592886137049", "PL023" },
                    { "IMG024", "Quả Sầu riêng gai góc", "https://static-images.vnncdn.net/files/publish/2023/5/25/sau-rieng-1-426.jpg", "PL024" },
                    { "IMG025", "Quả Thanh long ruột đỏ", "https://thuyanhfruits.com/wp-content/uploads/2020/11/thanh-long-do.jpg", "PL025" },
                    { "IMG026", "Hạt Điều rang muối", "https://traicaysaynutfarm.com/wp-content/uploads/2021/03/hat-dieu-1.jpg", "PL026" },
                    { "IMG027", "Ly nước Mía mát lạnh", "https://befresh.vn/wp-content/uploads/2023/04/9-tac-dung-cua-nuoc-mia-khien-ban.jpeg-1024x669.jpeg", "PL027" },
                    { "IMG028", "Hạt Đậu tương (Đậu nành)", "https://bizweb.dktcdn.net/100/363/007/articles/dau-tuong-dau-nanh-9.jpg?v=1623683502643", "PL028" },
                    { "IMG029", "Củ Lạc (Đậu phộng) tươi", "https://file.hstatic.net/1000175970/file/nguyen_lieu__1__29e0d3aacd2c4a66b8491e010314954b_grande.jpg", "PL029" },
                    { "IMG030", "Cây Lưỡi hổ trong chậu", "https://vuachaukieng.com/cdn/static/news/2022/637b54d8ca904_860_keep_ratio.jpg", "PL030" },
                    { "IMG031", "Cây Trầu bà leo cột", "", "PL031" },
                    { "IMG032", "Bông Hoa hồng đỏ thắm", "https://img2.thuthuatphanmem.vn/uploads/2019/03/09/bong-hoa-hong-do_114633861.jpg", "PL032" },
                    { "IMG033", "Hoa súng nở trên mặt hồ", "https://cdn.tgdd.vn/Files/2021/07/28/1371313/cach-trong-hoa-sung-trong-chau-kieng-don-gian-cho-bong-no-quanh-nam-202107280257108539.jpg", "PL033" },
                    { "IMG034", "Đĩa rau muống xào tỏi", "https://tse3.mm.bing.net/th/id/OIP.32aW-7tU2FRki2BVXyqYTwHaEn?r=0&rs=1&pid=ImgDetMain&o=7&rm=3", "PL034" },
                    { "IMG035", "Những quả Cà chua chín đỏ", "https://tse2.mm.bing.net/th/id/OIP.ZkznhhKneM69DFNWhEOSAwHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3", "PL035" },
                    { "IMG036", "Dưa chuột tươi ngon", "https://cdn.tgdd.vn/Files/2021/07/21/1369776/dua-chuot-bao-nhieu-calo-an-dua-chuot-co-giam-can-khong-202107211519389648.jpg", "PL036" },
                    { "IMG037", "Hành lá xanh tươi", "https://tse3.mm.bing.net/th/id/OIP.hOD5htl_uf_Y5zhW15toVwAAAA?r=0&rs=1&pid=ImgDetMain&o=7&rm=3", "PL037" },
                    { "IMG038", "Quả Ớt đỏ cay nồng", "https://suckhoedoisong.qltns.mediacdn.vn/324455921873985536/2022/6/18/cach-an-che-bien-bao-quan-ot-2-1655566236587196946971.jpg", "PL038" },
                    { "IMG039", "Rau Xà lách tươi xanh", "https://th.bing.com/th/id/R.2dd5796db3374f0920f419c8bae8fbd0?rik=O6LqdjvSbzBfiA&riu=http%3a%2f%2fcdn.tgdd.vn%2fFiles%2f2019%2f12%2f03%2f1224593%2fcach-trong-rau-xa-lach-xanh-tuoi-tai-nha-202201101405579038.jpg&ehk=b5s2XxSdxHzQ%2fVe4nSUGWei8c3sSjOuEVZWtvGtcUxc%3d&risl=&pid=ImgRaw&r=0", "PL039" },
                    { "IMG040", "Cây Sả và tinh dầu", "https://vnras.com/wp-content/uploads/2023/11/sa-2-1024x725.jpg", "PL040" }
                });

            migrationBuilder.InsertData(
                table: "plant_region",
                columns: new[] { "plant_id", "region_id" },
                values: new object[,]
                {
                    { "PL001", "REG02" },
                    { "PL002", "REG01" },
                    { "PL003", "REG04" },
                    { "PL004", "REG01" },
                    { "PL005", "REG03" },
                    { "PL006", "REG01" },
                    { "PL007", "REG04" },
                    { "PL008", "REG01" },
                    { "PL009", "REG03" },
                    { "PL010", "REG03" },
                    { "PL011", "REG05" },
                    { "PL012", "REG01" },
                    { "PL013", "REG01" },
                    { "PL014", "REG04" },
                    { "PL015", "REG04" },
                    { "PL016", "REG04" },
                    { "PL017", "REG01" },
                    { "PL018", "REG05" },
                    { "PL019", "REG02" },
                    { "PL020", "REG05" },
                    { "PL021", "REG05" },
                    { "PL022", "REG02" },
                    { "PL023", "REG05" },
                    { "PL024", "REG01" },
                    { "PL025", "REG04" },
                    { "PL026", "REG03" },
                    { "PL027", "REG01" },
                    { "PL028", "REG01" },
                    { "PL029", "REG02" },
                    { "PL030", "REG01" },
                    { "PL031", "REG02" },
                    { "PL032", "REG05" },
                    { "PL033", "REG01" },
                    { "PL034", "REG02" },
                    { "PL035", "REG05" },
                    { "PL036", "REG02" },
                    { "PL037", "REG01" },
                    { "PL038", "REG04" },
                    { "PL039", "REG05" },
                    { "PL040", "REG01" }
                });

            migrationBuilder.InsertData(
                table: "plant_soil",
                columns: new[] { "plant_id", "soil_type_id" },
                values: new object[,]
                {
                    { "PL001", "SOIL01" },
                    { "PL002", "SOIL05" },
                    { "PL003", "SOIL01" },
                    { "PL004", "SOIL03" },
                    { "PL005", "SOIL01" },
                    { "PL006", "SOIL01" },
                    { "PL007", "SOIL03" },
                    { "PL008", "SOIL01" },
                    { "PL009", "SOIL02" },
                    { "PL010", "SOIL02" },
                    { "PL011", "SOIL02" },
                    { "PL012", "SOIL01" },
                    { "PL013", "SOIL01" },
                    { "PL014", "SOIL04" },
                    { "PL015", "SOIL04" },
                    { "PL016", "SOIL04" },
                    { "PL017", "SOIL01" },
                    { "PL018", "SOIL05" },
                    { "PL019", "SOIL01" },
                    { "PL020", "SOIL05" },
                    { "PL021", "SOIL01" },
                    { "PL022", "SOIL05" },
                    { "PL023", "SOIL01" },
                    { "PL024", "SOIL02" },
                    { "PL025", "SOIL03" },
                    { "PL026", "SOIL02" },
                    { "PL027", "SOIL01" },
                    { "PL028", "SOIL01" },
                    { "PL029", "SOIL03" },
                    { "PL030", "SOIL01" },
                    { "PL031", "SOIL01" },
                    { "PL032", "SOIL01" },
                    { "PL033", "SOIL01" },
                    { "PL034", "SOIL01" },
                    { "PL035", "SOIL01" },
                    { "PL036", "SOIL01" },
                    { "PL037", "SOIL01" },
                    { "PL038", "SOIL01" },
                    { "PL039", "SOIL01" },
                    { "PL040", "SOIL05" }
                });

            migrationBuilder.InsertData(
                table: "plant_usage",
                columns: new[] { "plant_id", "usage_id" },
                values: new object[,]
                {
                    { "PL001", "USE03" },
                    { "PL002", "USE03" },
                    { "PL003", "USE03" },
                    { "PL004", "USE03" },
                    { "PL005", "USE03" },
                    { "PL006", "USE01" },
                    { "PL007", "USE01" },
                    { "PL008", "USE01" },
                    { "PL009", "USE05" },
                    { "PL010", "USE05" },
                    { "PL011", "USE05" },
                    { "PL012", "USE02" },
                    { "PL013", "USE01" },
                    { "PL014", "USE01" },
                    { "PL015", "USE01" },
                    { "PL016", "USE01" },
                    { "PL017", "USE03" },
                    { "PL018", "USE03" },
                    { "PL019", "USE03" },
                    { "PL020", "USE03" },
                    { "PL021", "USE05" },
                    { "PL022", "USE03" },
                    { "PL022", "USE05" },
                    { "PL023", "USE01" },
                    { "PL024", "USE01" },
                    { "PL025", "USE01" },
                    { "PL026", "USE05" },
                    { "PL027", "USE01" },
                    { "PL027", "USE05" },
                    { "PL028", "USE01" },
                    { "PL029", "USE01" },
                    { "PL030", "USE03" },
                    { "PL031", "USE03" },
                    { "PL032", "USE03" },
                    { "PL033", "USE03" },
                    { "PL034", "USE01" },
                    { "PL035", "USE01" },
                    { "PL036", "USE01" },
                    { "PL037", "USE01" },
                    { "PL038", "USE01" },
                    { "PL039", "USE01" },
                    { "PL040", "USE01" },
                    { "PL040", "USE02" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_plant_climate_climate_id",
                table: "plant_climate",
                column: "climate_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_img_plant_id",
                table: "plant_img",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_info_plant_type_id",
                table: "plant_info",
                column: "plant_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_region_region_id",
                table: "plant_region",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_soil_soil_type_id",
                table: "plant_soil",
                column: "soil_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_usage_usage_id",
                table: "plant_usage",
                column: "usage_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plant_care");

            migrationBuilder.DropTable(
                name: "plant_climate");

            migrationBuilder.DropTable(
                name: "plant_img");

            migrationBuilder.DropTable(
                name: "plant_region");

            migrationBuilder.DropTable(
                name: "plant_soil");

            migrationBuilder.DropTable(
                name: "plant_usage");

            migrationBuilder.DropTable(
                name: "user_login_data");

            migrationBuilder.DropTable(
                name: "climate");

            migrationBuilder.DropTable(
                name: "region");

            migrationBuilder.DropTable(
                name: "soil_type");

            migrationBuilder.DropTable(
                name: "plant_info");

            migrationBuilder.DropTable(
                name: "usage");

            migrationBuilder.DropTable(
                name: "user_account");

            migrationBuilder.DropTable(
                name: "plant_type");
        }
    }
}
