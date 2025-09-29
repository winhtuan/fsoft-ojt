using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Helper;
using Plantpedia.Service;
using Plantpedia.ViewModel;

namespace Plantpedia.Pages.Plant
{
    [Authorize]
    public class UpdatePlantModel : PageModel
    {
        private readonly IPlantService _plantService;
        private readonly ILookupService _lookupService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        [BindProperty]
        public UpdatePlantViewModel Plant { get; set; } = new();

        public List<CheckboxViewModel> Regions { get; set; } = new();
        public List<CheckboxViewModel> SoilTypes { get; set; } = new();
        public List<CheckboxViewModel> Climates { get; set; } = new();
        public List<CheckboxViewModel> Usages { get; set; } = new();

        public List<SelectListItem> PlantTypes { get; set; } = new();

        public bool IsCreating => string.IsNullOrEmpty(Plant.PlantId);
        public string PageTitle =>
            IsCreating ? "Thêm cây trồng mới" : "Chỉnh sửa thông tin cây trồng";
        public string SubmitButtonText => IsCreating ? "Thêm mới" : "Cập nhật";

        public UpdatePlantModel(
            IPlantService plantService,
            ILookupService lookupService,
            IServiceScopeFactory scopeFactory,
            IMapper mapper
        )
        {
            _plantService = plantService;
            _lookupService = lookupService;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            var act = string.IsNullOrEmpty(id)
                ? "tạo cây trồng mới"
                : $"cập nhật cây trồng ID: {id}";
            LoggerHelper.Info($"OnGetAsync: Bắt đầu tải trang để {act}.");

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    // Chế độ tạo mới
                    Plant.LightRequirement = System
                        .Enum.GetValues(typeof(LightRequirement))
                        .Cast<LightRequirement>()
                        .First();
                    Plant.WateringNeeds = System
                        .Enum.GetValues(typeof(WateringNeeds))
                        .Cast<WateringNeeds>()
                        .First();
                    Plant.HumidityPreference = System
                        .Enum.GetValues(typeof(HumidityPreference))
                        .Cast<HumidityPreference>()
                        .First();
                    Plant.GrowthRate = System
                        .Enum.GetValues(typeof(GrowthRate))
                        .Cast<GrowthRate>()
                        .First();
                    await LoadLookupsAsync();
                    LoggerHelper.Info("Đã chuẩn bị form để tạo cây trồng mới thành công.");
                }
                else
                {
                    // Chế độ cập nhật
                    var plantDto = await _plantService.GetPlantById(id);
                    if (plantDto == null || string.IsNullOrEmpty(plantDto.PlantId))
                    {
                        LoggerHelper.Warn($"OnGetAsync: Không tìm thấy cây trồng với ID '{id}'.");
                        return NotFound();
                    }
                    LoggerHelper.Info(
                        $"Đã tìm thấy cây trồng '{plantDto.CommonName}' để cập nhật."
                    );
                    Plant = _mapper.Map<UpdatePlantViewModel>(plantDto);
                    await LoadLookupsAsync(plantDto);
                }
                return Page();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"OnGetAsync: Xảy ra lỗi nghiêm trọng khi tải trang để {act}."
                );
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var act = IsCreating ? "tạo mới" : "cập nhật";
            LoggerHelper.Info(
                $"OnPostAsync: Bắt đầu xử lý yêu cầu {act} cây trồng '{Plant.CommonName}'."
            );
            if (!ModelState.IsValid)
            {
                LoggerHelper.Warn(
                    $"OnPostAsync: Dữ liệu (ModelState) không hợp lệ cho cây trồng '{Plant.CommonName}'. Hiển thị lại form."
                );
                await LoadLookupsAsync();
                return Page();
            }
            try
            {
                // Chỉ xử lý form nếu model hợp lệ
                Plant.RegionIds = Request
                    .Form["RegionIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => id!)
                    .ToList();

                Plant.SoilTypeIds = Request
                    .Form["SoilTypeIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => id!)
                    .ToList();

                Plant.ClimateIds = Request
                    .Form["ClimateIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => id!)
                    .ToList();

                Plant.UsageIds = Request
                    .Form["UsageIds"]
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => id!)
                    .ToList();

                if (IsCreating)
                {
                    await _plantService.CreatePlantAsync(Plant);
                    LoggerHelper.Info($"Tạo mới thành công cây trồng '{Plant.CommonName}'.");
                }
                else
                {
                    await _plantService.UpdatePlantAsync(Plant);
                    LoggerHelper.Info($"Cập nhật thành công cây trồng có ID: {Plant.PlantId}.");
                }

                return RedirectToPage("/Admin/Plant");
            }
            catch (KeyNotFoundException knfex)
            {
                LoggerHelper.Error(
                    knfex,
                    $"OnPostAsync: Cập nhật thất bại. Không tìm thấy cây trồng với ID {Plant.PlantId}."
                );
                ModelState.AddModelError(
                    string.Empty,
                    "Cây trồng bạn đang sửa không còn tồn tại. Có thể nó đã bị người khác xóa."
                );
                await LoadLookupsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"OnPostAsync: Xảy ra lỗi không mong muốn khi đang lưu cây trồng '{Plant.CommonName}'."
                );
                ModelState.AddModelError(
                    string.Empty,
                    "Đã có lỗi không mong muốn xảy ra khi lưu. Vui lòng thử lại."
                );
                await LoadLookupsAsync();
                return Page();
            }
        }

        private async Task LoadLookupsAsync(PlantDto? currentPlant = null)
        {
            LoggerHelper.Info(
                "LoadLookupsAsync: Bắt đầu tải dữ liệu cho các dropdown và checkbox."
            );
            try
            {
                async Task<T> ExecuteInScope<T>(Func<ILookupService, Task<T>> operation)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var lookupService = scope.ServiceProvider.GetRequiredService<ILookupService>();
                    return await operation(lookupService);
                }

                // Định nghĩa các tác vụ
                var plantTypesTask = ExecuteInScope(service => service.GetPlantTypesAsync());
                var regionsTask = ExecuteInScope(service => service.GetRegionsAsync());
                var soilTypesTask = ExecuteInScope(service => service.GetSoilTypesAsync());
                var climatesTask = ExecuteInScope(service => service.GetClimatesAsync());
                var usagesTask = ExecuteInScope(service => service.GetUsagesAsync());

                // Đợi tất cả hoàn thành
                await Task.WhenAll(
                    plantTypesTask,
                    regionsTask,
                    soilTypesTask,
                    climatesTask,
                    usagesTask
                );

                // Gán kết quả và thực hiện mapping
                PlantTypes = (await plantTypesTask)
                    .Select(pt => new SelectListItem { Value = pt.Id, Text = pt.Name })
                    .ToList();

                Regions = (await regionsTask)
                    .Select(r => new CheckboxViewModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        IsChecked = currentPlant?.RegionIds.Contains(r.Id) ?? false,
                    })
                    .ToList();

                SoilTypes = (await soilTypesTask)
                    .Select(s => new CheckboxViewModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsChecked = currentPlant?.SoilTypeIds.Contains(s.Id) ?? false,
                    })
                    .ToList();

                Climates = (await climatesTask)
                    .Select(c => new CheckboxViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        IsChecked = currentPlant?.ClimateIds.Contains(c.Id) ?? false,
                    })
                    .ToList();

                Usages = (await usagesTask)
                    .Select(u => new CheckboxViewModel
                    {
                        Id = u.Id,
                        Name = u.Name,
                        IsChecked = currentPlant?.UsageIds.Contains(u.Id) ?? false,
                    })
                    .ToList();

                LoggerHelper.Info("LoadLookupsAsync: Đã tải thành công tất cả dữ liệu.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    "LoadLookupsAsync: Xảy ra lỗi nghiêm trọng khi tải dữ liệu song song."
                );
                throw;
            }
        }
    }
}
