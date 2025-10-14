using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Plantpedia.Pages.Plant
{
    public class DetailsModel : PageModel
    {
        // Dùng trong .cshtml để JS lấy plantId
        public string PlantId { get; private set; } = string.Empty;

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Plant ID is required.");
            }

            PlantId = id;

            // Nếu chưa đăng nhập: chuẩn bị LoginUrl + cờ RequireLogin
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                var req = HttpContext.Request;
                ViewData["LoginUrl"] = Url.Page(
                    pageName: "/Auth/Login",
                    pageHandler: null,
                    values: new { ReturnUrl = req.Path + req.QueryString },
                    protocol: req.Scheme
                );
                ViewData["RequireLogin"] = true;
            }

            // Tiêu đề tạm, JS sẽ cập nhật lại
            ViewData["Title"] = "Đang tải chi tiết...";
            return Page();
        }
    }
}
