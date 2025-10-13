using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plantpedia.Helper;
using Plantpedia.Service;

namespace Plantpedia.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IUserFavoriteService _service;

    public FavoriteController(IUserFavoriteService service)
    {
        _service = service;
    }

    // Toggle favorite
    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleFavorite([FromBody] string plantId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            LoggerHelper.Warn("Yêu cầu toggle favorite nhưng user không hợp lệ.");
            return Unauthorized();
        }

        try
        {
            LoggerHelper.Info($"ToggleFavorite: user={userId}, plantId={plantId}");
            var isFavorite = await _service.ToggleFavoriteAsync(userId, plantId);
            LoggerHelper.Info(
                $"ToggleFavorite kết quả: user={userId}, plantId={plantId}, isFavorite={isFavorite}"
            );
            return Ok(new { success = true, isFavorite });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex, "Lỗi khi toggle favorite");
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống." });
        }
    }

    // Check favorite
    [HttpGet("is-favorite/{plantId}")]
    public async Task<IActionResult> IsFavorite(string plantId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
        {
            LoggerHelper.Warn("Yêu cầu kiểm tra favorite nhưng user không hợp lệ.");
            return Unauthorized();
        }

        try
        {
            LoggerHelper.Info($"Check isFavorite: user={userId}, plantId={plantId}");
            var isFavorite = await _service.IsFavoriteAsync(userId, plantId);
            LoggerHelper.Info(
                $"Kết quả isFavorite: user={userId}, plantId={plantId}, isFavorite={isFavorite}"
            );
            return Ok(new { success = true, isFavorite });
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex, "Lỗi khi kiểm tra trạng thái favorite");
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống." });
        }
    }
}
