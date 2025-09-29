using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plantpedia.Helper; // Thêm using cho LoggerHelper
using Plantpedia.Models;
using Plantpedia.Service;

namespace Plantpedia.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        // Thêm thuộc tính cho checkbox "Remember Me"
        [BindProperty]
        public bool RememberMe { get; set; }

        public bool LoginFailed { get; set; } = false;
        public string? ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        #region Public Methods
        public IActionResult OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Admin/Admin");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            LoggerHelper.Info($"Bắt đầu quá trình đăng nhập cho người dùng '{Username}'.");

            try
            {
                var user = await _userService.LoginAsync(Username, Password);
                if (user != null)
                {
                    await HandleSuccessfulLoginAsync(user);
                    LoggerHelper.Info(
                        $"Người dùng '{Username}' (UserId: {user.UserId}) đăng nhập thành công."
                    );
                    return RedirectAfterLogin();
                }
                else
                {
                    LoggerHelper.Warn(
                        $"Đăng nhập thất bại cho người dùng '{Username}'. Sai thông tin đăng nhập."
                    );
                    return Page();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Lỗi hệ thống trong quá trình đăng nhập.");
                LoggerHelper.Error(ex);
                return Page();
            }
        }
        #endregion

        #region Private Methods
        private async Task HandleSuccessfulLoginAsync(UserAccount user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.LoginData.Username),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // CẢI TIẾN: Thêm AuthenticationProperties để hỗ trợ "Remember Me"
            var authProperties = new AuthenticationProperties
            {
                // Nếu người dùng check "Remember Me", cookie sẽ tồn tại sau khi đóng trình duyệt.
                IsPersistent = RememberMe,

                // Thời gian sống của cookie, nên đồng bộ với cấu hình trong Program.cs
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),

                // Tự động làm mới cookie nếu người dùng hoạt động
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties // Truyền thuộc tính vào đây
            );
        }

        private IActionResult RedirectAfterLogin()
        {
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToPage("/Admin/Admin");
        }
        #endregion
    }
}
