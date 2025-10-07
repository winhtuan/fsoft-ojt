using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Pages.Plant
{
    public class DetailsModel : PageModel
    {
        private readonly IPlantService _plantService;
        public PlantDto Plant { get; private set; }

        public DetailsModel(IPlantService plantService)
        {
            _plantService = plantService;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            LoggerHelper.Info($"Bắt đầu lấy chi tiết cho cây trồng có ID: '{id}'.");

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    LoggerHelper.Warn("ID bị trống, không thể truy vấn chi tiết cây trồng.");
                    return BadRequest();
                }

                Plant = await _plantService.GetPlantById(id);
                if (Plant == null || string.IsNullOrEmpty(Plant.PlantId))
                {
                    LoggerHelper.Warn($"Không tìm thấy cây trồng nào với ID: '{id}'.");
                    return NotFound();
                }

                ViewData["Title"] = Plant.CommonName;
                LoggerHelper.Info(
                    $"Hiển thị thành công chi tiết cho cây '{Plant.CommonName}' (ID: {id})."
                );

                return Page();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi khi tải chi tiết cây trồng.");
                return RedirectToPage("/Error");
            }
        }
    }
}
