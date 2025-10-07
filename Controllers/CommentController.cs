using Microsoft.AspNetCore.Mvc;
using Plantpedia.DTO;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        [HttpGet("{plantId}")]
        public async Task<IActionResult> GetByPlant(string plantId)
        {
            LoggerHelper.Info($"GET list comment plant={plantId}");
            try
            {
                var data = await _service.GetCommentsByPlantAsync(plantId);
                LoggerHelper.Info($"Trả về {data.Count} comment");
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi khi lấy danh sách comment");
                return StatusCode(
                    500,
                    new { success = false, message = "Không thể tải bình luận." }
                );
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PlantCommentCreateRequest req)
        {
            LoggerHelper.Info($"POST create comment Plant={req.PlantId}, User={req.UserId}");
            try
            {
                var dto = await _service.CreateAsync(req);
                return Ok(new { success = true, data = dto });
            }
            catch (ArgumentException ex)
            {
                LoggerHelper.Warn($"Dữ liệu không hợp lệ: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi khi tạo comment");
                return StatusCode(
                    500,
                    new { success = false, message = "Không thể tạo bình luận." }
                );
            }
        }

        [HttpPost("react")]
        public async Task<IActionResult> React([FromBody] PlantCommentReactionRequest req)
        {
            LoggerHelper.Info(
                $"POST react Comment={req.CommentId}, User={req.UserId}, Type={req.ReactionType}"
            );
            try
            {
                await _service.ToggleReactionAsync(req);
                return Ok(new { success = true });
            }
            catch (ArgumentException ex)
            {
                LoggerHelper.Warn($"Dữ liệu không hợp lệ: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi khi cập nhật reaction");
                return StatusCode(
                    500,
                    new { success = false, message = "Không thể cập nhật phản ứng." }
                );
            }
        }
    }
}
