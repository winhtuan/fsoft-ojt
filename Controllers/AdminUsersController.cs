using Microsoft.AspNetCore.Mvc;
using Plantpedia.DTO;
using Plantpedia.Enum;
using Plantpedia.Service;

namespace Plantpedia.Controllers;

[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserAdminService _svc;

    public AdminUsersController(IUserAdminService svc) => _svc = svc;

    // GET: /api/admin/users/list?q=&status=&page=1&pageSize=20
    [HttpGet("list")]
    public async Task<ActionResult<PagedResult<AdminUserListItemDto>>> List(
        [FromQuery] string? q,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        UserStatus? st = null;
        if (
            !string.IsNullOrWhiteSpace(status)
            && System.Enum.TryParse<UserStatus>(status, out var s)
        )
            st = s;

        var result = await _svc.ListAsync(
            new AdminUserQuery
            {
                Q = q,
                Status = st,
                Page = page,
                PageSize = pageSize,
            }
        );
        return Ok(result);
    }

    // POST: /api/admin/users/create
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (dto is null)
            return BadRequest("Payload is required.");
        var id = await _svc.CreateAsync(
            dto.Username,
            dto.Email,
            dto.Name,
            dto.Gender,
            dto.DateOfBirth,
            dto.AvatarUrl,
            dto.Password
        );
        return Ok(new { ok = true, id });
    }

    // POST: /api/admin/users/update
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
        if (dto is null)
            return BadRequest("Payload is required.");
        var ok = await _svc.UpdateAsync(
            dto.UserId,
            dto.Name,
            dto.Gender,
            dto.DateOfBirth,
            dto.AvatarUrl,
            dto.Status
        );
        return Ok(new { ok });
    }

    // POST: /api/admin/users/delete
    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] IdDto dto)
    {
        if (dto is null)
            return BadRequest("Payload is required.");
        var ok = await _svc.SoftDeleteAsync(dto.Id);
        return Ok(new { ok });
    }

    // POST: /api/admin/users/restore
    [HttpPost("restore")]
    public async Task<IActionResult> Restore([FromBody] IdDto dto)
    {
        if (dto is null)
            return BadRequest("Payload is required.");
        var ok = await _svc.RestoreAsync(dto.Id);
        return Ok(new { ok });
    }

    // DTOs (có thể tách file riêng vào Plantpedia.DTO)
    public record CreateUserDto(
        string Username,
        string Email,
        string Name,
        char Gender,
        DateTime DateOfBirth,
        string? AvatarUrl,
        string Password
    );

    public record UpdateUserDto(
        int UserId,
        string Name,
        char Gender,
        DateTime DateOfBirth,
        string? AvatarUrl,
        UserStatus Status
    );

    public record IdDto(int Id);
}
