using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Plantpedia.Pages.Plant
{
    public class DetailsModel : PageModel
    {
        // Thuộc tính này sẽ được dùng trong file .cshtml để truyền ID cho JavaScript
        public string PlantId { get; private set; }

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Plant ID is required.");
            }

            // Gán ID để view có thể sử dụng
            PlantId = id;

            // Đặt một tiêu đề tạm thời, JS sẽ cập nhật lại sau khi tải dữ liệu
            ViewData["Title"] = "Đang tải chi tiết...";

            return Page();
        }
    }
}
