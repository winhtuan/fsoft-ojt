using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class ReportController : ControllerBase
{
    private readonly IMesciusReportService _reportService;
    private readonly IPlantService _plantService;

    public ReportController(IMesciusReportService reportService, IPlantService plantService)
    {
        _reportService = reportService;
        _plantService = plantService;
    }

    /// <summary>
    /// Trả dữ liệu cho biểu đồ + KPI cơ bản.
    /// GET /api/report/chart-data?type=plantType&date=yyyy-MM-dd
    /// </summary>
    [HttpGet("chart-data")]
    public async Task<ActionResult<ChartDataResponse>> GetChartData(
        [FromQuery] string type = "plantType",
        [FromQuery] string? date = null
    )
    {
        LoggerHelper.Info($"[API] chart-data → type='{type}', date='{date ?? "Tất cả"}'");

        try
        {
            var allPlants = await _plantService.GetAllAsync();
            var filtered = FilterPlants(allPlants, date);
            var grouped = GroupPlants(filtered, type).ToList();

            var labels = grouped.Select(g => g.Key).ToArray();
            var data = grouped.Select(g => g.Count()).ToArray();
            var total = data.Sum();

            string? topType = null;
            if (labels.Length > 0 && data.Length == labels.Length)
            {
                var maxVal = data.Max();
                var maxIndex = Array.IndexOf(data, maxVal);
                topType = maxIndex >= 0 ? labels[maxIndex] : null;
            }

            // Hiện tại chưa có dữ liệu tăng trưởng / yêu thích => trả 0 (JS đã handle)
            var resp = new ChartDataResponse
            {
                Labels = labels,
                Data = data,
                Total = total,
                TopType = topType,
                Growth = 0, // có thể tính nếu bạn có kỳ trước
                FavoritesNew = 0, // có thể điền từ service khác
            };

            return Ok(resp);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("[API] chart-data error");
            LoggerHelper.Error(ex);
            return StatusCode(500, new { error = "Đã xảy ra lỗi server." });
        }
    }

    /// <summary>
    /// Xuất PDF theo bộ lọc hiện tại.
    /// Nhận POST form: type, date (yyyy-MM-dd) — tương thích form hiện tại.
    /// </summary>
    [HttpPost("export-pdf")]
    [Consumes("application/x-www-form-urlencoded", "multipart/form-data", "application/json")]
    public async Task<IActionResult> ExportPdf([FromForm] ExportPdfRequest form) // dùng FromForm để hợp với <form>
    {
        var type = string.IsNullOrWhiteSpace(form.Type) ? "plantType" : form.Type;
        var date = string.IsNullOrWhiteSpace(form.Date) ? null : form.Date;

        LoggerHelper.Info($"[API] export-pdf → type='{type}', date='{date ?? "Tất cả"}'");

        try
        {
            var allPlants = await _plantService.GetAllAsync();
            var filtered = FilterPlants(allPlants, date);
            var pdfBytes = _reportService.ExportPlantReport(filtered.ToList());

            var fileName = $"PlantReport_{DateTime.UtcNow:yyyy-MM-dd}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("[API] export-pdf error");
            LoggerHelper.Error(ex);
            return StatusCode(500, "Đã xảy ra lỗi trong quá trình tạo báo cáo.");
        }
    }

    /* ───────────── Private helpers ───────────── */

    private static IEnumerable<PlantDto> FilterPlants(IEnumerable<PlantDto> plants, string? date)
    {
        if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var pickDate))
            return plants.Where(p => DateOnly.FromDateTime(p.CreatedDate.Date) == pickDate);

        return plants;
    }

    private static IEnumerable<IGrouping<string, PlantDto>> GroupPlants(
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
            default:
                return plants.GroupBy(p => p.PlantTypeName ?? "Khác");
        }
    }
}
