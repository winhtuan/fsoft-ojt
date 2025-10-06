using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;
using Plantpedia.ViewModel;

namespace Plantpedia.Pages
{
    [Authorize(Policy = "AdminOnly")]
    public class PlantModel : PageModel
    {
        private readonly ILookupService _lookupService;
        private readonly IPlantService _plantService;
        private readonly IMemoryCache _cache;

        public List<DropdownViewModel> FilterDropdowns { get; set; } = [];
        public List<PlantDto> Plants { get; set; } = new();
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

        public PlantModel(
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
            LoggerHelper.Info("OnGetAsync: Bắt đầu tải trang quản lý danh sách cây trồng.");
            try
            {
                FilterDropdowns = await GetDropdownsAsync();
                await LoadPlantListsAsync();
                LoggerHelper.Info("OnGetAsync: Tải trang quản lý thành công.");
                return Page();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    "OnGetAsync: Xảy ra lỗi nghiêm trọng khi tải trang quản lý."
                );
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnGetFilterTableAsync()
        {
            LoggerHelper.Info("OnGetFilterTableAsync: Bắt đầu xử lý yêu cầu lọc lại bảng dữ liệu.");
            try
            {
                await LoadPlantListsAsync();
                LoggerHelper.Info("OnGetFilterTableAsync: Lọc và tải lại bảng thành công.");
                return Partial("_ListPlantPartial", Plants);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    "OnGetFilterTableAsync: Xảy ra lỗi khi lọc lại bảng dữ liệu."
                );
                return StatusCode(500, "Lỗi máy chủ khi lọc dữ liệu.");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            LoggerHelper.Info(
                $"OnPostDeleteAsync: Bắt đầu xử lý yêu cầu xóa cây trồng ID: '{id}'."
            );
            if (string.IsNullOrEmpty(id))
            {
                LoggerHelper.Warn("OnPostDeleteAsync: Yêu cầu xóa không hợp lệ do ID bị trống.");
                return BadRequest();
            }

            try
            {
                var success = await _plantService.DeleteByIdAsync(id);
                if (success)
                {
                    LoggerHelper.Info($"Đã xóa thành công cây trồng với ID: '{id}'.");
                    // Xóa cache liên quan
                    RemovePlantCache();
                }
                else
                {
                    LoggerHelper.Warn($"Không thể xóa cây trồng với ID: '{id}' vì không tìm thấy.");
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    $"Đã xảy ra lỗi không mong muốn khi xóa cây trồng với ID: '{id}'."
                );
            }

            return RedirectToPage(
                new
                {
                    p = Page,
                    PlantTypeId,
                    RegionId,
                    ClimateId,
                    UsageId,
                    SoilTypeId,
                    Keywords,
                }
            );
        }

        // Caching cho filter dropdowns
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

        // Caching cho danh sách cây trồng
        private async Task<List<PlantDto>> GetPlantDataAsync(
            string? plantTypeId,
            string? regionId,
            string? climateId,
            string? usageId,
            string? soilTypeId,
            string? keywords,
            int page
        )
        {
            string cacheKey =
                $"Plants_{plantTypeId}_{regionId}_{climateId}_{usageId}_{soilTypeId}_{keywords}_{page}";
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

        // Xóa cache cây trồng khi có thao tác xóa (hoặc thêm/sửa)
        private void RemovePlantCache()
        {
            var keysToRemove = new List<string>();
            var field = typeof(MemoryCache).GetField(
                "_entries",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            if (field != null)
            {
                var entries = field.GetValue(_cache) as IDictionary<object, object>;
                if (entries != null)
                {
                    foreach (var key in entries.Keys)
                    {
                        if (key.ToString()?.StartsWith("Plants_") == true)
                        {
                            keysToRemove.Add(key.ToString()!);
                        }
                    }
                }
            }
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        private async Task LoadPlantListsAsync()
        {
            LoggerHelper.Info("LoadPlantListsAsync: Bắt đầu tải danh sách cây trồng theo bộ lọc.");
            try
            {
                LogFilterParameters();

                // Lấy danh sách cây trồng đã cache
                var filteredPlants = await GetPlantDataAsync(
                    PlantTypeId,
                    RegionId,
                    ClimateId,
                    UsageId,
                    SoilTypeId,
                    Keywords,
                    Page
                );

                LoggerHelper.Info($"Tìm thấy tổng cộng {filteredPlants.Count} cây trồng phù hợp.");

                var paginatedResult = filteredPlants.Paginate(
                    Page,
                    5,
                    pageNumber =>
                        Url.Page(
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

                Plants = paginatedResult.Items;
                Paging = paginatedResult.PagingInfo;

                LoggerHelper.Info($"Hiển thị {Plants.Count} cây trồng trên trang {Page}.");
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    ex,
                    "LoadPlantListsAsync: Xảy ra lỗi trong quá trình lọc và phân trang."
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
