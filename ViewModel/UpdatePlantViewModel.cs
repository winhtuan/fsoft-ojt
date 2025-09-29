using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Plantpedia.Enum; // Cần using này cho các enum

namespace Plantpedia.ViewModel
{
    public class UpdatePlantViewModel
    {
        // === Thông tin cơ bản (PlantInfo) ===
        public string? PlantId { get; set; }

        [Required(ErrorMessage = "Tên khoa học là bắt buộc.")]
        public string ScientificName { get; set; }

        [Required(ErrorMessage = "Tên thường gọi là bắt buộc.")]
        public string CommonName { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại cây.")]
        public string PlantTypeId { get; set; }

        [Display(Name = "Số ngày thu hoạch")]
        public int? HarvestDate { get; set; }

        // === Thông tin chăm sóc (PlantCare) ===
        [Required(ErrorMessage = "Yêu cầu ánh sáng là bắt buộc.")]
        public LightRequirement LightRequirement { get; set; }

        [Required(ErrorMessage = "Nhu cầu tưới nước là bắt buộc.")]
        public WateringNeeds WateringNeeds { get; set; }

        public HumidityPreference? HumidityPreference { get; set; }
        public GrowthRate? GrowthRate { get; set; }

        [Required(ErrorMessage = "Đề xuất loại đất là bắt buộc.")]
        public string SoilRecommendation { get; set; }

        [Required(ErrorMessage = "Thông tin phân bón là bắt buộc.")]
        public string FertilizerInfo { get; set; }

        // === Thông tin hình ảnh (PlantImg) ===
        public string? ImageUrl { get; set; } // Để hiển thị ảnh hiện tại
        public string? ImageCaption { get; set; } // Chú thích cho ảnh

        [Display(Name = "Tải ảnh mới")]
        public IFormFile? NewImage { get; set; }

        // === Thông tin quan hệ nhiều-nhiều ===
        public List<string> RegionIds { get; set; } = new();
        public List<string> SoilTypeIds { get; set; } = new();
        public List<string> ClimateIds { get; set; } = new();
        public List<string> UsageIds { get; set; } = new();
    }
}
