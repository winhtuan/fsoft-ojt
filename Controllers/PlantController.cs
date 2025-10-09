using Microsoft.AspNetCore.Mvc;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlantDetailById(string id)
        {
            LoggerHelper.Info($"Bắt đầu lấy chi tiết cho cây trồng có ID: '{id}'.");

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    LoggerHelper.Warn("ID bị trống, không thể truy vấn.");
                    // Trả về lỗi 400 Bad Request
                    return BadRequest(new { message = "Plant ID cannot be empty." });
                }

                var plant = await _plantService.GetPlantById(id);

                if (plant == null || string.IsNullOrEmpty(plant.PlantId))
                {
                    LoggerHelper.Warn($"Không tìm thấy cây trồng nào với ID: '{id}'.");
                    // Trả về lỗi 404 Not Found
                    return NotFound(new { message = $"Plant with ID '{id}' not found." });
                }

                LoggerHelper.Info(
                    $"Trả về thành công dữ liệu cho cây '{plant.CommonName}' (ID: {id})."
                );
                // Trả về dữ liệu plant dưới dạng JSON với status code 200 OK
                return Ok(plant);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Lỗi nghiêm trọng khi tải chi tiết cây trồng ID: {id}.");
                // Trả về lỗi 500 Internal Server Error
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }
    }
}
