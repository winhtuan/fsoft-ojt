using System.Linq;
using AutoMapper;
using Plantpedia.DTO;
using Plantpedia.Models;
using Plantpedia.ViewModel;

namespace Plantpedia.Mapping
{
    public class PlantProfile : Profile
    {
        public PlantProfile()
        {
            // =================================================================
            // 1. MAPPING ĐỂ ĐỌC DỮ LIỆU (ENTITY -> DTO)
            // Mục đích: Chuyển đổi đối tượng từ Database (PlantInfo) sang
            // đối tượng truyền tải dữ liệu (PlantDto) để hiển thị.
            // =================================================================
            CreateMap<PlantInfo, PlantDto>()
                // Ánh xạ các thuộc tính từ bảng phụ PlantCare
                .ForMember(
                    dest => dest.LightRequirement,
                    opt => opt.MapFrom(src => src.CareInfo.LightRequirement)
                )
                .ForMember(
                    dest => dest.WateringNeeds,
                    opt => opt.MapFrom(src => src.CareInfo.WateringNeeds)
                )
                .ForMember(
                    dest => dest.HumidityPreference,
                    opt => opt.MapFrom(src => src.CareInfo.HumidityPreference)
                )
                .ForMember(
                    dest => dest.GrowthRate,
                    opt => opt.MapFrom(src => src.CareInfo.GrowthRate)
                )
                .ForMember(
                    dest => dest.SoilRecommendation,
                    opt => opt.MapFrom(src => src.CareInfo.SoilRecommendation)
                )
                .ForMember(
                    dest => dest.FertilizerInfo,
                    opt => opt.MapFrom(src => src.CareInfo.FertilizerInfo)
                )
                // Ánh xạ tên từ các bảng liên quan
                .ForMember(
                    dest => dest.PlantTypeName,
                    opt => opt.MapFrom(src => src.PlantType.TypeName)
                )
                // Lấy URL ảnh đầu tiên làm ảnh đại diện
                .ForMember(
                    dest => dest.ImageUrl,
                    opt =>
                        opt.MapFrom(src =>
                            src.PlantImages.Select(img => img.ImageUrl).FirstOrDefault()
                        )
                )
                // Lấy danh sách các URL ảnh (cho trang chi tiết)
                .ForMember(
                    dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.PlantImages.Select(img => img.ImageUrl))
                )
                // Lấy danh sách các TÊN (cho trang chi tiết)
                .ForMember(
                    dest => dest.RegionNames,
                    opt => opt.MapFrom(src => src.PlantRegions.Select(r => r.Region.Name))
                )
                .ForMember(
                    dest => dest.SoilNames,
                    opt => opt.MapFrom(src => src.PlantSoils.Select(s => s.SoilType.Name))
                )
                .ForMember(
                    dest => dest.ClimateNames,
                    opt => opt.MapFrom(src => src.PlantClimates.Select(c => c.Climate.Name))
                )
                .ForMember(
                    dest => dest.UsageNames,
                    opt => opt.MapFrom(src => src.PlantUsages.Select(u => u.Usage.Name))
                )
                // Lấy danh sách các ID (để đổ vào form Edit)
                .ForMember(
                    dest => dest.RegionIds,
                    opt => opt.MapFrom(src => src.PlantRegions.Select(r => r.RegionId))
                )
                .ForMember(
                    dest => dest.SoilTypeIds,
                    opt => opt.MapFrom(src => src.PlantSoils.Select(s => s.SoilTypeId))
                )
                .ForMember(
                    dest => dest.ClimateIds,
                    opt => opt.MapFrom(src => src.PlantClimates.Select(c => c.ClimateId))
                )
                .ForMember(
                    dest => dest.UsageIds,
                    opt => opt.MapFrom(src => src.PlantUsages.Select(u => u.UsageId))
                );

            // =================================================================
            // 2. MAPPING ĐỂ ĐỔ DỮ LIỆU VÀO FORM (DTO -> VIEWMODEL)
            // Mục đích: Chuyển dữ liệu đã đọc (PlantDto) sang ViewModel
            // để hiển thị trên form cập nhật.
            // =================================================================
            CreateMap<PlantDto, UpdatePlantViewModel>();

            // =================================================================
            // 3. MAPPING ĐỂ GHI DỮ LIỆU (VIEWMODEL -> ENTITY)
            // Mục đích: Chuyển dữ liệu từ form (UpdatePlantViewModel) sang
            // đối tượng Database (PlantInfo) để tạo mới hoặc cập nhật.
            // =================================================================

            // Quy tắc chính: Từ ViewModel sang PlantInfo
            CreateMap<UpdatePlantViewModel, PlantInfo>()
                // Bỏ qua các thuộc tính được quản lý thủ công trong Service hoặc bởi Database
                .ForMember(dest => dest.PlantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.PlantType, opt => opt.Ignore()) // EF tự quản lý qua PlantTypeId
                // Các collection (danh sách) sẽ được xử lý riêng trong Service để cập nhật quan hệ
                .ForMember(dest => dest.PlantImages, opt => opt.Ignore())
                .ForMember(dest => dest.PlantRegions, opt => opt.Ignore())
                .ForMember(dest => dest.PlantSoils, opt => opt.Ignore())
                .ForMember(dest => dest.PlantClimates, opt => opt.Ignore())
                .ForMember(dest => dest.PlantUsages, opt => opt.Ignore())
                // Sử dụng quy tắc map phụ bên dưới để điền dữ liệu cho đối tượng CareInfo
                .ForMember(dest => dest.CareInfo, opt => opt.MapFrom(src => src));

            // Quy tắc phụ: Từ ViewModel sang PlantCare (được sử dụng bởi quy tắc ở trên)
            CreateMap<UpdatePlantViewModel, PlantCare>()
                .ForMember(dest => dest.PlantId, opt => opt.Ignore()); // Khóa ngoại do EF quản lý
        }
    }
}
