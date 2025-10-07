using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Plantpedia.Helper;
using Plantpedia.Service;
using Plantpedia.ViewModel;

namespace Plantpedia.Controllers
{
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            LoggerHelper.Info($"Bắt đầu đăng ký AJAX với email: {model?.Email}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToErrorMessages();
                return BadRequest(new { errors });
            }

            try
            {
                if (await _userService.EmailExistsAsync(model!.Email))
                {
                    return BadRequest(new { errors = new[] { "Email đã được sử dụng." } });
                }

                var newUser = await _userService.RegisterAsync(model);
                LoggerHelper.Info(
                    $"Đăng ký thành công: UserId={newUser.UserId}, Email={model.Email}"
                );

                await HttpContext.SignInUserAsync(newUser, model.Email);

                var role = newUser.LoginData.Role.ToString().ToLowerInvariant();
                var redirectUrl =
                    role == "user" ? Url.Page("/Home/Home")! : Url.Page("/Admin/Admin")!;

                return Ok(new { redirectUrl });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, $"Đăng ký thất bại (Controller) với email: {model?.Email}");
                return StatusCode(
                    500,
                    new { errors = new[] { "Đăng ký thất bại: Có lỗi hệ thống." } }
                );
            }
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [FromForm] string username,
            [FromForm] string password,
            [FromQuery] string? returnUrl = null
        )
        {
            LoggerHelper.Info($"Bắt đầu đăng nhập AJAX với username: {username}");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(
                    new { errors = new[] { "Vui lòng nhập đầy đủ thông tin đăng nhập." } }
                );
            }

            try
            {
                var user = await _userService.LoginAsync(username, password);
                if (user == null)
                {
                    return BadRequest(
                        new { errors = new[] { "Tên đăng nhập hoặc mật khẩu không đúng!" } }
                    );
                }

                await HttpContext.SignInUserAsync(user, user.LoginData.Username);

                string redirectUrl;
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    redirectUrl = returnUrl;
                }
                else
                {
                    var role = user.LoginData.Role.ToString().ToLowerInvariant();
                    redirectUrl =
                        role == "user" ? Url.Page("/Home/Home")! : Url.Page("/Admin/Admin")!;
                }

                return Ok(new { redirectUrl });
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex, "Lỗi hệ thống khi đăng nhập (Controller).");
                return StatusCode(
                    500,
                    new { errors = new[] { "Có lỗi hệ thống. Vui lòng thử lại sau." } }
                );
            }
        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            string username = User.Identity?.Name ?? "Unknown";
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
