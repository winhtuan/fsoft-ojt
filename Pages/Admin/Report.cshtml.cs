using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Pages;

[Authorize(Policy = "AdminOnly")]
public class ReportModel : PageModel
{
    private readonly IMesciusReportService _reportService;
    private readonly IPlantService _plantService;

    public ReportModel(IMesciusReportService mesciusReport, IPlantService plantService)
    {
        _reportService = mesciusReport;
        _plantService = plantService;
    }

    public async Task<IActionResult> OnPostExportPdfAsync(string type, string? date)
    {
        LoggerHelper.Info($"Yêu cầu xuất báo cáo PDF. Loại: '{type}', Ngày: '{date ?? "Tất cả"}'.");
        try
        {
            var allPlants = await _plantService.GetAllAsync();
            LoggerHelper.Info($"Đã lấy được {allPlants.Count()} bản ghi cây trồng từ service.");

            var filteredPlants = FilterPlants(allPlants, date);
            LoggerHelper.Info($"Sau khi lọc, còn lại {filteredPlants.Count()} bản ghi.");

            var pdfBytes = _reportService.ExportPlantReport(filteredPlants.ToList());
            LoggerHelper.Info(
                $"Đã tạo thành công file PDF với kích thước: {pdfBytes.Length} bytes."
            );

            string downloadDate = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = $"PlantReport_{downloadDate}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("Lỗi nghiêm trọng khi xuất báo cáo PDF.");
            LoggerHelper.Error(ex);
            return StatusCode(500, "Đã xảy ra lỗi trong quá trình tạo báo cáo.");
        }
    }

    public async Task<JsonResult> OnGetChartDataAsync(
        string type = "plantType",
        string? date = null
    )
    {
        LoggerHelper.Info(
            $"Yêu cầu lấy dữ liệu biểu đồ. Loại: '{type}', Ngày: '{date ?? "Tất cả"}'."
        );
        try
        {
            var allPlants = await _plantService.GetAllAsync();
            var filteredPlants = FilterPlants(allPlants, date);

            IEnumerable<IGrouping<string, PlantDto>> grouped = GroupPlants(filteredPlants, type);
            var labels = grouped.Select(x => x.Key).ToArray();
            var data = grouped.Select(x => x.Count()).ToArray();

            LoggerHelper.Info($"Đã tạo dữ liệu biểu đồ thành công với {labels.Length} nhóm.");
            return new JsonResult(new { labels, data });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("Lỗi nghiêm trọng khi lấy dữ liệu biểu đồ.");
            LoggerHelper.Error(ex);
            return new JsonResult(new { error = "Đã xảy ra lỗi server." }) { StatusCode = 500 };
        }
    }

    private IEnumerable<PlantDto> FilterPlants(IEnumerable<PlantDto> plants, string? date)
    {
        if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var pickDate))
            return plants.Where(p => DateOnly.FromDateTime(p.CreatedDate.Date) == pickDate);
        return plants;
    }

    private IEnumerable<IGrouping<string, PlantDto>> GroupPlants(
        IEnumerable<PlantDto> plants,
        string type
    )
    {
        switch (type)
        {
            case "region":
                return plants
                    .SelectMany(p =>
                        p.RegionNames.Select(region => new { Key = region, Plant = p })
                    )
                    .GroupBy(x => x.Key, x => x.Plant);

            case "climate":
                return plants
                    .SelectMany(p =>
                        p.ClimateNames.Select(climate => new { Key = climate, Plant = p })
                    )
                    .GroupBy(x => x.Key, x => x.Plant);

            case "soil":
                return plants
                    .SelectMany(p => p.SoilNames.Select(soil => new { Key = soil, Plant = p }))
                    .GroupBy(x => x.Key, x => x.Plant);

            case "usage":
                return plants
                    .SelectMany(p => p.UsageNames.Select(usage => new { Key = usage, Plant = p }))
                    .GroupBy(x => x.Key, x => x.Plant);

            case "plantType":
                return plants.GroupBy(p => p.PlantTypeName ?? "Khác");

            default:
                return plants.GroupBy(p => p.PlantTypeName ?? "Khác");
        }
    }
}
