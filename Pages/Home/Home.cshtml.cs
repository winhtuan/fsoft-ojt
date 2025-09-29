using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;
using Plantpedia.ViewModel;

namespace Plantpedia.Pages.Home
{
    public class HomeModel : PageModel
    {
        private readonly IMemoryCache _cache;
        private readonly IPlantService _plantService;
        private readonly ILookupService _lookupService;

        public List<DropdownViewModel> FilterDropdowns { get; set; } = new();
        public List<PlantDto> PlantCard { get; set; } = new();
        public PagingViewModel Paging { get; set; } = new();

        [BindProperty(SupportsGet = true, Name = "p")]
        public new int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string? PlantTypeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RegionId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ClimateId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? UsageId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SoilTypeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Keywords { get; set; }

        public HomeModel(
            IPlantService plantService,
            ILookupService lookupService,
            IMemoryCache cache
        )
        {
            _plantService = plantService;
            _lookupService = lookupService;
            _cache = cache;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            LoggerHelper.Info("OnGetAsync: Bắt đầu xử lý yêu cầu tải trang chủ.");
            try
            {
                FilterDropdowns = await GetDropdownsAsync();
                await LoadPlantCardsAsync();
                LoggerHelper.Info("OnGetAsync: Tải trang chủ thành công.");
                return Page();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "OnGetAsync: Xảy ra lỗi nghiêm trọng khi tải trang chủ.");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnGetPreviewAsync()
        {
            LoggerHelper.Info(
                "OnGetPreviewAsync: Bắt đầu xử lý yêu cầu tải một phần danh sách cây trồng (preview)."
            );
            try
            {
                await LoadPlantCardsAsync();
                LoggerHelper.Info("OnGetPreviewAsync: Tải partial view thành công.");
                return Partial("_CardPatial", PlantCard);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "OnGetPreviewAsync: Xảy ra lỗi khi tải partial view.");
                return StatusCode(500, "Đã xảy ra lỗi máy chủ khi tải dữ liệu.");
            }
        }

        private async Task<List<DropdownViewModel>> GetDropdownsAsync()
        {
            string cacheKey = "FilterDropdowns";
            if (!_cache.TryGetValue(cacheKey, out List<DropdownViewModel>? dropdowns))
            {
                dropdowns = await _lookupService.GetAllFilterDropdownsAsync();
                _cache.Set(cacheKey, dropdowns, TimeSpan.FromHours(1));
            }
            return dropdowns!;
        }

        private async Task<List<PlantDto>> GetPlantDataAsync(
            string? plantTypeId,
            string? regionId,
            string? climateId,
            string? usageId,
            string? soilTypeId,
            string? keywords
        )
        {
            string cacheKey =
                $"PlantData_{plantTypeId}_{regionId}_{climateId}_{usageId}_{soilTypeId}_{keywords}_{Page}";
            if (!_cache.TryGetValue(cacheKey, out List<PlantDto>? plantList))
            {
                plantList = await _plantService.GetFilteredDtosAsync(
                    plantTypeId,
                    regionId,
                    climateId,
                    usageId,
                    soilTypeId,
                    keywords
                );
                _cache.Set(cacheKey, plantList, TimeSpan.FromMinutes(10));
            }
            return plantList!;
        }

        private async Task LoadPlantCardsAsync()
        {
            LoggerHelper.Info("LoadPlantCardsAsync: Bắt đầu tải danh sách cây trồng theo bộ lọc.");
            try
            {
                LogFilterParameters();

                var filteredPlants = await GetPlantDataAsync(
                    PlantTypeId,
                    RegionId,
                    ClimateId,
                    UsageId,
                    SoilTypeId,
                    Keywords
                );

                LoggerHelper.Info(
                    $"Tìm thấy tổng cộng {filteredPlants.Count} cây trồng phù hợp với bộ lọc."
                );

                if (!filteredPlants.Any() && !string.IsNullOrEmpty(Keywords))
                {
                    LoggerHelper.Warn(
                        $"Không tìm thấy cây trồng nào với từ khóa '{Keywords}' và các bộ lọc đã chọn."
                    );
                }

                var paginatedResult = filteredPlants.Paginate(
                    Page,
                    8,
                    pageNumber =>
                        Url.Page(
                            "/Home/Home",
                            null,
                            new
                            {
                                p = pageNumber,
                                PlantTypeId,
                                RegionId,
                                ClimateId,
                                UsageId,
                                SoilTypeId,
                                Keywords,
                            }
                        ) ?? string.Empty
                );

                PlantCard = paginatedResult.Items;
                Paging = paginatedResult.PagingInfo;

                LoggerHelper.Info($"Hiển thị {PlantCard.Count} cây trồng trên trang {Page}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    "LoadPlantCardsAsync: Xảy ra lỗi trong quá trình lọc và phân trang cây trồng."
                );
                throw;
            }
        }

        private void LogFilterParameters()
        {
            var filters = new StringBuilder("Đang áp dụng các bộ lọc: ");
            var activeFilters = new List<string>();

            if (!string.IsNullOrEmpty(PlantTypeId))
                activeFilters.Add($"Loại cây ID: {PlantTypeId}");
            if (!string.IsNullOrEmpty(RegionId))
                activeFilters.Add($"Khu vực ID: {RegionId}");
            if (!string.IsNullOrEmpty(ClimateId))
                activeFilters.Add($"Khí hậu ID: {ClimateId}");
            if (!string.IsNullOrEmpty(UsageId))
                activeFilters.Add($"Công dụng ID: {UsageId}");
            if (!string.IsNullOrEmpty(SoilTypeId))
                activeFilters.Add($"Loại đất ID: {SoilTypeId}");
            if (!string.IsNullOrEmpty(Keywords))
                activeFilters.Add($"Từ khóa: '{Keywords}'");

            if (activeFilters.Any())
            {
                filters.Append(string.Join(", ", activeFilters));
            }
            else
            {
                filters.Append("Không có bộ lọc nào được áp dụng.");
            }

            LoggerHelper.Info(filters.ToString());
        }
    }
}
