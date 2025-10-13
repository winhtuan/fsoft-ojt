using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantDiagnosisController : ControllerBase
    {
        private static readonly string[] OkTypes =
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif",
        };
        private readonly IPlantDiagnosisService _plantDiagnosisService;

        public PlantDiagnosisController(IPlantDiagnosisService plantDiagnosisService)
        {
            _plantDiagnosisService = plantDiagnosisService;
        }

        [HttpPost("detect")]
        public async Task<IActionResult> Diagnose([FromForm] IFormFile image)
        {
            // ====== Log đầu vào ======
            if (image == null || image.Length == 0)
            {
                LoggerHelper.Warn("Nhận yêu cầu chẩn đoán nhưng không kèm tệp ảnh hoặc ảnh rỗng.");
                return BadRequest(new { success = false, message = "Bạn chưa chọn file!" });
            }
            var contentType = string.IsNullOrWhiteSpace(image.ContentType)
                ? ""
                : image.ContentType.Trim().ToLowerInvariant();

            if (Array.IndexOf(OkTypes, contentType) < 0)
            {
                LoggerHelper.Warn($"Loại file không hợp lệ: {image.ContentType}");
                return BadRequest(
                    new { success = false, message = "Vui lòng chọn ảnh JPG/PNG/WebP/GIF." }
                );
            }
            LoggerHelper.Info(
                $"Nhận yêu cầu chẩn đoán. Tệp: {image.FileName} | Kích thước: {image.Length} bytes | Loại: {image.ContentType}"
            );

            try
            {
                // ====== Gọi service ======
                var result = await _plantDiagnosisService.DiagnoseAsync(image);

                if (result == null)
                {
                    LoggerHelper.Error("AI service trả về 'null' hoặc lỗi trong quá trình xử lý.");
                    return StatusCode(
                        502,
                        new { success = false, message = "Lỗi khi gửi tới AI service!" }
                    );
                }

                // ====== Log tóm tắt kết quả ======
                try
                {
                    if (result.Value.TryGetProperty("model_version", out var mv))
                        LoggerHelper.Info($"Model sử dụng: {mv.GetString()}");

                    if (result.Value.TryGetProperty("result", out var r))
                    {
                        string plantFlag = "(không có)";
                        if (
                            r.TryGetProperty("is_plant", out var ip)
                            && ip.ValueKind == System.Text.Json.JsonValueKind.True
                        )
                            plantFlag = "true";
                        else if (
                            r.TryGetProperty("is_plant", out ip)
                            && ip.ValueKind == System.Text.Json.JsonValueKind.Object
                            && ip.TryGetProperty("binary", out var bin)
                            && bin.ValueKind == System.Text.Json.JsonValueKind.True
                        )
                            plantFlag = "true";

                        LoggerHelper.Info($"Kết quả có trường 'result'. is_plant = {plantFlag}");
                    }
                }
                catch { }

                // ====== Trả về client ======
                LoggerHelper.Info("Trả kết quả chẩn đoán cho client thành công.");
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex); // ghi đầy đủ stacktrace
                return StatusCode(
                    500,
                    new { success = false, message = "Có lỗi xảy ra khi xử lý yêu cầu!" }
                );
            }
        }
    }
}
