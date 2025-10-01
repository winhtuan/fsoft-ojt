using System;
using System.Collections.Generic;
using Plantpedia.Enum;

namespace Plantpedia.DTO
{
    public class PlantDto
    {
        // =================================================================
        // THÔNG TIN NHẬN DẠNG CƠ BẢN
        // (Thường dùng để hiển thị trên thẻ và các danh sách)
        // =================================================================
        public string PlantId { get; set; } = string.Empty;
        public string CommonName { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; // Ảnh đại diện

        // =================================================================
        // THÔNG TIN CHI TIẾT
        // (Thường dùng để hiển thị trên trang chi tiết)
        // =================================================================
        public string Description { get; set; } = string.Empty;

        // --- Phân loại ---
        public string PlantTypeName { get; set; } = string.Empty;
        public int? HarvestDate { get; set; }

        // --- Thông tin chăm sóc ---
        public LightRequirement LightRequirement { get; set; }
        public WateringNeeds WateringNeeds { get; set; }
        public HumidityPreference HumidityPreference { get; set; }
        public GrowthRate GrowthRate { get; set; }
        public string SoilRecommendation { get; set; } = string.Empty;
        public string FertilizerInfo { get; set; } = string.Empty;

        // --- Các danh sách tên (để hiển thị) ---
        public List<string> RegionNames { get; set; } = new();
        public List<string> SoilNames { get; set; } = new();
        public List<string> ClimateNames { get; set; } = new();
        public List<string> UsageNames { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new(); // Dùng cho gallery ảnh

        // =================================================================
        // DỮ LIỆU DÙNG CHO LOGIC VÀ FORM
        // (Thường dùng để đổ dữ liệu vào form chỉnh sửa)
        // =================================================================
        public string PlantTypeId { get; set; } = string.Empty;
        public List<string> RegionIds { get; set; } = new();
        public List<string> SoilTypeIds { get; set; } = new();
        public List<string> ClimateIds { get; set; } = new();
        public List<string> UsageIds { get; set; } = new();

        // =================================================================
        // DỮ LIỆU METADATA
        // =================================================================
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
