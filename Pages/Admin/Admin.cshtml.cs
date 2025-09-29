using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper;
using Plantpedia.Models;
using Plantpedia.Service;

namespace Plantpedia.Pages.Admin
{
    [Authorize]
    public class AdminModel : PageModel
    {
        private readonly IUserService _userService;

        public AdminModel(IUserService userService)
        {
            _userService = userService;
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
