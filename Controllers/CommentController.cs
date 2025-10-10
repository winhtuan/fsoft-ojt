using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] PlantCommentCreateRequest req)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { success = false, message = "Người dùng không hợp lệ." });
            }

            LoggerHelper.Info($"POST create comment Plant={req.PlantId}, User={userIdString}");

            try
            {
                var dto = await _service.CreateAsync(req, userId);
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> React([FromBody] PlantCommentReactionRequest req)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { success = false, message = "Người dùng không hợp lệ." });
            }

            LoggerHelper.Info(
                $"POST react Comment={req.CommentId}, User={userIdString}, Type={req.ReactionType}"
            );

            try
            {
                int reactCount = await _service.ToggleReactionAsync(req, userId);
                return Ok(new { success = true, data = new { reactCount } });
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

        [HttpPut("update")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] PlantCommentUpdateRequest req)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { success = false, message = "Người dùng không hợp lệ." });
            }

            try
            {
                await _service.UpdateAsync(req, userId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi khi cập nhật comment");
                return StatusCode(
                    500,
                    new { success = false, message = "Không thể cập nhật bình luận." }
                );
            }
        }

        [HttpDelete("delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] PlantCommentDeleteRequest req)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { success = false, message = "Người dùng không hợp lệ." });
            }

            try
            {
                await _service.DeleteAsync(req.CommentId, userId);
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi khi xóa comment");
                return StatusCode(
                    500,
                    new { success = false, message = "Không thể xóa bình luận." }
                );
            }
        }
    }
}
