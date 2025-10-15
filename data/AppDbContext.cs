using Microsoft.EntityFrameworkCore;
using Plantpedia.Enum;
using Plantpedia.Models;

namespace PLANTINFOWEB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSet cho từng bảng
        public DbSet<PlantInfo> PlantInfos { get; set; }
        public DbSet<Climate> Climates { get; set; }
        public DbSet<PlantRegion> PlantRegions { get; set; }
        public DbSet<PlantSoil> PlantSoils { get; set; }
        public DbSet<PlantClimate> PlantClimates { get; set; }
        public DbSet<PlantUsage> PlantUsages { get; set; }
        public DbSet<PlantType> PlantTypes { get; set; }
        public DbSet<SoilType> SoilTypes { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Usage> Usages { get; set; }
        public DbSet<PlantImg> PlantImgs { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserLoginData> UserLoginDatas { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; } = default!;
        public DbSet<PlantCare> PlantCares { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<PlantComment> PlantComments { get; set; }
        public DbSet<PlantCommentReaction> PlantCommentReactions { get; set; }
        public DbSet<PasswordReset> PasswordResets => Set<PasswordReset>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== COMPOSITE KEYS ====================
            // Composite keys cho bảng liên kết
            modelBuilder.Entity<PlantRegion>().HasKey(pr => new { pr.PlantId, pr.RegionId });
            modelBuilder.Entity<PlantSoil>().HasKey(ps => new { ps.PlantId, ps.SoilTypeId });
            modelBuilder.Entity<PlantClimate>().HasKey(pc => new { pc.PlantId, pc.ClimateId });
            modelBuilder.Entity<PlantUsage>().HasKey(pu => new { pu.PlantId, pu.UsageId });

            // ==================== PLANT INFO RELATIONSHIPS ====================
            // Quan hệ PlantInfo - PlantType (nhiều-1)
            modelBuilder
                .Entity<PlantInfo>()
                .HasOne(p => p.PlantType)
                .WithMany(t => t.Plants)
                .HasForeignKey(p => p.PlantTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PlantInfo - PlantCare (1-1)
            modelBuilder
                .Entity<PlantInfo>()
                .HasOne(p => p.CareInfo)
                .WithOne(c => c.PlantInfo)
                .HasForeignKey<PlantCare>(c => c.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PlantInfo - PlantImg (1-nhiều)
            modelBuilder
                .Entity<PlantImg>()
                .HasOne(pi => pi.Plant)
                .WithMany(p => p.PlantImages)
                .HasForeignKey(pi => pi.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== MANY-TO-MANY RELATIONSHIPS ====================
            // Quan hệ PlantInfo - Region (nhiều-nhiều qua PlantRegion)
            modelBuilder
                .Entity<PlantRegion>()
                .HasOne(pr => pr.Plant)
                .WithMany(p => p.PlantRegions)
                .HasForeignKey(pr => pr.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantRegion>()
                .HasOne(pr => pr.Region)
                .WithMany(r => r.PlantRegions)
                .HasForeignKey(pr => pr.RegionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PlantInfo - SoilType (nhiều-nhiều qua PlantSoil)
            modelBuilder
                .Entity<PlantSoil>()
                .HasOne(ps => ps.Plant)
                .WithMany(p => p.PlantSoils)
                .HasForeignKey(ps => ps.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantSoil>()
                .HasOne(ps => ps.SoilType)
                .WithMany(s => s.PlantSoils)
                .HasForeignKey(ps => ps.SoilTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PlantInfo - Climate (nhiều-nhiều qua PlantClimate)
            modelBuilder
                .Entity<PlantClimate>()
                .HasOne(pc => pc.Plant)
                .WithMany(p => p.PlantClimates)
                .HasForeignKey(pc => pc.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantClimate>()
                .HasOne(pc => pc.Climate)
                .WithMany(c => c.PlantClimates)
                .HasForeignKey(pc => pc.ClimateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PlantInfo - Usage (nhiều-nhiều qua PlantUsage)
            modelBuilder
                .Entity<PlantUsage>()
                .HasOne(pu => pu.Plant)
                .WithMany(p => p.PlantUsages)
                .HasForeignKey(pu => pu.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantUsage>()
                .HasOne(pu => pu.Usage)
                .WithMany(u => u.PlantUsages)
                .HasForeignKey(pu => pu.UsageId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== USER RELATIONSHIPS ====================
            // Quan hệ UserAccount - UserLoginData (1-1)
            modelBuilder
                .Entity<UserLoginData>()
                .HasOne(uld => uld.User)
                .WithOne(u => u.LoginData)
                .HasForeignKey<UserLoginData>(uld => uld.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordReset>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Email).HasMaxLength(256).IsRequired();
                e.Property(x => x.CodeHash).HasMaxLength(512).IsRequired();
                e.HasIndex(x => new { x.Email, x.Used });
                e.HasIndex(x => x.ExpiresAtUtc);
            });
            // ==================== USER ACTIVITY ====================
            modelBuilder.Entity<UserLoginData>().HasIndex(x => x.Username).IsUnique();
            modelBuilder.Entity<UserLoginData>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<UserAccount>().HasIndex(x => x.Status);
            modelBuilder
                .Entity<UserActivity>()
                .HasIndex(x => new
                {
                    x.UserId,
                    x.Type,
                    x.CreatedAt,
                });
            // ==================== USER FAVORITE ====================
            // Quan hệ UserAccount - UserFavorite (1-nhiều)
            modelBuilder
                .Entity<UserFavorite>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PlantInfo - UserFavorite (1-nhiều)
            modelBuilder
                .Entity<UserFavorite>()
                .HasOne(uf => uf.Plant)
                .WithMany(p => p.UserFavorites)
                .HasForeignKey(uf => uf.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: 1 user không thể favorite 1 cây 2 lần
            modelBuilder
                .Entity<UserFavorite>()
                .HasIndex(uf => new { uf.UserId, uf.PlantId })
                .IsUnique();
            // ==================== PLANT COMMENT ====================
            modelBuilder
                .Entity<PlantComment>()
                .HasOne(c => c.User)
                .WithMany(u => u.PlantComments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantComment>()
                .HasOne(c => c.Plant)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PlantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantComment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);

            // ==================== PLANT COMMENT REACTION ====================
            modelBuilder
                .Entity<PlantCommentReaction>()
                .HasOne(r => r.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<PlantCommentReaction>()
                .HasOne(r => r.User)
                .WithMany(u => u.PlantCommentReactions)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Đảm bảo 1 user chỉ có 1 reaction trên 1 comment
            modelBuilder
                .Entity<PlantCommentReaction>()
                .HasIndex(r => new { r.UserId, r.CommentId })
                .IsUnique();

            // Seed some initial data: not using for stability
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            seedPlantType(modelBuilder);
            seedClimate(modelBuilder);
            seedRegion(modelBuilder);
            seedUsage(modelBuilder);
            seedSoilType(modelBuilder);
            seedPlantClimate(modelBuilder);
            seedPlantSoid(modelBuilder);
            seedPlantUsage(modelBuilder);
            seedPlantRegion(modelBuilder);
            seedPlantCare(modelBuilder);
            seedPlantInfo(modelBuilder);
            seedPlantImg(modelBuilder);
            seedAccount(modelBuilder);
        }

        private static void seedAccount(ModelBuilder modelBuilder)
        {
            // Seed cho user_account
            modelBuilder
                .Entity<UserAccount>()
                .HasData(
                    new UserAccount
                    {
                        UserId = 1,
                        LastName = "Nguyen Minh A",
                        Gender = 'M',
                        DateOfBirth = new DateTime(2004, 6, 21, 0, 0, 0, DateTimeKind.Utc),
                        AvatarUrl =
                            "https://www.ibm.com/content/dam/adobe-cms/instana/media_logo/AWS-EC2.png/_jcr_content/renditions/cq5dam.web.1280.1280.png",
                        Status = UserStatus.Active,
                        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedAt = null,
                        DeletedAt = null,
                    },
                    new UserAccount
                    {
                        UserId = 2,
                        LastName = "Nguyen Minh B",
                        Gender = 'M',
                        DateOfBirth = new DateTime(2004, 6, 21, 0, 0, 0, DateTimeKind.Utc),
                        AvatarUrl =
                            "https://tse3.mm.bing.net/th/id/OIP.JMspq1z3Vm2m00ioNzUtEgHaHa?cb=12&rs=1&pid=ImgDetMain&o=7&rm=3",
                        Status = UserStatus.Active,
                        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedAt = null,
                        DeletedAt = null,
                    }
                );

            // Seed cho user_login_data
            modelBuilder
                .Entity<UserLoginData>()
                .HasData(
                    new UserLoginData
                    {
                        UserId = 1,
                        Username = "minha",
                        Role = Role.admin,
                        Email = "winhtuan.dev@gmail.com",
                        PasswordSalt = "5W8Ubef8XcxAeznr0uPnWA==",
                        PasswordHash = "dD8tZsGrCCpE6ZJgyiv7s85HAfs6MI0L8ccPVZ6gOXQ=",
                        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        LastLoginAt = null,
                    },
                    new UserLoginData
                    {
                        UserId = 2,
                        Username = "minhb",
                        Role = Role.user,
                        Email = "winhtuan@gmail.com",
                        PasswordSalt = "5W8Ubef8XcxAeznr0uPnWA==",
                        PasswordHash = "dD8tZsGrCCpE6ZJgyiv7s85HAfs6MI0L8ccPVZ6gOXQ=",
                        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        LastLoginAt = null,
                    }
                );
        }

        private static void seedPlantType(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantType>()
                .HasData(
                    new PlantType
                    {
                        PlantTypeId = "TYPE01",
                        TypeName = "Cây thân gỗ",
                        Description =
                            "Cây có thân cứng, sống lâu năm, thường dùng làm bóng mát hoặc lấy gỗ.",
                    },
                    new PlantType
                    {
                        PlantTypeId = "TYPE02",
                        TypeName = "Cây ăn quả",
                        Description =
                            "Cây cho quả dùng làm thực phẩm, phổ biến trong nông nghiệp Việt Nam.",
                    },
                    new PlantType
                    {
                        PlantTypeId = "TYPE03",
                        TypeName = "Cây công nghiệp",
                        Description = "Cây trồng để khai thác nguyên liệu như cao su, trà, cà phê.",
                    },
                    new PlantType
                    {
                        PlantTypeId = "TYPE04",
                        TypeName = "Cây lương thực",
                        Description = "Cây cung cấp thực phẩm chính như lúa, ngô, khoai.",
                    },
                    new PlantType
                    {
                        PlantTypeId = "TYPE05",
                        TypeName = "Cây cảnh",
                        Description =
                            "Cây trồng để trang trí, làm đẹp không gian sống và công cộng.",
                    },
                    new PlantType
                    {
                        PlantTypeId = "TYPE06",
                        TypeName = "Cây rau màu & gia vị",
                        Description =
                            "Bao gồm các loại cây trồng ngắn ngày để thu hoạch lá, củ, quả dùng làm thực phẩm hoặc gia vị.",
                    }
                );
        }

        private static void seedClimate(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Climate>()
                .HasData(
                    new Climate
                    {
                        ClimateId = "CLM01",
                        Name = "Nhiệt đới gió mùa",
                        TemperatureRange = "20°C - 35°C",
                        RainfallRange = "1500mm - 2500mm/năm",
                        HumidityRange = "70% - 90%",
                    },
                    new Climate
                    {
                        ClimateId = "CLM02",
                        Name = "Ôn đới núi cao",
                        TemperatureRange = "5°C - 20°C",
                        RainfallRange = "1000mm - 2000mm/năm",
                        HumidityRange = "60% - 80%",
                    },
                    new Climate
                    {
                        ClimateId = "CLM03",
                        Name = "Khí hậu ven biển",
                        TemperatureRange = "22°C - 32°C",
                        RainfallRange = "1200mm - 2200mm/năm",
                        HumidityRange = "75% - 95%",
                    },
                    new Climate
                    {
                        ClimateId = "CLM04",
                        Name = "Khí hậu khô hạn",
                        TemperatureRange = "25°C - 38°C",
                        RainfallRange = "500mm - 1000mm/năm",
                        HumidityRange = "40% - 60%",
                    },
                    new Climate
                    {
                        ClimateId = "CLM05",
                        Name = "Khí hậu mát mẻ",
                        TemperatureRange = "15°C - 25°C",
                        RainfallRange = "1000mm - 1800mm/năm",
                        HumidityRange = "65% - 85%",
                    }
                );
        }

        private static void seedRegion(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Region>()
                .HasData(
                    new Region
                    {
                        RegionId = "REG01",
                        Name = "Đồng bằng sông Cửu Long",
                        Note =
                            "Vùng đất thấp, nhiều kênh rạch, khí hậu nhiệt đới gió mùa, thích hợp trồng lúa và cây ăn quả.",
                    },
                    new Region
                    {
                        RegionId = "REG02",
                        Name = "Đồng bằng sông Hồng",
                        Note =
                            "Vùng nông nghiệp trọng điểm phía Bắc, đất phù sa màu mỡ, khí hậu cận nhiệt đới ẩm.",
                    },
                    new Region
                    {
                        RegionId = "REG03",
                        Name = "Tây Nguyên",
                        Note =
                            "Vùng cao nguyên đất đỏ bazan, khí hậu ôn đới núi cao, thích hợp cây công nghiệp như cà phê, cao su.",
                    },
                    new Region
                    {
                        RegionId = "REG04",
                        Name = "Duyên hải Nam Trung Bộ",
                        Note =
                            "Vùng ven biển khô hạn, đất cát pha, phù hợp cây chịu hạn và cây cảnh.",
                    },
                    new Region
                    {
                        RegionId = "REG05",
                        Name = "Trung du và miền núi Bắc Bộ",
                        Note =
                            "Vùng đồi núi, khí hậu mát mẻ, đa dạng sinh học, thích hợp cây thuốc và cây rừng.",
                    }
                );
        }

        private static void seedUsage(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Usage>()
                .HasData(
                    new Usage
                    {
                        UsageId = "USE01",
                        Name = "Thực phẩm",
                        Note = "Cây cung cấp quả, lá, củ hoặc hạt để ăn.",
                    },
                    new Usage
                    {
                        UsageId = "USE02",
                        Name = "Dược liệu",
                        Note = "Cây có giá trị chữa bệnh trong y học cổ truyền.",
                    },
                    new Usage
                    {
                        UsageId = "USE03",
                        Name = "Cảnh quan",
                        Note = "Cây dùng để trang trí, tạo không gian xanh.",
                    },
                    new Usage
                    {
                        UsageId = "USE04",
                        Name = "Gỗ",
                        Note = "Cây thân gỗ dùng trong xây dựng, nội thất.",
                    },
                    new Usage
                    {
                        UsageId = "USE05",
                        Name = "Nguyên liệu công nghiệp",
                        Note = "Cây dùng để sản xuất cao su, trà, cà phê, v.v.",
                    }
                );
        }

        private static void seedSoilType(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<SoilType>()
                .HasData(
                    new SoilType
                    {
                        SoilTypeId = "SOIL01",
                        Name = "Đất phù sa",
                        Note = "Giàu dinh dưỡng, thích hợp trồng lúa và cây ăn quả.",
                    },
                    new SoilType
                    {
                        SoilTypeId = "SOIL02",
                        Name = "Đất đỏ bazan",
                        Note = "Thường thấy ở Tây Nguyên, tốt cho cây công nghiệp.",
                    },
                    new SoilType
                    {
                        SoilTypeId = "SOIL03",
                        Name = "Đất cát ven biển",
                        Note = "Thích hợp trồng rau màu, cây chịu hạn.",
                    },
                    new SoilType
                    {
                        SoilTypeId = "SOIL04",
                        Name = "Đất mặn",
                        Note = "Cần cải tạo, có thể trồng cây chịu mặn như đước.",
                    },
                    new SoilType
                    {
                        SoilTypeId = "SOIL05",
                        Name = "Đất đồi núi",
                        Note = "Thường dùng trồng cây lâu năm, cây rừng.",
                    }
                );
        }

        #region N-N
        private static void seedPlantClimate(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantClimate>()
                .HasData(
                    new PlantClimate { PlantId = "PL001", ClimateId = "CLM01" }, // Bồ Đề – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL002", ClimateId = "CLM01" }, // Bằng Lăng – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL003", ClimateId = "CLM03" }, // Ngọc Lan – Khí hậu ven biển
                    new PlantClimate { PlantId = "PL004", ClimateId = "CLM01" }, // Phượng Vĩ – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL005", ClimateId = "CLM01" }, // Xoài – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL006", ClimateId = "CLM01" }, // Ổi – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL007", ClimateId = "CLM03" }, // Dừa – Khí hậu ven biển
                    new PlantClimate { PlantId = "PL008", ClimateId = "CLM01" }, // Mít – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL009", ClimateId = "CLM02" }, // Cà phê – Ôn đới núi cao
                    new PlantClimate { PlantId = "PL010", ClimateId = "CLM02" }, // Cao su – Ôn đới núi cao
                    new PlantClimate { PlantId = "PL011", ClimateId = "CLM02" }, // Trà – Ôn đới núi cao
                    new PlantClimate { PlantId = "PL012", ClimateId = "CLM01" }, // Hồ tiêu – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL013", ClimateId = "CLM01" }, // Lúa – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL014", ClimateId = "CLM04" }, // Ngô – Khí hậu khô hạn
                    new PlantClimate { PlantId = "PL015", ClimateId = "CLM04" }, // Khoai lang – Khí hậu khô hạn
                    new PlantClimate { PlantId = "PL016", ClimateId = "CLM04" }, // Sắn – Khí hậu khô hạn
                    new PlantClimate { PlantId = "PL017", ClimateId = "CLM01" }, // Mai vàng – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL018", ClimateId = "CLM05" }, // Đào – Khí hậu mát mẻ
                    new PlantClimate { PlantId = "PL019", ClimateId = "CLM01" }, // Cúc vạn thọ – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL020", ClimateId = "CLM05" }, // Lan Hồ Điệp – Khí hậu mát mẻ
                    new PlantClimate { PlantId = "PL021", ClimateId = "CLM01" }, // Sưa đỏ – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL022", ClimateId = "CLM01" }, // Xà cừ – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL023", ClimateId = "CLM01" }, // Vải – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL024", ClimateId = "CLM01" }, // Sầu riêng – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL025", ClimateId = "CLM04" }, // Thanh long – Khí hậu khô hạn
                    new PlantClimate { PlantId = "PL026", ClimateId = "CLM04" }, // Điều – Khí hậu khô hạn
                    new PlantClimate { PlantId = "PL027", ClimateId = "CLM01" }, // Mía – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL028", ClimateId = "CLM01" }, // Đậu tương – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL029", ClimateId = "CLM04" }, // Lạc – Khí hậu khô hạn
                    new PlantClimate { PlantId = "PL030", ClimateId = "CLM01" }, // Lưỡi hổ – Nhiệt đới gió mùa (trong nhà)
                    new PlantClimate { PlantId = "PL031", ClimateId = "CLM01" }, // Trầu bà – Nhiệt đới gió mùa (trong nhà)
                    new PlantClimate { PlantId = "PL032", ClimateId = "CLM05" }, // Hoa hồng – Khí hậu mát mẻ
                    new PlantClimate { PlantId = "PL033", ClimateId = "CLM01" }, // Hoa súng – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL034", ClimateId = "CLM01" }, // Rau muống – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL035", ClimateId = "CLM01" }, // Cà chua – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL036", ClimateId = "CLM01" }, // Dưa chuột – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL037", ClimateId = "CLM01" }, // Hành lá – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL038", ClimateId = "CLM01" }, // Ớt – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL039", ClimateId = "CLM05" }, // Xà lách – Khí hậu mát mẻ
                    new PlantClimate { PlantId = "PL040", ClimateId = "CLM01" }, // Sả – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL041", ClimateId = "CLM01" }, // Sầu riêng R6 – Nhiệt đới gió mùa
                    new PlantClimate { PlantId = "PL042", ClimateId = "CLM01" } // Sầu riêng Dona – Nhiệt đới gió mùa
                );
        }

        private static void seedPlantSoid(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantSoil>()
                .HasData(
                    new PlantSoil { PlantId = "PL001", SoilTypeId = "SOIL01" }, // Bồ Đề - đất phù sa
                    new PlantSoil { PlantId = "PL002", SoilTypeId = "SOIL05" }, // Bằng Lăng - đất đồi núi
                    new PlantSoil { PlantId = "PL003", SoilTypeId = "SOIL01" }, // Ngọc Lan - đất phù sa
                    new PlantSoil { PlantId = "PL004", SoilTypeId = "SOIL03" }, // Phượng Vĩ - đất cát ven biển
                    new PlantSoil { PlantId = "PL005", SoilTypeId = "SOIL01" }, // Muồng Hoàng Yến - đất phù sa
                    new PlantSoil { PlantId = "PL006", SoilTypeId = "SOIL01" }, // Ổi – đất phù sa
                    new PlantSoil { PlantId = "PL007", SoilTypeId = "SOIL03" }, // Dừa – đất cát ven biển
                    new PlantSoil { PlantId = "PL008", SoilTypeId = "SOIL01" }, // Mít – đất phù sa
                    new PlantSoil { PlantId = "PL009", SoilTypeId = "SOIL02" }, // Cà phê – đất đỏ bazan
                    new PlantSoil { PlantId = "PL010", SoilTypeId = "SOIL02" }, // Cao su – đất đỏ bazan
                    new PlantSoil { PlantId = "PL011", SoilTypeId = "SOIL02" }, // Trà – đất đỏ bazan
                    new PlantSoil { PlantId = "PL012", SoilTypeId = "SOIL01" }, // Hồ tiêu – đất phù sa
                    new PlantSoil { PlantId = "PL013", SoilTypeId = "SOIL01" }, // Lúa – đất phù sa
                    new PlantSoil { PlantId = "PL014", SoilTypeId = "SOIL04" }, // Ngô – đất mặn
                    new PlantSoil { PlantId = "PL015", SoilTypeId = "SOIL04" }, // Khoai lang – đất mặn
                    new PlantSoil { PlantId = "PL016", SoilTypeId = "SOIL04" }, // Sắn – đất mặn
                    new PlantSoil { PlantId = "PL017", SoilTypeId = "SOIL01" }, // Mai vàng – đất phù sa
                    new PlantSoil { PlantId = "PL018", SoilTypeId = "SOIL05" }, // Đào – đất đồi núi
                    new PlantSoil { PlantId = "PL019", SoilTypeId = "SOIL01" }, // Cúc vạn thọ – đất phù sa
                    new PlantSoil { PlantId = "PL020", SoilTypeId = "SOIL05" }, // Lan Hồ Điệp – đất đồi núi
                    new PlantSoil { PlantId = "PL021", SoilTypeId = "SOIL01" }, // Sưa đỏ - đất phù sa
                    new PlantSoil { PlantId = "PL022", SoilTypeId = "SOIL05" }, // Xà cừ - đất đồi núi (chịu đất xấu)
                    new PlantSoil { PlantId = "PL023", SoilTypeId = "SOIL01" }, // Vải - đất phù sa
                    new PlantSoil { PlantId = "PL024", SoilTypeId = "SOIL02" }, // Sầu riêng – đất đỏ bazan
                    new PlantSoil { PlantId = "PL025", SoilTypeId = "SOIL03" }, // Thanh long - đất cát ven biển
                    new PlantSoil { PlantId = "PL026", SoilTypeId = "SOIL02" }, // Điều – đất đỏ bazan
                    new PlantSoil { PlantId = "PL027", SoilTypeId = "SOIL01" }, // Mía - đất phù sa
                    new PlantSoil { PlantId = "PL028", SoilTypeId = "SOIL01" }, // Đậu tương - đất phù sa
                    new PlantSoil { PlantId = "PL029", SoilTypeId = "SOIL03" }, // Lạc - đất cát ven biển
                    new PlantSoil { PlantId = "PL030", SoilTypeId = "SOIL01" }, // Lưỡi hổ - đất phù sa (trong chậu)
                    new PlantSoil { PlantId = "PL031", SoilTypeId = "SOIL01" }, // Trầu bà - đất phù sa (trong chậu)
                    new PlantSoil { PlantId = "PL032", SoilTypeId = "SOIL01" }, // Hoa hồng - đất phù sa
                    new PlantSoil { PlantId = "PL033", SoilTypeId = "SOIL01" }, // Hoa súng - đất phù sa (bùn ao)
                    new PlantSoil { PlantId = "PL034", SoilTypeId = "SOIL01" }, // Rau muống - đất phù sa
                    new PlantSoil { PlantId = "PL035", SoilTypeId = "SOIL01" }, // Cà chua - đất phù sa
                    new PlantSoil { PlantId = "PL036", SoilTypeId = "SOIL01" }, // Dưa chuột - đất phù sa
                    new PlantSoil { PlantId = "PL037", SoilTypeId = "SOIL01" }, // Hành lá - đất phù sa
                    new PlantSoil { PlantId = "PL038", SoilTypeId = "SOIL01" }, // Ớt - đất phù sa
                    new PlantSoil { PlantId = "PL039", SoilTypeId = "SOIL01" }, // Xà lách - đất phù sa
                    new PlantSoil { PlantId = "PL040", SoilTypeId = "SOIL05" }, // Sả - đất đồi núi
                    new PlantSoil { PlantId = "PL041", SoilTypeId = "SOIL02" }, // Sầu riêng R6 – đất đỏ bazan
                    new PlantSoil { PlantId = "PL042", SoilTypeId = "SOIL02" } // Sầu riêng Dona – đất đỏ bazan
                );
        }

        private static void seedPlantUsage(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantUsage>()
                .HasData(
                    new PlantUsage { PlantId = "PL001", UsageId = "USE03" }, // Bồ Đề - làm cảnh
                    new PlantUsage { PlantId = "PL002", UsageId = "USE03" }, // Bằng Lăng - làm cảnh
                    new PlantUsage { PlantId = "PL003", UsageId = "USE03" }, // Ngọc Lan - làm cảnh
                    new PlantUsage { PlantId = "PL004", UsageId = "USE03" }, // Phượng Vĩ - làm cảnh
                    new PlantUsage { PlantId = "PL005", UsageId = "USE03" }, // Muồng Hoàng Yến - làm cảnh
                    // PL006–PL008: cây ăn quả
                    new PlantUsage { PlantId = "PL006", UsageId = "USE01" }, // Ổi – thực phẩm
                    new PlantUsage { PlantId = "PL007", UsageId = "USE01" }, // Dừa – thực phẩm
                    new PlantUsage { PlantId = "PL008", UsageId = "USE01" }, // Mít – thực phẩm
                    // PL009–PL012: cây công nghiệp
                    new PlantUsage { PlantId = "PL009", UsageId = "USE05" }, // Cà phê – nguyên liệu công nghiệp
                    new PlantUsage { PlantId = "PL010", UsageId = "USE05" }, // Cao su – nguyên liệu công nghiệp
                    new PlantUsage { PlantId = "PL011", UsageId = "USE05" }, // Trà – nguyên liệu công nghiệp
                    new PlantUsage { PlantId = "PL012", UsageId = "USE02" }, // Hồ tiêu – dược liệu
                    // PL013–PL016: cây lương thực
                    new PlantUsage { PlantId = "PL013", UsageId = "USE01" }, // Lúa – thực phẩm
                    new PlantUsage { PlantId = "PL014", UsageId = "USE01" }, // Ngô – thực phẩm
                    new PlantUsage { PlantId = "PL015", UsageId = "USE01" }, // Khoai lang – thực phẩm
                    new PlantUsage { PlantId = "PL016", UsageId = "USE01" }, // Sắn – thực phẩm
                    // PL017–PL020: cây cảnh
                    new PlantUsage { PlantId = "PL017", UsageId = "USE03" }, // Mai vàng – làm cảnh
                    new PlantUsage { PlantId = "PL018", UsageId = "USE03" }, // Đào – làm cảnh
                    new PlantUsage { PlantId = "PL019", UsageId = "USE03" }, // Cúc vạn thọ – làm cảnh
                    new PlantUsage { PlantId = "PL020", UsageId = "USE03" }, // Lan Hồ Điệp – làm cảnh
                    new PlantUsage { PlantId = "PL021", UsageId = "USE05" }, // Sưa đỏ – nguyên liệu công nghiệp (gỗ)
                    new PlantUsage { PlantId = "PL022", UsageId = "USE03" }, // Xà cừ - làm cảnh (bóng mát)
                    new PlantUsage { PlantId = "PL022", UsageId = "USE05" }, // Xà cừ - nguyên liệu công nghiệp (gỗ)
                    new PlantUsage { PlantId = "PL023", UsageId = "USE01" }, // Vải – thực phẩm
                    new PlantUsage { PlantId = "PL024", UsageId = "USE01" }, // Sầu riêng – thực phẩm
                    new PlantUsage { PlantId = "PL025", UsageId = "USE01" }, // Thanh long – thực phẩm
                    new PlantUsage { PlantId = "PL026", UsageId = "USE05" }, // Điều – nguyên liệu công nghiệp
                    new PlantUsage { PlantId = "PL027", UsageId = "USE01" }, // Mía – thực phẩm
                    new PlantUsage { PlantId = "PL027", UsageId = "USE05" }, // Mía – nguyên liệu công nghiệp
                    new PlantUsage { PlantId = "PL028", UsageId = "USE01" }, // Đậu tương – thực phẩm
                    new PlantUsage { PlantId = "PL029", UsageId = "USE01" }, // Lạc – thực phẩm
                    new PlantUsage { PlantId = "PL030", UsageId = "USE03" }, // Lưỡi hổ - làm cảnh
                    new PlantUsage { PlantId = "PL031", UsageId = "USE03" }, // Trầu bà - làm cảnh
                    new PlantUsage { PlantId = "PL032", UsageId = "USE03" }, // Hoa hồng - làm cảnh
                    new PlantUsage { PlantId = "PL033", UsageId = "USE03" }, // Hoa súng - làm cảnh
                    new PlantUsage { PlantId = "PL034", UsageId = "USE01" }, // Rau muống – thực phẩm
                    new PlantUsage { PlantId = "PL035", UsageId = "USE01" }, // Cà chua – thực phẩm
                    new PlantUsage { PlantId = "PL036", UsageId = "USE01" }, // Dưa chuột – thực phẩm
                    new PlantUsage { PlantId = "PL037", UsageId = "USE01" }, // Hành lá – thực phẩm (gia vị)
                    new PlantUsage { PlantId = "PL038", UsageId = "USE01" }, // Ớt – thực phẩm (gia vị)
                    new PlantUsage { PlantId = "PL039", UsageId = "USE01" }, // Xà lách – thực phẩm
                    new PlantUsage { PlantId = "PL040", UsageId = "USE01" }, // Sả – thực phẩm (gia vị)
                    new PlantUsage { PlantId = "PL040", UsageId = "USE02" }, // Sả – dược liệu
                    new PlantUsage { PlantId = "PL041", UsageId = "USE01" }, // Sầu riêng R6 – thực phẩm
                    new PlantUsage { PlantId = "PL042", UsageId = "USE01" } // Sầu riêng Dona – thực phẩm
                );
        }

        private static void seedPlantRegion(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantRegion>()
                .HasData(
                    new PlantRegion { PlantId = "PL001", RegionId = "REG02" }, // Bồ Đề - Đồng bằng sông Hồng
                    new PlantRegion { PlantId = "PL002", RegionId = "REG01" }, // Bằng Lăng - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL003", RegionId = "REG04" }, // Ngọc Lan - Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL004", RegionId = "REG01" }, // Phượng Vĩ - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL005", RegionId = "REG03" }, // Muồng Hoàng Yến - Tây Nguyên
                    new PlantRegion { PlantId = "PL006", RegionId = "REG01" }, // Ổi – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL007", RegionId = "REG04" }, // Dừa – Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL008", RegionId = "REG01" }, // Mít – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL009", RegionId = "REG03" }, // Cà phê – Tây Nguyên
                    new PlantRegion { PlantId = "PL010", RegionId = "REG03" }, // Cao su – Tây Nguyên
                    new PlantRegion { PlantId = "PL011", RegionId = "REG05" }, // Trà – Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL012", RegionId = "REG01" }, // Hồ tiêu – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL013", RegionId = "REG01" }, // Lúa – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL014", RegionId = "REG04" }, // Ngô – Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL015", RegionId = "REG04" }, // Khoai lang – Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL016", RegionId = "REG04" }, // Sắn – Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL017", RegionId = "REG01" }, // Mai vàng – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL018", RegionId = "REG05" }, // Đào – Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL019", RegionId = "REG02" }, // Cúc vạn thọ – Đồng bằng sông Hồng
                    new PlantRegion { PlantId = "PL020", RegionId = "REG05" }, // Lan Hồ Điệp – Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL021", RegionId = "REG05" }, // Sưa đỏ - Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL022", RegionId = "REG02" }, // Xà cừ - Đồng bằng sông Hồng
                    new PlantRegion { PlantId = "PL023", RegionId = "REG05" }, // Vải - Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL024", RegionId = "REG01" }, // Sầu riêng - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL025", RegionId = "REG04" }, // Thanh long - Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL026", RegionId = "REG03" }, // Điều – Tây Nguyên
                    new PlantRegion { PlantId = "PL027", RegionId = "REG01" }, // Mía - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL028", RegionId = "REG01" }, // Đậu tương - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL029", RegionId = "REG02" }, // Lạc - Đồng bằng sông Hồng
                    new PlantRegion { PlantId = "PL030", RegionId = "REG01" }, // Lưỡi hổ - Phổ biến cả nước
                    new PlantRegion { PlantId = "PL031", RegionId = "REG02" }, // Trầu bà - Phổ biến cả nước
                    new PlantRegion { PlantId = "PL032", RegionId = "REG05" }, // Hoa hồng - Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL033", RegionId = "REG01" }, // Hoa súng - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL034", RegionId = "REG02" }, // Rau muống - Đồng bằng sông Hồng
                    new PlantRegion { PlantId = "PL035", RegionId = "REG05" }, // Cà chua - Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL036", RegionId = "REG02" }, // Dưa chuột - Đồng bằng sông Hồng
                    new PlantRegion { PlantId = "PL037", RegionId = "REG01" }, // Hành lá - Phổ biến cả nước
                    new PlantRegion { PlantId = "PL038", RegionId = "REG04" }, // Ớt - Duyên hải Nam Trung Bộ
                    new PlantRegion { PlantId = "PL039", RegionId = "REG05" }, // Xà lách - Trung du miền núi Bắc Bộ
                    new PlantRegion { PlantId = "PL040", RegionId = "REG01" }, // Sả - Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL041", RegionId = "REG01" }, // Sầu riêng R6 – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL041", RegionId = "REG03" }, // Sầu riêng R6 – Tây Nguyên
                    new PlantRegion { PlantId = "PL042", RegionId = "REG01" }, // Sầu riêng Dona – Đồng bằng sông Cửu Long
                    new PlantRegion { PlantId = "PL042", RegionId = "REG03" } // Sầu riêng Dona – Tây Nguyên
                );
        }
        #endregion


        #region Plant Care
        private static void seedPlantCare(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantCare>()
                .HasData(
                    // TYPE01 – Cây thân gỗ
                    new PlantCare
                    {
                        PlantId = "PL001", // Bồ Đề
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất thoát nước tốt, giàu mùn.",
                        FertilizerInfo =
                            "Bón phân hữu cơ định kỳ hàng năm khi cây còn non. Cây trưởng thành không yêu cầu bón phân thường xuyên.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL002", // Bằng Lăng
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Ưa đất tơi xốp, thoát nước tốt và giàu dinh dưỡng.",
                        FertilizerInfo =
                            "Bón phân NPK có hàm lượng Kali cao trước mùa ra hoa để kích thích hoa nở rộ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL003", // Ngọc Lan
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất chua nhẹ, giàu mùn và thoát nước tốt.",
                        FertilizerInfo =
                            "Bón phân hữu cơ hoai mục vào đầu mùa mưa. Bổ sung NPK định kỳ 2-3 tháng/lần.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL004", // Phượng Vĩ
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Thap,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Chịu được nhiều loại đất, kể cả đất nghèo dinh dưỡng, nhưng cần thoát nước tốt.",
                        FertilizerInfo =
                            "Không yêu cầu bón phân thường xuyên. Có thể bón bổ sung phân hữu cơ hàng năm để cây phát triển tốt hơn.",
                    },
                    // TYPE02 – Cây ăn quả
                    new PlantCare
                    {
                        PlantId = "PL005", // Xoài
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất thịt nhẹ, phù sa, thoát nước tốt, pH từ 5.5-7.0.",
                        FertilizerInfo =
                            "Bón phân theo giai đoạn: kiến thiết cơ bản, ra hoa, nuôi quả. Tăng cường Kali trước khi thu hoạch để quả ngọt hơn.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL006", // Ổi
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Thích nghi rộng, nhưng tốt nhất là đất tơi xốp, giàu hữu cơ.",
                        FertilizerInfo =
                            "Bón phân NPK và phân hữu cơ định kỳ sau mỗi vụ thu hoạch để cây phục hồi và cho quả vụ tiếp theo.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL007", // Dừa
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Ưa đất cát pha, đất phù sa ven biển có độ mặn nhẹ.",
                        FertilizerInfo =
                            "Cần nhiều Kali và Clo. Bón phân NPK có tỷ lệ Kali cao và bổ sung muối (NaCl) định kỳ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL008", // Mít
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất thịt pha sét, thoát nước tốt, tầng canh tác dày.",
                        FertilizerInfo =
                            "Bón nhiều phân hữu cơ và NPK. Tăng cường Kali khi quả bắt đầu lớn để tăng độ ngọt và phẩm chất.",
                    },
                    // TYPE03 – Cây công nghiệp
                    new PlantCare
                    {
                        PlantId = "PL009", // Cà phê
                        LightRequirement = LightRequirement.BanRam,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất bazan tơi xốp, giàu hữu cơ, thoát nước tốt.",
                        FertilizerInfo =
                            "Yêu cầu dinh dưỡng cao, bón phân cân đối NPK theo chu kỳ sinh trưởng, đặc biệt là sau thu hoạch và trước khi ra hoa.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL010", // Cao su
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất xám, đất đỏ bazan, pH từ 4.5-6.5.",
                        FertilizerInfo =
                            "Bón phân tập trung vào giai đoạn kiến thiết cơ bản (5-6 năm đầu) để cây nhanh đạt đường kính đủ tiêu chuẩn khai thác mủ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL011", // Trà
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất chua (pH 4.5-5.5), tơi xốp, giàu hữu cơ.",
                        FertilizerInfo =
                            "Cần nhiều Đạm (N) để phát triển búp non. Bón phân NPK giàu đạm sau mỗi lứa hái.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL012", // Hồ tiêu
                        LightRequirement = LightRequirement.BanRam,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation =
                            "Đất đỏ bazan hoặc đất xám, tơi xốp, thoát nước cực tốt để tránh bệnh thối rễ.",
                        FertilizerInfo =
                            "Kết hợp phân hữu cơ và phân vô cơ. Bón định kỳ, tăng cường Kali giai đoạn nuôi hạt để hạt chắc.",
                    },
                    // TYPE04 – Cây lương thực
                    new PlantCare
                    {
                        PlantId = "PL013", // Lúa
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Đất phù sa, đất sét hoặc đất thịt có khả năng giữ nước tốt.",
                        FertilizerInfo =
                            "Yêu cầu bón phân theo từng giai đoạn: bón lót, bón thúc đẻ nhánh và bón đón đòng.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL014", // Ngô
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất tơi xốp, giàu dinh dưỡng, thoát nước tốt.",
                        FertilizerInfo =
                            "Cần nhiều dinh dưỡng, đặc biệt là Đạm. Bón lót và bón thúc ở các giai đoạn cây 3-5 lá, xoáy nõn, trỗ cờ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL015", // Khoai lang
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Đất cát pha, đất thịt nhẹ, tơi xốp để củ dễ phát triển.",
                        FertilizerInfo =
                            "Cần nhiều Kali để củ to và ngọt. Bón lót và bón thúc tập trung vào giai đoạn dây bắt đầu phát triển và hình thành củ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL016", // Sắn
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Thap,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Chịu được đất nghèo dinh dưỡng, đất dốc, nhưng cần thoát nước.",
                        FertilizerInfo =
                            "Phản ứng tốt với phân bón, đặc biệt là Kali. Bón lót và bón thúc 1-2 lần trong 3 tháng đầu.",
                    },
                    // TYPE05 – Cây cảnh
                    new PlantCare
                    {
                        PlantId = "PL017", // Mai vàng
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất thịt pha, tơi xốp, thoát nước tốt, giàu hữu cơ.",
                        FertilizerInfo =
                            "Bón phân định kỳ. Cuối năm cần tuốt lá và bón phân có hàm lượng Lân và Kali cao để kích thích ra hoa đúng dịp Tết.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL018", // Đào
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất thịt nhẹ, đất phù sa, thoát nước tốt.",
                        FertilizerInfo =
                            "Tương tự như mai, cần tuốt lá và xử lý phân bón, nước để cây ra hoa đúng dịp Tết Nguyên Đán.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL019", // Cúc vạn thọ
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất tơi xốp, giàu dinh dưỡng, thoát nước tốt.",
                        FertilizerInfo =
                            "Bón phân NPK định kỳ 10-15 ngày/lần. Bấm ngọn để cây ra nhiều nhánh và cho nhiều hoa hơn.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL020", // Lan Hồ Điệp
                        LightRequirement = LightRequirement.BongRam,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        HumidityPreference = HumidityPreference.Cao,
                        GrowthRate = GrowthRate.Cham,
                        SoilRecommendation =
                            "Không trồng trong đất. Trồng trong giá thể chuyên dụng cho lan như vỏ thông, than củi, dớn.",
                        FertilizerInfo =
                            "Sử dụng phân bón chuyên dụng cho lan, pha loãng. Bón định kỳ 1-2 tuần/lần, ngưng bón khi cây đang ra hoa.",
                    },
                    // TYPE01 – Cây thân gỗ (Thêm)
                    new PlantCare
                    {
                        PlantId = "PL021", // Sưa đỏ
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Cham,
                        SoilRecommendation = "Đất tơi xốp, giàu dinh dưỡng và thoát nước tốt.",
                        FertilizerInfo =
                            "Bón lót phân hữu cơ khi trồng. Bón thúc NPK định kỳ trong vài năm đầu để cây phát triển nhanh.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL022", // Xà cừ
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Thap,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Thích nghi tốt với nhiều loại đất, kể cả đất khô cằn.",
                        FertilizerInfo =
                            "Không yêu cầu bón phân thường xuyên, sức sống rất mạnh mẽ.",
                    },
                    // TYPE02 – Cây ăn quả (Thêm)
                    new PlantCare
                    {
                        PlantId = "PL023", // Vải
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất phù sa, giàu hữu cơ, thoát nước tốt.",
                        FertilizerInfo =
                            "Bón phân theo giai đoạn, tăng cường Lân và Kali để thúc đẩy ra hoa và đậu quả.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL024", // Sầu riêng
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        HumidityPreference = HumidityPreference.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation =
                            "Đất thịt giàu hữu cơ, thoát nước cực tốt để tránh bệnh thối rễ.",
                        FertilizerInfo =
                            "Yêu cầu dinh dưỡng rất cao, cần bón phân hữu cơ và NPK thường xuyên, đặc biệt là Kali trong giai đoạn nuôi trái.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL025", // Thanh long
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Thap,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation =
                            "Đất cát pha, tơi xốp, thoát nước tốt. Cần có trụ để leo.",
                        FertilizerInfo =
                            "Bón phân hữu cơ và NPK định kỳ, đặc biệt khi cây đang ra hoa và nuôi quả.",
                    },
                    // TYPE03 – Cây công nghiệp (Thêm)
                    new PlantCare
                    {
                        PlantId = "PL026", // Điều
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Thap,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất xám bạc màu, đất cát, thoát nước tốt.",
                        FertilizerInfo =
                            "Chịu được đất nghèo dinh dưỡng. Bón phân NPK vào đầu và cuối mùa mưa để tăng năng suất.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL027", // Mía
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất phù sa, đất sét pha, giữ ẩm tốt.",
                        FertilizerInfo =
                            "Cần nhiều Đạm và Kali. Bón thúc nhiều lần trong giai đoạn vươn lóng.",
                    },
                    // TYPE04 – Cây lương thực (Thêm)
                    new PlantCare
                    {
                        PlantId = "PL028", // Đậu tương (Đậu nành)
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất thịt nhẹ, tơi xốp, thoát nước tốt.",
                        FertilizerInfo =
                            "Cây họ đậu có khả năng tự cố định đạm. Cần bón lót Lân và Kali, bón ít đạm.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL029", // Lạc (Đậu phộng)
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất cát pha tơi xốp để củ dễ phát triển.",
                        FertilizerInfo =
                            "Cần nhiều Lân, Kali và Vôi (Canxi) để củ chắc. Bón lót và bón thúc khi cây ra hoa rộ.",
                    },
                    // TYPE05 – Cây cảnh (Thêm)
                    new PlantCare
                    {
                        PlantId = "PL030", // Lưỡi hổ
                        LightRequirement = LightRequirement.BanRam,
                        WateringNeeds = WateringNeeds.Thap,
                        GrowthRate = GrowthRate.Cham,
                        SoilRecommendation = "Hỗn hợp đất thoát nước tốt như đất cho xương rồng.",
                        FertilizerInfo =
                            "Hầu như không cần bón phân. Có thể bón phân loãng 1-2 lần vào mùa xuân.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL031", // Trầu bà
                        LightRequirement = LightRequirement.BanRam,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất trồng trong nhà thông thường, thoát nước tốt.",
                        FertilizerInfo =
                            "Bón phân cân bằng dạng lỏng mỗi tháng một lần vào mùa sinh trưởng.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL032", // Hoa hồng
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất giàu hữu cơ, tơi xốp và thoát nước tốt.",
                        FertilizerInfo =
                            "Là cây ưa phân. Bón phân hữu cơ và NPK định kỳ 2-3 tuần/lần để cây ra hoa liên tục.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL033", // Hoa súng
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation =
                            "Đất bùn, đất sét nặng trong ao hồ hoặc chậu không có lỗ thoát nước.",
                        FertilizerInfo =
                            "Sử dụng phân bón viên nén chuyên dụng cho cây thủy sinh, nhét vào gốc 1-2 tháng/lần.",
                    },
                    // TYPE06 – Cây rau màu & gia vị
                    new PlantCare
                    {
                        PlantId = "PL034", // Rau muống
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất bùn hoặc đất thịt giữ nước tốt.",
                        FertilizerInfo =
                            "Cần nhiều đạm để phát triển lá. Bón phân ure hoặc phân hữu cơ giàu đạm sau mỗi lần thu hoạch.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL035", // Cà chua
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất tơi xốp, giàu hữu cơ, thoát nước tốt.",
                        FertilizerInfo =
                            "Bón lót và bón thúc định kỳ. Cần nhiều Canxi để tránh bệnh thối đít quả.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL036", // Dưa chuột
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất tơi xốp, giàu dinh dưỡng. Cần làm giàn để leo.",
                        FertilizerInfo =
                            "Ưa phân hữu cơ. Bón thúc định kỳ bằng phân NPK khi cây bắt đầu ra quả.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL037", // Hành lá
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất tơi xốp, giàu mùn.",
                        FertilizerInfo =
                            "Bón phân hữu cơ hoặc NPK giàu đạm sau mỗi lần cắt lá để cây nhanh ra lá mới.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL038", // Ớt
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation = "Đất tơi xốp, thoát nước tốt.",
                        FertilizerInfo =
                            "Bón NPK cân bằng. Tăng cường Kali khi cây ra hoa, đậu quả để quả chắc và cay hơn.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL039", // Xà lách
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Đất tơi xốp, giàu dinh dưỡng, giữ ẩm tốt.",
                        FertilizerInfo =
                            "Chủ yếu cần đạm để phát triển lá. Bón phân pha loãng định kỳ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL040", // Sả
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.TrungBinh,
                        GrowthRate = GrowthRate.Nhanh,
                        SoilRecommendation = "Chịu được nhiều loại đất nhưng cần thoát nước tốt.",
                        FertilizerInfo =
                            "Không kén phân bón. Bón một ít phân NPK hoặc phân hữu cơ vào mùa mưa để cây đẻ nhánh nhiều.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL041", // Sầu riêng R6
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation =
                            "Đất đỏ bazan hoặc đất phù sa cổ, tơi xốp, thoát nước tốt, pH 6.0-6.5.",
                        FertilizerInfo =
                            "Bón phân hữu cơ hoai mục đầu mùa mưa, bổ sung NPK 16-16-8 định kỳ. Tránh úng nước vì dễ thối rễ.",
                    },
                    new PlantCare
                    {
                        PlantId = "PL042", // Sầu riêng Dona
                        LightRequirement = LightRequirement.NangToanPhan,
                        WateringNeeds = WateringNeeds.Cao,
                        GrowthRate = GrowthRate.TrungBinh,
                        SoilRecommendation =
                            "Thích hợp đất bazan, thoát nước tốt, có tầng canh tác sâu và giàu hữu cơ.",
                        FertilizerInfo =
                            "Cần tưới đều trong mùa khô, bón NPK kết hợp phân hữu cơ vi sinh, đặc biệt giai đoạn nuôi trái.",
                    }
                );
        }
        #endregion


        #region Plant Info
        private static void seedPlantInfo(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantInfo>()
                .HasData(
                    // TYPE01 – Cây thân gỗ
                    new PlantInfo
                    {
                        PlantId = "PL001",
                        ScientificName = "Ficus religiosa",
                        CommonName = "Bồ Đề",
                        Description = "Cây linh thiêng, lá hình tim, thường trồng ở chùa.",
                        PlantTypeId = "TYPE01",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095, // 3 năm (cây gỗ lâu năm)
                    },
                    new PlantInfo
                    {
                        PlantId = "PL002",
                        ScientificName = "Lagerstroemia speciosa",
                        CommonName = "Bằng Lăng tím",
                        Description = "Cây thân gỗ, hoa tím rực rỡ, thường thấy ở công viên.",
                        PlantTypeId = "TYPE01",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL003",
                        ScientificName = "Michelia alba",
                        CommonName = "Ngọc Lan",
                        Description = "Cây hoa trắng thơm, thường trồng làm cảnh.",
                        PlantTypeId = "TYPE01",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL004",
                        ScientificName = "Delonix regia",
                        CommonName = "Phượng Vĩ",
                        Description = "Cây hoa đỏ rực, biểu tượng mùa hè học trò.",
                        PlantTypeId = "TYPE01",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095,
                    },
                    // TYPE02 – Cây ăn quả
                    new PlantInfo
                    {
                        PlantId = "PL005",
                        ScientificName = "Mangifera indica",
                        CommonName = "Xoài",
                        Description = "Cây ăn quả phổ biến, quả ngọt, giàu vitamin.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 730, // 2 năm, cây ăn quả lâu năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL006",
                        ScientificName = "Psidium guajava",
                        CommonName = "Ổi",
                        Description = "Cây ăn quả, lá và quả có giá trị dược liệu.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 365, // 1 năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL007",
                        ScientificName = "Cocos nucifera",
                        CommonName = "Dừa",
                        Description = "Cây nhiệt đới, cung cấp nước và dầu dừa.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1460, // 4 năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL008",
                        ScientificName = "Artocarpus heterophyllus",
                        CommonName = "Mít",
                        Description = "Cây ăn quả lớn, quả to, mùi thơm đặc trưng.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 730, // 2 năm
                    },
                    // TYPE03 – Cây công nghiệp
                    new PlantInfo
                    {
                        PlantId = "PL009",
                        ScientificName = "Coffea robusta",
                        CommonName = "Cà phê",
                        Description = "Cây công nghiệp, trồng nhiều ở Tây Nguyên.",
                        PlantTypeId = "TYPE03",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL010",
                        ScientificName = "Hevea brasiliensis",
                        CommonName = "Cao su",
                        Description = "Cây công nghiệp, dùng để lấy mủ cao su.",
                        PlantTypeId = "TYPE03",
                        CreatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1825, // 5 năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL011",
                        ScientificName = "Camellia sinensis",
                        CommonName = "Trà",
                        Description = "Cây công nghiệp, dùng để chế biến trà.",
                        PlantTypeId = "TYPE03",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 730,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL012",
                        ScientificName = "Piper nigrum",
                        CommonName = "Hồ tiêu",
                        Description = "Cây gia vị, xuất khẩu mạnh của Việt Nam.",
                        PlantTypeId = "TYPE03",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1460,
                    },
                    // TYPE04 – Cây lương thực
                    new PlantInfo
                    {
                        PlantId = "PL013",
                        ScientificName = "Oryza sativa",
                        CommonName = "Lúa",
                        Description = "Cây lương thực chính của Việt Nam.",
                        PlantTypeId = "TYPE04",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 120, // 4 tháng
                    },
                    new PlantInfo
                    {
                        PlantId = "PL014",
                        ScientificName = "Zea mays",
                        CommonName = "Ngô",
                        Description = "Cây lương thực, dùng làm thức ăn và nguyên liệu.",
                        PlantTypeId = "TYPE04",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 90, // 3 tháng
                    },
                    new PlantInfo
                    {
                        PlantId = "PL015",
                        ScientificName = "Ipomoea batatas",
                        CommonName = "Khoai lang",
                        Description = "Củ ngọt, dễ trồng, phổ biến ở nông thôn.",
                        PlantTypeId = "TYPE04",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 120,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL016",
                        ScientificName = "Manihot esculenta",
                        CommonName = "Sắn",
                        Description = "Cây lương thực, dùng làm bột và thức ăn chăn nuôi.",
                        PlantTypeId = "TYPE04",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 210, // 7 tháng
                    },
                    // TYPE05 – Cây cảnh
                    new PlantInfo
                    {
                        PlantId = "PL017",
                        ScientificName = "Ochna integerrima",
                        CommonName = "Mai vàng",
                        Description = "Hoa Tết miền Nam, biểu tượng may mắn.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 730,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL018",
                        ScientificName = "Prunus persica",
                        CommonName = "Đào",
                        Description = "Hoa Tết miền Bắc, tượng trưng cho mùa xuân.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 730,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL019",
                        ScientificName = "Tagetes erecta",
                        CommonName = "Cúc vạn thọ",
                        Description = "Hoa trang trí, dễ trồng, màu vàng rực rỡ.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 90,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL020",
                        ScientificName = "Phalaenopsis spp.",
                        CommonName = "Lan Hồ Điệp",
                        Description = "Hoa sang trọng, thường dùng làm quà tặng.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 365,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL021",
                        ScientificName = "Dalbergia tonkinensis",
                        CommonName = "Sưa đỏ",
                        Description = "Cây gỗ quý hiếm, có giá trị kinh tế rất cao, được bảo vệ.",
                        PlantTypeId = "TYPE01",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 3650, // 10 năm+
                    },
                    new PlantInfo
                    {
                        PlantId = "PL022",
                        ScientificName = "Khaya senegalensis",
                        CommonName = "Xà cừ",
                        Description =
                            "Cây bóng mát phổ biến ở đô thị, gỗ dùng trong xây dựng và đồ mộc.",
                        PlantTypeId = "TYPE01",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 2555, // 7 năm+
                    },
                    // TYPE02 – Cây ăn quả (Thêm)
                    new PlantInfo
                    {
                        PlantId = "PL023",
                        ScientificName = "Litchi chinensis",
                        CommonName = "Vải",
                        Description = "Cây ăn quả đặc sản miền Bắc, quả ngọt, mọng nước.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095, // 3 năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL024",
                        ScientificName = "Durio zibethinus",
                        CommonName = "Sầu riêng",
                        Description =
                            "Được mệnh danh là 'vua của các loại trái cây', mùi vị đặc trưng.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1825, // 5 năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL025",
                        ScientificName = "Hylocereus undatus",
                        CommonName = "Thanh long",
                        Description = "Cây họ xương rồng, quả có vị ngọt mát, tốt cho sức khỏe.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 365, // 1 năm
                    },
                    // TYPE03 – Cây công nghiệp (Thêm)
                    new PlantInfo
                    {
                        PlantId = "PL026",
                        ScientificName = "Anacardium occidentale",
                        CommonName = "Điều",
                        Description =
                            "Cây công nghiệp trồng để lấy hạt, một sản phẩm xuất khẩu giá trị.",
                        PlantTypeId = "TYPE03",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 1095, // 3 năm
                    },
                    new PlantInfo
                    {
                        PlantId = "PL027",
                        ScientificName = "Saccharum officinarum",
                        CommonName = "Mía",
                        Description =
                            "Nguyên liệu chính để sản xuất đường, nước giải khát phổ biến.",
                        PlantTypeId = "TYPE03",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 300, // 10 tháng
                    },
                    // TYPE04 – Cây lương thực (Thêm)
                    new PlantInfo
                    {
                        PlantId = "PL028",
                        ScientificName = "Glycine max",
                        CommonName = "Đậu tương (Đậu nành)",
                        Description =
                            "Cây họ đậu giàu protein, dùng làm thực phẩm và thức ăn chăn nuôi.",
                        PlantTypeId = "TYPE04",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 100, // ~3 tháng
                    },
                    new PlantInfo
                    {
                        PlantId = "PL029",
                        ScientificName = "Arachis hypogaea",
                        CommonName = "Lạc (Đậu phộng)",
                        Description = "Cây lương thực lấy củ, dùng để ép dầu hoặc làm thực phẩm.",
                        PlantTypeId = "TYPE04",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 120, // 4 tháng
                    },
                    // TYPE05 – Cây cảnh (Thêm)
                    new PlantInfo
                    {
                        PlantId = "PL030",
                        ScientificName = "Dracaena trifasciata",
                        CommonName = "Lưỡi hổ",
                        Description =
                            "Cây cảnh nội thất phổ biến, có khả năng lọc không khí tốt, rất dễ chăm sóc.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 365,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL031",
                        ScientificName = "Epipremnum aureum",
                        CommonName = "Trầu bà",
                        Description =
                            "Cây dây leo trong nhà, dễ trồng, có thể trồng đất hoặc thủy canh.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 180,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL032",
                        ScientificName = "Rosa chinensis",
                        CommonName = "Hoa hồng",
                        Description =
                            "Biểu tượng của tình yêu, có nhiều màu sắc và chủng loại khác nhau.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 120,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL033",
                        ScientificName = "Nymphaea",
                        CommonName = "Hoa súng",
                        Description =
                            "Cây thủy sinh, hoa nở trên mặt nước, thường trồng trong ao, hồ.",
                        PlantTypeId = "TYPE05",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 180,
                    },
                    // TYPE06 – Cây rau màu & gia vị (Loại mới)
                    new PlantInfo
                    {
                        PlantId = "PL034",
                        ScientificName = "Ipomoea aquatica",
                        CommonName = "Rau muống",
                        Description = "Loại rau ăn lá phổ biến nhất trong bữa ăn của người Việt.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 30, // 1 tháng
                    },
                    new PlantInfo
                    {
                        PlantId = "PL035",
                        ScientificName = "Solanum lycopersicum",
                        CommonName = "Cà chua",
                        Description = "Cây ăn quả, được dùng như một loại rau trong ẩm thực.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 75, // 2.5 tháng
                    },
                    new PlantInfo
                    {
                        PlantId = "PL036",
                        ScientificName = "Cucumis sativus",
                        CommonName = "Dưa chuột",
                        Description = "Quả dùng để ăn sống, làm salad hoặc chế biến món ăn.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 50, // ~1.5 tháng
                    },
                    new PlantInfo
                    {
                        PlantId = "PL037",
                        ScientificName = "Allium fistulosum",
                        CommonName = "Hành lá",
                        Description =
                            "Gia vị không thể thiếu trong nhiều món ăn, dễ trồng tại nhà.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 45,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL038",
                        ScientificName = "Capsicum frutescens",
                        CommonName = "Ớt",
                        Description =
                            "Quả có vị cay, dùng làm gia vị hoặc nguyên liệu cho các món ăn.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 90,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL039",
                        ScientificName = "Lactuca sativa",
                        CommonName = "Xà lách",
                        Description = "Rau ăn sống phổ biến, thành phần chính của các món salad.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 60,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL040",
                        ScientificName = "Cymbopogon citratus",
                        CommonName = "Sả",
                        Description =
                            "Cây gia vị có mùi thơm đặc trưng, dùng trong nấu ăn và y học.",
                        PlantTypeId = "TYPE06",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 120,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL041",
                        ScientificName = "Durio zibethinus 'R6'",
                        CommonName = "Sầu riêng R6",
                        Description =
                            "Giống sầu riêng R6 nổi tiếng ở Việt Nam, cơm vàng, hạt lép, hương vị ngọt béo.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 120,
                    },
                    new PlantInfo
                    {
                        PlantId = "PL042",
                        ScientificName = "Durio zibethinus 'Dona'",
                        CommonName = "Sầu riêng Dona (Monthong)",
                        Description =
                            "Giống sầu riêng Dona (Monthong Thái Lan), cơm dày, vị ngọt nhẹ, được trồng phổ biến ở miền Nam.",
                        PlantTypeId = "TYPE02",
                        CreatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                        HarvestDate = 130,
                    }
                );
        }
        #endregion

        #region Plant img
        private static void seedPlantImg(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PlantImg>()
                .HasData(
                    new PlantImg
                    {
                        ImageId = "IMG001",
                        PlantId = "PL001",
                        ImageUrl =
                            "https://www.thuocdantoc.org/wp-content/uploads/2019/10/cay-bo-de.jpg",
                        Caption = "Cây Bồ Đề cổ thụ",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG002",
                        PlantId = "PL002",
                        ImageUrl =
                            "https://bancuanhanong.com/img/images/products/mshong-bang-lang-giong0.jpg",
                        Caption = "Hoa Bằng Lăng tím",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG003",
                        PlantId = "PL003",
                        ImageUrl =
                            "https://misshoa.com/wp-content/uploads/2020/06/hinh-anh-hoa-ngoc-lan.jpg",
                        Caption = "Hoa Ngọc Lan trắng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG004",
                        PlantId = "PL004",
                        ImageUrl =
                            "https://baoquangbinh.vn/dataimages/202406/original/images786085_images786071_anh_hoa_phuong_vi_do_013046367.jpg",
                        Caption = "Hoa Phượng Vĩ đỏ",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG005",
                        PlantId = "PL005",
                        ImageUrl = "https://resource.kinhtedothi.vn/2024/06/26/1-1716882547.jpg",
                        Caption = "Quả Xoài chín",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG006",
                        PlantId = "PL006",
                        ImageUrl =
                            "https://cdn.youmed.vn/tin-tuc/wp-content/uploads/2020/09/oi5.png",
                        Caption = "Quả Ổi xanh",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG007",
                        PlantId = "PL007",
                        ImageUrl =
                            "https://khachsandayroi.com/wp-content/uploads/2020/04/1564978606054-trong-dua-bo-bien-3.jpg",
                        Caption = "Cây Dừa ven biển",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG008",
                        PlantId = "PL008",
                        ImageUrl =
                            "https://static.kinhtedouong.vn/w640/images/upload/huongtra/08182025/qua-mit-1.jpg",
                        Caption = "Quả Mít vàng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG009",
                        PlantId = "PL009",
                        ImageUrl =
                            "https://vinbarista.com/uploads/news/tim-hieu-cay-ca-phe-a-z-nguon-goc-dac-diem-sinh-hoc-phan-loai-202408131653.jpg",
                        Caption = "Cây Cà phê",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG010",
                        PlantId = "PL010",
                        ImageUrl = "https://caosu.vn/files/assets/cs8.webp",
                        Caption = "Cây Cao su xanh mướt",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG011",
                        PlantId = "PL011",
                        ImageUrl =
                            "https://suckhoehangngay.mediacdn.vn/154880486097817600/2020/11/10/cach-nau-la-tra-xanh-tuoi-de-uong-giam-can-16049983939621864329843.jpg",
                        Caption = "Lá Trà tươi",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG012",
                        PlantId = "PL012",
                        ImageUrl = "https://vnras.com/wp-content/uploads/2023/11/hat-tieu-2.jpg",
                        Caption = "Hồ Tiêu",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG013",
                        PlantId = "PL013",
                        ImageUrl =
                            "https://media-cdn-v2.laodong.vn/storage/newsportal/2024/8/15/1380610/Vanhomc6.jpg",
                        Caption = "Ruộng Lúa xanh mướt",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG014",
                        PlantId = "PL014",
                        ImageUrl =
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQqV8SlyIe-Ufow8IL8qQ_KdEPhj3s8K6tcpQ&s",
                        Caption = "Cây Ngô và bắp vàng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG015",
                        PlantId = "PL015",
                        ImageUrl =
                            "https://images2.thanhnien.vn/zoom/686_429/528068263637045248/2024/6/16/khoai-lang-tim-17185341801521870433944-1-0-384-612-crop-1718534203791803949402.jpg",
                        Caption = "Củ Khoai lang tím",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG016",
                        PlantId = "PL016",
                        ImageUrl =
                            "https://images2.thanhnien.vn/528068263637045248/2024/1/10/cu-san-khung-2-1704871816254294431810.jpg",
                        Caption = "Củ Sắn trắng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG017",
                        PlantId = "PL017",
                        ImageUrl =
                            "https://cdn2.fptshop.com.vn/unsafe/800x0/hinh_anh_hoa_mai_tet_1_d8a15be3ea.png",
                        Caption = "Hoa Mai vàng ngày Tết",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG018",
                        PlantId = "PL018",
                        ImageUrl =
                            "https://media-cdn-v2.laodong.vn/Storage/NewsPortal/2023/1/31/1142999/Hoa-Anh-Dao-4.JPG",
                        Caption = "Hoa Đào hồng rực rỡ",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG019",
                        PlantId = "PL019",
                        ImageUrl =
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQfh5HbPKm23X2BbSWsI5QoWBZV1fAySEtvVA&s",
                        Caption = "Hoa Cúc vạn thọ vàng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG020",
                        PlantId = "PL020",
                        ImageUrl =
                            "https://lanhodiep.vn/wp-content/uploads/2021/10/Lan-ho-diep-mau-cam-mang-ve-dep-tao-nha-sang-trong.jpg",
                        Caption = "Hoa Lan Hồ Điệp sang trọng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG021",
                        PlantId = "PL021",
                        ImageUrl =
                            "https://i.etsystatic.com/18026585/r/il/552977/1859839778/il_794xN.1859839778_vvh3.jpg",
                        Caption = "Gỗ cây Sưa đỏ quý hiếm",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG022",
                        PlantId = "PL022",
                        ImageUrl =
                            "https://thegioicaycongtrinh.com/wp-content/uploads/2022/07/cay-xa-cu-cong-trinh-gia-re-2.jpg",
                        Caption = "Cây Xà cừ tỏa bóng mát",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG023",
                        PlantId = "PL023",
                        ImageUrl =
                            "https://bacgiang.gov.vn/documents/20181/8745446/1592886137045_trai+nghiem+vai+3.jpg/5c7481ce-fb26-43c1-ba0b-8c5ba9601054?t=1592886137049",
                        Caption = "Chùm Vải thiều chín mọng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG024",
                        PlantId = "PL024",
                        ImageUrl =
                            "https://static-images.vnncdn.net/files/publish/2023/5/25/sau-rieng-1-426.jpg",
                        Caption = "Quả Sầu riêng gai góc",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG025",
                        PlantId = "PL025",
                        ImageUrl =
                            "https://thuyanhfruits.com/wp-content/uploads/2020/11/thanh-long-do.jpg",
                        Caption = "Quả Thanh long ruột đỏ",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG026",
                        PlantId = "PL026",
                        ImageUrl =
                            "https://traicaysaynutfarm.com/wp-content/uploads/2021/03/hat-dieu-1.jpg",
                        Caption = "Hạt Điều rang muối",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG027",
                        PlantId = "PL027",
                        ImageUrl =
                            "https://befresh.vn/wp-content/uploads/2023/04/9-tac-dung-cua-nuoc-mia-khien-ban.jpeg-1024x669.jpeg",
                        Caption = "Ly nước Mía mát lạnh",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG028",
                        PlantId = "PL028",
                        ImageUrl =
                            "https://bizweb.dktcdn.net/100/363/007/articles/dau-tuong-dau-nanh-9.jpg?v=1623683502643",
                        Caption = "Hạt Đậu tương (Đậu nành)",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG029",
                        PlantId = "PL029",
                        ImageUrl =
                            "https://file.hstatic.net/1000175970/file/nguyen_lieu__1__29e0d3aacd2c4a66b8491e010314954b_grande.jpg",
                        Caption = "Củ Lạc (Đậu phộng) tươi",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG030",
                        PlantId = "PL030",
                        ImageUrl =
                            "https://vuachaukieng.com/cdn/static/news/2022/637b54d8ca904_860_keep_ratio.jpg",
                        Caption = "Cây Lưỡi hổ trong chậu",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG031",
                        PlantId = "PL031",
                        ImageUrl = "",
                        Caption = "Cây Trầu bà leo cột",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG032",
                        PlantId = "PL032",
                        ImageUrl =
                            "https://img2.thuthuatphanmem.vn/uploads/2019/03/09/bong-hoa-hong-do_114633861.jpg",
                        Caption = "Bông Hoa hồng đỏ thắm",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG033",
                        PlantId = "PL033",
                        ImageUrl =
                            "https://cdn.tgdd.vn/Files/2021/07/28/1371313/cach-trong-hoa-sung-trong-chau-kieng-don-gian-cho-bong-no-quanh-nam-202107280257108539.jpg",
                        Caption = "Hoa súng nở trên mặt hồ",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG034",
                        PlantId = "PL034",
                        ImageUrl =
                            "https://tse3.mm.bing.net/th/id/OIP.32aW-7tU2FRki2BVXyqYTwHaEn?r=0&rs=1&pid=ImgDetMain&o=7&rm=3",
                        Caption = "Đĩa rau muống xào tỏi",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG035",
                        PlantId = "PL035",
                        ImageUrl =
                            "https://tse2.mm.bing.net/th/id/OIP.ZkznhhKneM69DFNWhEOSAwHaHa?r=0&rs=1&pid=ImgDetMain&o=7&rm=3",
                        Caption = "Những quả Cà chua chín đỏ",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG036",
                        PlantId = "PL036",
                        ImageUrl =
                            "https://cdn.tgdd.vn/Files/2021/07/21/1369776/dua-chuot-bao-nhieu-calo-an-dua-chuot-co-giam-can-khong-202107211519389648.jpg",
                        Caption = "Dưa chuột tươi ngon",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG037",
                        PlantId = "PL037",
                        ImageUrl =
                            "https://tse3.mm.bing.net/th/id/OIP.hOD5htl_uf_Y5zhW15toVwAAAA?r=0&rs=1&pid=ImgDetMain&o=7&rm=3",
                        Caption = "Hành lá xanh tươi",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG038",
                        PlantId = "PL038",
                        ImageUrl =
                            "https://suckhoedoisong.qltns.mediacdn.vn/324455921873985536/2022/6/18/cach-an-che-bien-bao-quan-ot-2-1655566236587196946971.jpg",
                        Caption = "Quả Ớt đỏ cay nồng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG039",
                        PlantId = "PL039",
                        ImageUrl =
                            "https://th.bing.com/th/id/R.2dd5796db3374f0920f419c8bae8fbd0?rik=O6LqdjvSbzBfiA&riu=http%3a%2f%2fcdn.tgdd.vn%2fFiles%2f2019%2f12%2f03%2f1224593%2fcach-trong-rau-xa-lach-xanh-tuoi-tai-nha-202201101405579038.jpg&ehk=b5s2XxSdxHzQ%2fVe4nSUGWei8c3sSjOuEVZWtvGtcUxc%3d&risl=&pid=ImgRaw&r=0",
                        Caption = "Rau Xà lách tươi xanh",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG040",
                        PlantId = "PL040",
                        ImageUrl = "https://vnras.com/wp-content/uploads/2023/11/sa-2-1024x725.jpg",
                        Caption = "Cây Sả và tinh dầu",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG041",
                        PlantId = "PL041",
                        ImageUrl =
                            "https://img.lazcdn.com/g/p/f48e393476366397e4444dd9df86534a.jpg_720x720q80.jpg",
                        Caption = "Sầu riêng R6 - cơm vàng, hạt lép, vị ngọt béo đậm đà",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG042",
                        PlantId = "PL041",
                        ImageUrl =
                            "https://caygiong.tiendatbanme.com/wp-content/uploads/2024/06/dia_chi_cung_cap_giong_sau_rieng_ri6_monthon_musang_king_black_thorn.jpg",
                        Caption = "Vườn sầu riêng R6 tại Tây Nguyên - năng suất cao, trái lớn",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG043",
                        PlantId = "PL041",
                        ImageUrl =
                            "https://bizweb.dktcdn.net/100/482/702/products/5-e189baf5-96d2-49af-8341-569e4bb7d9f5.jpg?v=1690703696620",
                        Caption = "Cắt ngang trái sầu riêng R6 - cơm vàng óng, mùi thơm đặc trưng",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG044",
                        PlantId = "PL042",
                        ImageUrl =
                            "https://nongsanhaugiang.com.vn/images/10012020/93bf714a6b523023951f486f5b902be0.jpg",
                        Caption = "Sầu riêng Dona (Monthong) - cơm dày, hạt lép, vị ngọt thanh",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG045",
                        PlantId = "PL042",
                        ImageUrl =
                            "https://thegioicaygiong.net/wp-content/uploads/2021/12/sau-rieng-dona-5.jpg",
                        Caption = "Vườn sầu riêng Dona tại Lâm Đồng - năng suất ổn định",
                    },
                    new PlantImg
                    {
                        ImageId = "IMG046",
                        PlantId = "PL042",
                        ImageUrl =
                            "https://sfarm.vn/wp-content/uploads/2025/05/sau-rieng-thai-dona-la-gi-1.jpg",
                        Caption = "Cơm sầu riêng Dona vàng nhạt, thơm nhẹ, dẻo và béo",
                    }
                );
        }
    }
        #endregion
}
