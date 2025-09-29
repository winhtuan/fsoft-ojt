using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper;

namespace Plantpedia.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public LogoutModel() { }

        public async Task<IActionResult> OnPostAsync()
        {
            string username = User.GetUsername() ?? "Unknown";
            LoggerHelper.Info($"Người dùng '{username}' đang thực hiện đăng xuất.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            LoggerHelper.Info(
                $"Người dùng '{username}' đã đăng xuất thành công. Chuyển hướng về trang chủ."
            );

            return RedirectToPage("/Home/Home");
        }
    }
}
