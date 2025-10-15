using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Service;

namespace Plantpedia.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IPlantService _plantService;

        public int UserCount { get; set; }
        public int PlantCount { get; set; }

        public AdminModel(IUserService userService, IPlantService plantService)
        {
            _userService = userService;
            _plantService = plantService;
        }

        public UserAccount? CurrentUser { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            LoggerHelper.Info("Bắt đầu tải trang Admin cho người dùng.");

            try
            {
                int? userId = User.GetUserId();

                if (!userId.HasValue)
                {
                    LoggerHelper.Warn(
                        "Không thể lấy UserId từ Claims. Phiên đăng nhập có thể đã hết hạn hoặc không hợp lệ."
                    );
                    return Page();
                }

                CurrentUser = await _userService.GetUserByIdAsync(userId.Value);
                UserCount = await _userService.GetUserCountAsync();
                PlantCount = await _plantService.GetPlantCountAsync();

                if (CurrentUser == null)
                {
                    LoggerHelper.Warn(
                        $"Không tìm thấy user trong database với UserId: {userId.Value}."
                    );
                }
                else
                {
                    LoggerHelper.Info(
                        $"Tải thành công thông tin cho người dùng '{CurrentUser.LoginData.Username}' (UserId: {userId.Value})."
                    );
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(
                    "Một lỗi không mong muốn đã xảy ra trong AdminModel.OnGetAsync."
                );
                LoggerHelper.Error(ex);
            }

            return Page();
        }
    }
}
